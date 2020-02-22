using System;
using data;
using UnityEngine;

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

            LoadNextSceneWithTimer(trialData.Scene);
        }

        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            var trialEndKeyCode = trialData.TrialEndKey;
            var ignoreUserInputDelay = DataSingleton.GetData().IgnoreUserInputDelay;

            if (!String.IsNullOrEmpty(trialEndKeyCode) && Input.GetKey(trialEndKeyCode.ToLower()) && (_runningTime > ignoreUserInputDelay))
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}