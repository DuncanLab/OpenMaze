using System;
using data;
using UnityEngine;
using value;

namespace trial
{
    public class InstructionalTrial : TimeoutableTrial

    {
        public InstructionalTrial(BlockId blockId, TrialId trialId) : base(blockId, trialId)
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

            if (Input.GetKey(trialEndKeyCode) && (_runningTime > ignoreUserInputDelay))
            {
                Debug.Log(_runningTime);
                Progress();
            }

            //Exit the experiment 
            if (ExitButton.clicked == true | Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}