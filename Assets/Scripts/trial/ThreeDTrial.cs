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

            _runningTime -= trialData.Rotate;

            LoadNextSceneWithTimer(trialData.Scene);
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

            var trialEndKeyCode = trialData.TrialEndKey;
            var ignoreUserInputDelay = DataSingleton.GetData().IgnoreUserInputDelay;

            if (!String.IsNullOrEmpty(trialEndKeyCode))
            {
                trialEndKeyCode = trialData.TrialEndKey.ToLower();
            }

            if (Input.GetKey(trialEndKeyCode) && (_runningTime > ignoreUserInputDelay))
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}