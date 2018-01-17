using data;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace trial
{
    public class TwoDTrial : TimeoutableTrial
    {
        public TwoDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }
        
        
        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            SceneManager.LoadScene(Value.EnvironmentType);
        }

        
        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Input.GetKey(KeyCode.Space))
            {
                Progress();
            }
        }
    }
}