using System;
using UnityEngine;

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
            t.TimingVerification = data.DataSingleton.GetData().TimingVerification; // timing diagnostics
            t.TrialID = TrialID;
            t.BlockID = BlockID;
            t.Instructional = trialData.Instructional;
            LoadNextSceneWithTimer(trialData.Scene);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Default to space key
            var trialEndKeyCode = "space";

            if (!String.IsNullOrEmpty(trialData.TrialEndKey))
            {
                trialEndKeyCode = trialData.TrialEndKey.ToLower();
            }

            if (Input.GetKey(trialEndKeyCode))
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}