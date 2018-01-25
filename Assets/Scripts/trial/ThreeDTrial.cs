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
            t.PickupType = Value.PickupType;
            t.EnvironmentType = Value.EnvironmentType;
            t.Sides = Value.Sides;
            t.BlockID = BlockID;
            t.TrialID = TrialID;
            t.TwoDim = Value.TwoDimensional;
            t.Visible = Value.PickupVisible;
            t.LastX = t.TargetX;
            t.LastY = t.TargetY;
            t.TargetX = 0;
            t.TargetY = 0;
            SceneManager.LoadScene(Value.EnvironmentType);
        }

        public override void Progress()
        {
            TrialProgress.Num3D++;

            base.Progress();
        }

        public override void Notify()
        {
            
            TrialProgress.NumSuccess++;

        }
    }
}