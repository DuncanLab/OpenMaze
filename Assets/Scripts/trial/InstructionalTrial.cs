using System;
using data;
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

            LoadNextSceneWithTimer(Constants.LoadingScreen);
            t.isLoaded = true;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Default to space key
            var trialEndKeyCode = "space";
            var ignoreUserInputDelay = DataSingleton.GetData().IgnoreUserInputDelay;

            if (!String.IsNullOrEmpty(trialData.TrialEndKey.ToLower()))
            {
                trialEndKeyCode = trialData.TrialEndKey.ToLower();
            }
            else
            {
                trialEndKeyCode = "space";
            }

            if (Input.GetKey(trialEndKeyCode) && (_runningTime > ignoreUserInputDelay))
            {
                Debug.Log(_runningTime);
                Progress();
            }

            //Exit the experiment 
            if (ExitButton.clicked == true)
            {
                Application.Quit();
            }
        }
    }
}