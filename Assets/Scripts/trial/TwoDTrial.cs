using data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace trial
{
    //This is a two dimensional trial
    public class TwoDTrial : TimeoutableTrial
    {
        public TwoDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }
        
        
        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            t.EnvironmentType = Value.EnvironmentType;
            t.Sides = Value.Sides;
            t.BlockID = BlockID;
            t.TrialID = TrialID;
            t.TwoDim = Value.TwoDimensional;
            t.LastX = t.TargetX;
            t.LastY = t.TargetY;
            t.TargetX = 0;
            t.TargetY = 0;
            SceneManager.LoadScene(Value.EnvironmentType);
        }

        
        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0))
            {
                Progress();
            }
        }
    }
}