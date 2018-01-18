using System;
using System.Diagnostics;
using data;
using main;

namespace trial
{
    public class TrialProgress
    {
        public AbstractTrial PreviousTrial;
        public DateTime StartTime;
        public float NumSuccess;
        public float Num3D;
        
        
        public int TrialNumber;
        public int EnvironmentType;
        public int Sides;
        public int PickupType;
        public int TargetX;
        public int TargetY;
        public int BlockID;
        public int TrialID;
        public string Subject;
        public string Delay;
        public int TwoDim;
        public int Visible;
        public int LastX;
        public int LastY;
        
        
        public TrialProgress()
        {
            StartTime = DateTime.Now;
            TrialNumber = -1;
            Num3D = 0;
            TrialNumber = 0;
        }


        public static AbstractTrial GetCurrTrial()
        {
            return Loader.Get().CurrTrial;
        }

    }
}