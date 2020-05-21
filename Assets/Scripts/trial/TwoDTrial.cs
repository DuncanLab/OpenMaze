using data;
using UnityEngine;
using value;

namespace trial
{
    //This is a two dimensional trial
    public class TwoDTrial : TimeoutableTrial
    {
        public TwoDTrial(Data data, BlockId blockId, TrialId trialId) : base(data, blockId, trialId)
        {
        }

        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            var trialEndKeyCode = trialData.TrialEndKey;
            var ignoreUserInputDelay = DataSingleton.GetData().IgnoreUserInputDelay;

            if (!string.IsNullOrEmpty(trialEndKeyCode) && Input.GetKey(trialEndKeyCode.ToLower()) &&
                _runningTime > ignoreUserInputDelay)
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}
