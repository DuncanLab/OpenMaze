using data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace trial
{
    public class ThreeDTrial : TimeoutableTrial
    {
        
        
        public ThreeDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
            
        }

        public override void PreEntry(TrialProgress t)
        {
            base.PreEntry(t);
            SceneManager.LoadScene(Value.EnvironmentType);
        }

        public override void Progress()
        {
            base.Progress();
            TrialProgress.Num3D++;
        }

        public override void Register()
        {
            TrialProgress.NumSuccess++;
        }
    }
}