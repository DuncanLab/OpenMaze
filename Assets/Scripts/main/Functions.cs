using System;
using data;
using trial;
using UnityEngine;

namespace main
{
    public class Functions
    {
        public static bool CheckFoodThresholdPercentage(TrialProgress tp)
        {
            Debug.Log((DateTime.Now - tp.StartTime).TotalSeconds);
             
            return tp.Num3D < 5;
        }
    }
}