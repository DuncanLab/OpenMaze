using data;
using UnityEngine;

namespace trial
{
    //We have the ClosingTrial.
    
    //If we are in this state, then we finish the experiment.
    public class CloseTrial : AbstractTrial
    {
        public CloseTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }

        public override void Progress()
        {
            //This should never be called
        }

    }
}