using System;
using System.Collections.Generic;
using System.Diagnostics;
using data;
using JetBrains.Annotations;
using main;

namespace trial
{
    //The is a data container that pumps data forward through the trials.
    public class TrialProgress
    {
        public AbstractTrial PreviousTrial;
        public DateTime StartTime;
        public float NumSuccess;
        public float Num3D;
        public List<int> successes;

        public int TrialNumber;
        public int EnvironmentType;
        public int Sides;
        public int PickupType;
        public float TargetX;
        public float TargetY;
        public int BlockID;
        public int TrialID;
        public string Subject;
        public string Delay;
        public int TwoDim;
        public int Visible;
        public float LastX;
        public float LastY;
        public string SessionID;


        public TrialProgress()
        {
            StartTime = DateTime.Now;
            TrialNumber = -1;
            Num3D = 0;
            TrialNumber = 0;
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