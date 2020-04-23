using contingency;
using data;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using trial;
using UnityEngine;
using value;

namespace Tests.trial
{
    public class TrialServiceTest
    {
        private Mock<IContingencyServiceFactory> _contingencyServiceFactoryMock;

        [SetUp]
        public void Setup()
        {
            _contingencyServiceFactoryMock = new Mock<IContingencyServiceFactory>();
        }

        // Basic Trial Check

        [Test]
        public void TestGenerateBasicTrials()
        {
            var config = @"{
                ""BlockOrder"": [1, 2],
                ""Blocks"": [
                    {
                        ""TrialOrder"": [1, 2, 3],
                        ""Contingencies"": [{
                            ""ContingencyFunction"": ""TempFunction"",
                            ""ForTrials"": [1],
                        }]
                    },
                    {
                        ""TrialOrder"": [1, 2, 3]
                    }
                ],
                ""Trials"": [
                    {
                      ""FileLocation"": ""file""
                    },
                    {
                      ""TwoDimensional"": 1 
                    },
                    {}
                ]
            }";

            var data = JsonConvert.DeserializeObject<Data>(config);
            var service = ConstructTrialService(data);

            AbstractTrial appliedTrial = null;
            var root = AbstractTrial.TempHead;
            _contingencyServiceFactoryMock.Setup(e => e.CreateEmpty());
            _contingencyServiceFactoryMock
                .Setup(e =>
                    e.Create(data.Blocks[0].Contingencies[0], It.IsAny<AbstractTrial>()))
                .Callback<Data.Contingency, AbstractTrial>((_, trial) => appliedTrial = trial);

            service.GenerateAllStartingTrials(root);
            _contingencyServiceFactoryMock.Verify(e => e.CreateEmpty(), Times.Exactly(5));
            _contingencyServiceFactoryMock.Verify(e => e
                .Create(data.Blocks[0].Contingencies[0], It.IsAny<AbstractTrial>()), Times.Exactly(1));

            Assert.AreEqual(appliedTrial.TrialId, new TrialId(1));
            Assert.AreEqual(appliedTrial.SourceTrial, appliedTrial);

            root = root.next;
            Debug.Log(root.GetType());

            Assert.False(root.isTail);
            Assert.AreEqual(root, root.SourceTrial);
            Assert.AreEqual(root.GetType(), typeof(InstructionalTrial));
            root = root.next;

            Assert.False(root.isTail);
            Assert.AreEqual(root.GetType(), typeof(TwoDTrial));
            root = root.next;

            Assert.True(root.isTail);
            Assert.AreEqual(root.GetType(), typeof(ThreeDTrial));
            root = root.next;

            Assert.False(root.isTail);
            Assert.AreEqual(root.GetType(), typeof(InstructionalTrial));
            root = root.next;

            Assert.False(root.isTail);
            Assert.AreEqual(root.GetType(), typeof(TwoDTrial));
            root = root.next;

            Assert.True(root.isTail);
            Assert.AreEqual(root.GetType(), typeof(ThreeDTrial));
            root = root.next;

            Assert.AreEqual(root.GetType(), typeof(CloseTrial));
        }


        private ITrialService ConstructTrialService(Data data)
        {
            return TestUtils.Construct<TrialService>(
                new[] {typeof(Data), typeof(ContingencyService.ContingencyServiceFactory)},
                new object[] {data, _contingencyServiceFactoryMock.Object});
        }
    }
}
