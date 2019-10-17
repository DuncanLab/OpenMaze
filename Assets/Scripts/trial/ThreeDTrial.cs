using System;
using data;
using UnityEngine;

namespace trial
{
    public class ThreeDTrial : TimeoutableTrial
    {
        public ThreeDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {

            // Sets the field of the preentry
            base.PreEntry(t, first);
            t.TimingVerification = data.DataSingleton.GetData().TimingVerification; // timing diagnostics
            t.EnvironmentType = Value.EnvironmentType;
            t.CurrentMazeName = Value.MazeName;
            t.BlockID = BlockID;
            t.TrialID = TrialID;
            t.TwoDim = Value.TwoDimensional;
            t.Instructional = 0;
            t.LastX = t.TargetX;
            t.LastY = t.TargetY;
            t.TargetX = 0;
            t.TargetY = 0;
            _runningTime -= Value.TimeToRotate;

            LoadNextSceneWithTimer(Value.EnvironmentType);
        }

        public override void Progress()
        {
            TrialProgress.Num3D++;

            // If we are progressing without a success, record the failure
            // as a zero, otherwise record a 1.
            if (isSuccessful)
            {
                TrialProgress.successes.Add(1);
            }
            else
            {
                TrialProgress.successes.Add(0);
            }
            isSuccessful = false;
            base.Progress();
        }

        public override void Notify()
        {
            // Record that this particular trial was a success
            TrialProgress.NumSuccess++;
            isSuccessful = true;

        }

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