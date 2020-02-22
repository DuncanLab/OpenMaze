using System.Collections.Generic;

using main;
using UnityEngine;

namespace trial
{
    // The is a data container that pumps data forward through the trials.
    public class TrialProgress
    {
        public AbstractTrial PreviousTrial;
        public float TimeSinceExperimentStart;
        public float NumSuccess;
        public float Num3D;

        public List<int> successes; // Whether a trial was a success or not (1 or 0).
        public int[] NumCollectedPerBlock; // Number of goals during each block.
        public int TrialNumber;
        public int EnvironmentType;
        public bool TimingVerification; // timing diagnostics boolean
        public int CurrentEnclosureIndex;
        public int PickupType;
        public float TargetX;
        public float TargetY;
        public int BlockID;
        public int TrialID;
        public string Subject;
        public string Condition;
        public int TwoDim;
        public int Instructional;
        public int Visible;
        public float LastX;
        public float LastY;
        public string SessionID;

        public bool isLoaded = true;

        public Transform playerTransform;

        public TrialProgress()
        {
            TrialNumber = -1;
            Num3D = 0;
        }

        public string Note { get; set; }

        public static AbstractTrial GetCurrTrial()
        {
            return Loader.Get().CurrTrial;
        }

        public void ResetOngoing()
        {
            NumSuccess = 0;
            Num3D = 0;
            TrialNumber = -1;
        }
    }
}