using System.Collections.Generic;
using main;
using UnityEngine;
using value;

namespace trial
{
    // The is a data container that pumps data forward through the trials.
    public class TrialProgress
    {
        public BlockId BlockId;
        public string Condition;
        public int CurrentEnclosureIndex;
        public int EnvironmentType;
        public int Instructional;

        public bool isLoaded = true;
        public float LastX;
        public float LastY;
        public int MegaBonus; // Number of mega bous points 
        public float Num3D;
        public int[] NumCollectedPerBlock; // Number of goals during each block.
        public float NumSuccess;
        public int PickupType;

        public Transform playerTransform;
        public AbstractTrial PreviousTrial;
        public string SessionID;
        public string Subject;

        public List<int> successes; // Whether a trial was a success or not (1 or 0).
        public float TargetX;
        public float TargetY;
        public float TimeSinceExperimentStart;
        public bool TimingVerification; // timing diagnostics boolean
        public TrialId TrialId;
        public int TrialNumber;
        public int TwoDim;
        public int Visible;

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