using System;
using data;
using trial;
using UnityEngine;
using E = main.Loader;
using DS = data.DataSingleton;

namespace main
{    
    //Collection of classes that are called with reflection.
    //These are called at the end of the trail and are associated with the given pickup file.
    //Return true to repeat
    public class Functions
    {
        // ReSharper disable once UnusedMember.Global
        // Called with reflection
        public static bool CheckFoodThresholdPercentage(TrialProgress tp)
        {

            var curr = TrialProgress.GetCurrTrial();

            var blockId = curr.BlockID;
            var bd = DS.GetData().BlockList[blockId];

            var numSuccessfulInPrevious = bd.EndGoal.Split(' ');
            var numSuccessfulRequired = float.Parse(numSuccessfulInPrevious[0]);
            int previousTrialsToCheck = int.Parse(numSuccessfulInPrevious[1]);

            // Automatically continue if we haven't reached the required run length
            if (tp.successes.Count < previousTrialsToCheck) return true;

            var successCountInRunLength = 0;
            for (int i = tp.successes.Count - previousTrialsToCheck; i < tp.successes.Count; i++)
            {
                if (tp.successes[i] == 1) successCountInRunLength++;
            }

            Debug.Log(string.Format(
                "Number of Successes required: {0}, Actual: {1}, Previous trials to check: {2}", 
                numSuccessfulRequired, successCountInRunLength, previousTrialsToCheck));
            
            return successCountInRunLength < numSuccessfulRequired;
        }
    }
}