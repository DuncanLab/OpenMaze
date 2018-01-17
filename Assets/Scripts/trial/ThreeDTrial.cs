using data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace trial
{
    public class ThreeDTrial : TimeoutableTrial
    {
        
        
        public ThreeDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
            
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            SceneManager.LoadScene(Value.EnvironmentType);
        }

        public override void Progress()
        {
            TrialProgress.Num3D++;

            base.Progress();
        }

        public override void Notify()
        {
            var text = GameObject.Find("CountDown").GetComponent<Text>();
            
            TrialProgress.NumSuccess++;
            text.text = "FOUND: " + TrialProgress.NumSuccess;

        }
    }
}