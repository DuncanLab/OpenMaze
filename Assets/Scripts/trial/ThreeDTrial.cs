using data;
using UnityEngine;

namespace trial
{
    public class ThreeDTrial : AbstractTrial
    {
        public ThreeDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }
        
        //Here is the entry point to the current trial, in PreEntry, we pass in the data from the previou
        public override void PreEntry(TrialProgress t)
        {
            throw new System.NotImplementedException();
        }

        

        public override void Progress()
        {
            throw new System.NotImplementedException();
        }

    }
}