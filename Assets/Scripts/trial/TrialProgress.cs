using System;
using System.Diagnostics;
using data;
using main;

namespace trial
{
    public class TrialProgress
    {
        public AbstractTrial PreviousTrial;
        public float NumProgressed;
        public DateTime StartTime;
        public float NumSuccess;

        public TrialProgress()
        {
            StartTime = DateTime.Now;
            NumProgressed = -1;
            Num3D = 0;
            NumProgressed = 0;
        }


        public AbstractTrial getCurrTrial()
        {
            return Loader.Get().CurrTrial;
        }
        public float Num3D { get; set; }
    }
}