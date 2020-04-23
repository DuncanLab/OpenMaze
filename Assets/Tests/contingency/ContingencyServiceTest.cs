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
        private Mock<IContingencyBehaviourValidator> _contingencyBehaviourValidator;
        private Mock<IContingencyFunctionCaller> _contingencyFunctionCaller;
        private Mock<ITrialService> _trialService;
        private AbstractTrial _trial;
        private TrialProgress tp = new TrialProgress();
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
                .Setup(e => e.InvokeContingencyFunction(tp, data.Blocks[0].Contingencies[0]))
                .Returns("A");
            _contingencyBehaviourValidator
                .Setup(e =>
                    e.ValidateContingencyBehaviour(It.IsAny<Data.ContingencyBehaviour>()))
                .Returns(true);
            var t1 = new ThreeDTrial(data, new BlockId(1), new TrialId(1));
            _trial.next = t1;
            var t2 = new ThreeDTrial(data, new BlockId(1), new TrialId(2)) {isTail = true};
            t1.next = t2;
            var t3 = new ThreeDTrial(data, new BlockId(2), new TrialId(1));
            t2.next = t3;
            var t4 = new ThreeDTrial(data, new BlockId(2), new TrialId(2));
            t3.next = t4;
            var contingencyService = CreateContingencyService(t1, data, data.Blocks[0].Contingencies[0]);

            var returnedTrial = contingencyService.ExecuteContingency(tp);
            Assert.AreEqual(returnedTrial, t3);
        }
        
        [Test]
        public void TestContingency_RepeatContingency()
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
                                        ""RepeatContingency"": true
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
                .Setup(e => e.InvokeContingencyFunction(tp, data.Blocks[0].Contingencies[0]))
                .Returns("A");
            _contingencyBehaviourValidator
                .Setup(e =>
                    e.ValidateContingencyBehaviour(It.IsAny<Data.ContingencyBehaviour>()))
                .Returns(true);
            var t1 = new ThreeDTrial(data, new BlockId(1), new TrialId(1));
            t1.SourceTrial = t1;
            _trial.next = t1;
            var t2 = new ThreeDTrial(data, new BlockId(1), new TrialId(2)) {isTail = true};
            t1.next = t2;
            var t3 = new ThreeDTrial(data, new BlockId(2), new TrialId(1));
            t2.next = t3;
            var t4 = new ThreeDTrial(data, new BlockId(2), new TrialId(2));
            t3.next = t4;
            var contingencyService = CreateContingencyService(t1, data, data.Blocks[0].Contingencies[0]);

            var returnedTrial = contingencyService.ExecuteContingency(tp);
            Assert.AreEqual(returnedTrial, t1);
        }
        
                [Test]
        public void TestContingency_()
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
                                        ""RepeatContingency"": true
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
                .Setup(e => e.InvokeContingencyFunction(tp, data.Blocks[0].Contingencies[0]))
                .Returns("A");
            _contingencyBehaviourValidator
                .Setup(e =>
                    e.ValidateContingencyBehaviour(It.IsAny<Data.ContingencyBehaviour>()))
                .Returns(true);
            var t1 = new ThreeDTrial(data, new BlockId(1), new TrialId(1));
            t1.SourceTrial = t1;
            _trial.next = t1;
            var t2 = new ThreeDTrial(data, new BlockId(1), new TrialId(2)) {isTail = true};
            t1.next = t2;
            var t3 = new ThreeDTrial(data, new BlockId(2), new TrialId(1));
            t2.next = t3;
            var t4 = new ThreeDTrial(data, new BlockId(2), new TrialId(2));
            t3.next = t4;
            var contingencyService = CreateContingencyService(t1, data, data.Blocks[0].Contingencies[0]);

            var returnedTrial = contingencyService.ExecuteContingency(tp);
            Assert.AreEqual(returnedTrial, t1);
        }


        private IContingencyService 
            CreateContingencyService(AbstractTrial trial, Data data, Data.Contingency contingency)
        {
            return TestUtils.Construct<ContingencyService>(
                new []
                {
                    typeof(IContingencyBehaviourValidator),
                    typeof(IContingencyFunctionCaller),
                    typeof(ITrialService),
                    typeof(Data),
                    typeof(Data.Contingency),
                    typeof(AbstractTrial)
                },

                new object []{
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