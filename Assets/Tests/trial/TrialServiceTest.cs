using System;
using System.IO;
using contingency;
using data;
using Newtonsoft.Json;
using NUnit.Framework;
using trial;
using UnityEngine;

namespace Tests.trial
{
    public class TrialServiceTest
    {
        // Basic Trial Check
        [Test]
        public void TestGenerateBasicTrials()
        {
            var config = @"{
              ""BlockOrder"": [1],
              ""Blocks"": [
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
            
            var root = AbstractTrial.TempHead;
            var service = ConstructTrialService(config);
            service.GenerateAllStartingTrials(root);
            root = root.next;
            Debug.Log(root.GetType());
            Assert.AreEqual(root.GetType(), typeof(InstructionalTrial));
            root = root.next;
            Assert.AreEqual(root.GetType(), typeof(TwoDTrial));
            root = root.next;
            Assert.AreEqual(root.GetType(), typeof(ThreeDTrial));
            root = root.next;
            Assert.AreEqual(root.GetType(), typeof(CloseTrial));
        }
        
        [Test]
        public void TestTrialContingency()
        {
          var config = $@"{{
              ""BlockOrder"": [1],
              ""Blocks"": [
                {{
                  ""TrialOrder"": [1],
                  ""ContingencyFunction"": ""Func"",
                  ""FunctionInput"": ""1 2"",
                  ""BehaviourByResult"": {{
                    
                  }}
              ],
              ""Trials"": [
                {{
                  ""FileLocation"": ""file""
                }}
              ]
            }}";
            
          var root = AbstractTrial.TempHead;
          var service = ConstructTrialService(config);
          service.GenerateAllStartingTrials(root);
          root = root.next;
          Debug.Log(root.GetType());
          Assert.AreEqual(root.GetType(), typeof(InstructionalTrial));
          root = root.next;
          Assert.AreEqual(root.GetType(), typeof(TwoDTrial));
          root = root.next;
          Assert.AreEqual(root.GetType(), typeof(ThreeDTrial));
          root = root.next;
          Assert.AreEqual(root.GetType(), typeof(CloseTrial));
        }

        
        private static ITrialService ConstructTrialService(string file)
        {
            var data = JsonConvert.DeserializeObject<Data>(file);
            return TestUtils.Construct<TrialService>(
              new[] {typeof(Data), typeof(ContingencyService.ContingencyServiceFactory)},
              new object[] {data});
        }
    }
}
        