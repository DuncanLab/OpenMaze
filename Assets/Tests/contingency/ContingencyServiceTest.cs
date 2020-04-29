using contingency;
using contingency.reflection;
using data;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using trial;
using value;

// ReSharper disable once CheckNamespace
namespace Tests
{
    public class ContingencyServiceTest
    {
        private readonly TrialProgress _tp = new TrialProgress();
        private Mock<IContingencyBehaviourValidator> _contingencyBehaviourValidator;
        private Mock<IContingencyFunctionCaller> _contingencyFunctionCaller;
        private AbstractTrial _trial;
        private Mock<ITrialService> _trialService;

        [SetUp]
        public void Setup()
        {
            _trial = new CloseTrial();
            _contingencyFunctionCaller = new Mock<IContingencyFunctionCaller>();
            _trialService = new Mock<ITrialService>();
            _contingencyBehaviourValidator = new Mock<IContingencyBehaviourValidator>();
        }

        [Test]
        public void TestContingency_EndBlock()
        {
            var config =
                @"{
                    ""BlockOrder"": [1, 2],
                    ""Blocks"": [
                        {
                            ""TrialOrder"": [1, 2],
                            ""Contingencies"": [{
                                ""ContingencyFunction"": ""TempFunction"",
                                ""ForTrials"": [1],
                                ""BehaviourByResult"": {
                                    ""A"": {
                                        ""EndBlock"": true
                                    }
                                }
                            }]
                        },
                        {
                            ""TrialOrder"": [1, 2]
                        }
                    ],
                    ""Trials"": [
                        {},
                        {}
                    ]
                }";

            var data = JsonConvert.DeserializeObject<Data>(config);
            _contingencyFunctionCaller
                .Setup(e => e.InvokeContingencyFunction(_tp, data.Blocks[0].Contingencies[0]))
                .Returns("A");
            _contingencyBehaviourValidator
                .Setup(e =>
                    e.ValidateContingencyBehaviour(It.IsAny<Data.ContingencyBehaviour>()))
                .Returns(true);
            var t1 = new ThreeDTrial(data, new BlockId(1), new TrialId(1)){IsHead = true};
            _trial.next = t1;
            var t2 = new ThreeDTrial(data, new BlockId(1), new TrialId(2));
            t1.next = t2;
            var t3 = new ThreeDTrial(data, new BlockId(2), new TrialId(1)){IsHead = true};
            t2.next = t3;
            var t4 = new ThreeDTrial(data, new BlockId(2), new TrialId(2));
            t3.next = t4;
            var contingencyService = CreateContingencyService(t1, data, data.Blocks[0].Contingencies[0]);

            var returnedTrial = contingencyService.ExecuteContingency(_tp);
            Assert.AreEqual(returnedTrial, t3);
        }

        [Test]
        public void TestContingency_AddTrialsAndRepeatContingency()
        {
            var config =
                @"{
                    ""BlockOrder"": [1, 2],
                    ""Blocks"": [
                        {
                            ""TrialOrder"": [1, 2],
                            ""Contingencies"": [{
                                ""ContingencyFunction"": ""TempFunction"",
                                ""ForTrials"": [1],
                                ""BehaviourByResult"": {
                                    ""A"": {
                                        ""NextTrials"": [3, 4]
                                    }
                                }
                            },
                            {
                                ""ContingencyFunction"": ""TempFunction2"",
                                ""ForTrials"": [4],
                                ""BehaviourByResult"": {
                                    ""B"": {
                                        ""RestartBlock"": true
                                    }
                                }
                            }]
                        }
                    ],
                    ""Trials"": [
                        {},
                        {},
                        {},
                        {}
                    ]
                }";

            var data = JsonConvert.DeserializeObject<Data>(config);
            _contingencyFunctionCaller
                .Setup(e => e.InvokeContingencyFunction(_tp, data.Blocks[0].Contingencies[0]))
                .Returns("A");
            _contingencyFunctionCaller
                .Setup(e => e.InvokeContingencyFunction(_tp, data.Blocks[0].Contingencies[1]))
                .Returns("B");
            _contingencyBehaviourValidator
                .Setup(e =>
                    e.ValidateContingencyBehaviour(It.IsAny<Data.ContingencyBehaviour>()))
                .Returns(true);

            var t13 = new ThreeDTrial(data, new BlockId(1), new TrialId(3));
            _trialService
                .Setup(e =>
                    e.GenerateBasicTrialFromConfig(new BlockId(1), new TrialId(3), data.Trials[2]))
                .Returns(t13);

            var t14 = new ThreeDTrial(data, new BlockId(1), new TrialId(4));
            _trialService
                .Setup(e =>
                    e.GenerateBasicTrialFromConfig(new BlockId(1), new TrialId(4), data.Trials[3]))
                .Returns(t14);

            _trialService
                .Setup(e =>
                    e.AddContingencyServiceToTrial(t14));

            var t11 = new ThreeDTrial(data, new BlockId(1), new TrialId(1)){IsHead = true};
            t11.SourceTrial = t11;
            t11.head = t11;
            _trial.next = t11;
            var t12 = new ThreeDTrial(data, new BlockId(1), new TrialId(2));
            t11.next = t12;
            t12.head = t11;
            var t21 = new ThreeDTrial(data, new BlockId(2), new TrialId(1)){IsHead = true};
            t12.next = t21;
            t21.head = t21;
            var t22 = new ThreeDTrial(data, new BlockId(2), new TrialId(2));
            t21.next = t22;
            t22.head = t21;
            var contingencyService = CreateContingencyService(t11, data, data.Blocks[0].Contingencies[0]);

            var returnedTrial = contingencyService.ExecuteContingency(_tp);

            _trialService.Verify(e =>
                e.AddContingencyServiceToTrial(t14), Times.Once);

            Assert.AreEqual(t13, returnedTrial);
            Assert.IsTrue(t13.IsGenerated);
            Assert.AreEqual(t11, t13.SourceTrial);

            Assert.AreEqual(returnedTrial.next, t14);
            Assert.IsTrue(t14.IsGenerated);
            Assert.AreEqual(t11, t14.SourceTrial);

            Assert.AreEqual(returnedTrial.next.next, t12);

            contingencyService = CreateContingencyService(t14, data, data.Blocks[0].Contingencies[1]);

            returnedTrial = contingencyService.ExecuteContingency(_tp);
            Assert.AreEqual(t11, returnedTrial);
        }


        private IContingencyService
            CreateContingencyService(AbstractTrial trial, Data data, Data.Contingency contingency)
        {
            return TestUtils.Construct<ContingencyService>(
                new[]
                {
                    typeof(IContingencyBehaviourValidator),
                    typeof(IContingencyFunctionCaller),
                    typeof(ITrialService),
                    typeof(Data),
                    typeof(Data.Contingency),
                    typeof(AbstractTrial)
                },
                new object[]
                {
                    _contingencyBehaviourValidator.Object,
                    _contingencyFunctionCaller.Object,
                    _trialService.Object,
                    data,
                    contingency,
                    trial
                });
        }
    }
}
