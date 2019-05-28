using data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace trial
{
    public class InstructionalTrial : TimeoutableTrial

    {
        public InstructionalTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            t.TrialID = TrialID;
            t.BlockID = BlockID;
            t.Instructional = Value.Instructional;
            SceneManager.LoadScene(Constants.LoadingScreen);
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