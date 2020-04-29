using UnityEngine;
using value;

namespace trial
{
    //We have the ClosingTrial.

    //If we are in this state, then we finish the experiment.
    public class CloseTrial : AbstractTrial
    {
        public CloseTrial() : base(null, BlockId.EMPTY, TrialId.EMPTY)
        {
            IsHead = true;
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            Debug.Log("In close trial");
        }


        public override void Progress()
        {
            //This should never be called
        }
    }
}
