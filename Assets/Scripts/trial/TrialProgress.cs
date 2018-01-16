using System;
using System.Diagnostics;
using data;

namespace trial
{
    public class TrialProgress
    {
        public AbstractTrial PreviousTrial;
        public int NumProgressed;
        public DateTime StartTime;
        public int NumSuccess;

        public TrialProgress()
        {
            StartTime = DateTime.Now;
            NumProgressed = -1;
            Num3D = 0;
            NumProgressed = 0;
        }

        public int Num3D { get; set; }
    }
}