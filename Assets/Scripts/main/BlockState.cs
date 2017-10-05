using System.Collections.Generic;
using UnityEngine;

namespace main
{   
    //Here is the primary interface that 
    public static class BlockState
    {

        private static double  _blockTime;

        private static readonly List<int> TrialSuccess = new List<int>();

        public static double GetCurrentBlockTime()
        {        
            return _blockTime;
        }

        public static int GetNumberItemsFound(int total = 0)
        {
            var goal = Mathf.Max(TrialSuccess.Count - total, 0);
            var sum = 0;
            for (var i = goal; i < TrialSuccess.Count; i++)
            {
                sum += TrialSuccess[i];
            }
            return sum;
        }

        public static int GetBlockLength()
        {
            return TrialSuccess.Count;
        }
        
        
        
        
        //------------------- These are the game methods. Don't touch these -----------------------------
        
        public static void Update(double deltaTime)
        {
            _blockTime += deltaTime;
        }
        
        public static void Reset()
        {
            _blockTime = 0;
            TrialSuccess.Clear();
        }
        
        
        public static void Found()
        {
            TrialSuccess.Add(1);
        }

        public static void Failed()
        {

            TrialSuccess.Add(0);
        }
        
        
    }
}