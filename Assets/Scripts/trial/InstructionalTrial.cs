using System;
using data;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            t.Instructional = Value.Instructional;
            SceneManager.LoadScene(Constants.LoadingScreen);
            t.isLoaded = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Default to space key
            var trialEndKeyCode = "space";

            if (!String.IsNullOrEmpty(Value.TrialEndKey))
            {
                trialEndKeyCode = Value.TrialEndKey.ToLower();
            }

            if (Input.GetKey(trialEndKeyCode))
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}