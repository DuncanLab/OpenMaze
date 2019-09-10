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
            t.EnvironmentType = Value.EnvironmentType;
            t.Sides = Value.Sides;
            t.BlockID = BlockID;
            t.TrialID = TrialID;
            t.TwoDim = Value.TwoDimensional;
            t.Instructional = 0;
            t.LastX = t.TargetX;
            t.LastY = t.TargetY;
            t.TargetX = 0;
            t.TargetY = 0;

            LoadNextSceneWithTimer(Value.EnvironmentType);
        }

        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            var trialEndKeyCode = Value.TrialEndKey;
            var ignoreUserInputDelay = DataSingleton.GetData().IgnoreUserInputDelay;

            if (!String.IsNullOrEmpty(trialEndKeyCode) && Input.GetKey(trialEndKeyCode.ToLower()) && (_runningTime > ignoreUserInputDelay))
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}