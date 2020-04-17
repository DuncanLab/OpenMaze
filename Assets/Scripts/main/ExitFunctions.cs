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
    public class ExitFunctions
    {
        // ReSharper disable once UnusedMember.Global
        // Called with reflection
        public static bool CheckFoodThresholdPercentage(TrialProgress tp)
        {

            // var curr = TrialProgress.GetCurrTrial();
            //
            // var blockId = curr.BlockId;
            // var bd = DS.GetData().Blocks[blockId.Value];
            // var goal = bd.EndGoal ?? bd.ExitGoal;
            // var numSuccessfulInPrevious = goal.Split(' ');
            // var numSuccessfulRequired = float.Parse(numSuccessfulInPrevious[0]);
            //
            // var successCount = 0;
            //
            // if (numSuccessfulInPrevious.Length == 0)
            // {
            //     Debug.LogError("Missing arguments for threshold function in config");
            // }
            //
            // // User has both values set in the config
            // if (numSuccessfulInPrevious.Length == 2)
            // {
            //     int previousTrialsToCheck = int.Parse(numSuccessfulInPrevious[1]);
            //
            //     // Continue if we haven't reached the required run length
            //     if (tp.successes.Count < previousTrialsToCheck) return true;
            //
            //     for (int i = tp.successes.Count - previousTrialsToCheck; i < tp.successes.Count; i++)
            //     {
            //         if (tp.successes[i] == 1) successCount++;
            //     }
            //
            //     Debug.Log(
            //         $"Number of Successes required: {numSuccessfulRequired}, Actual: {successCount}, Previous trials to check: {previousTrialsToCheck}");
            //
            // }
            // else
            // {
            //     for (int i = 0; i < tp.successes.Count; i++)
            //     {
            //         if (tp.successes[i] == 1) successCount++;
            //     }
            //
            //     Debug.Log(
            //         $"Number of Successes required: {numSuccessfulRequired}, Actual: {successCount}, Previous trials to check: ALL");
            // }
            //
            // return successCount < numSuccessfulRequired;
            return false;
        }
    }
}