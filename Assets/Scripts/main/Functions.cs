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
        public static bool SuccessesCriterion(TrialProgress tp)
        {

            var curr = TrialProgress.GetCurrTrial();

            var blockId = curr.BlockID;
            var bd = DS.GetData().Blocks[blockId];
            var goal = bd.BlockGoal ?? bd.TrialGoal;
            var numSuccessfulInPrevious = goal.Split(' ');
            var numSuccessfulRequired = float.Parse(numSuccessfulInPrevious[0]);

            var successCount = 0;

            if (numSuccessfulInPrevious.Length == 0)
            {
                Debug.LogError("Missing arguments for threshold function in config");
            }

            // User has both values set in the config
            if (numSuccessfulInPrevious.Length == 2)
            {
                int previousTrialsToCheck = int.Parse(numSuccessfulInPrevious[1]);
                // Continue if we haven't reached the required run length
                if (tp.successes.Count < previousTrialsToCheck) return true;

                for (int i = tp.successes.Count - previousTrialsToCheck; i < tp.successes.Count; i++)
                {
                    if (tp.successes[i] == 1) successCount++;
                }

                Debug.Log(string.Format(
                    "Number of Successes required: {0}, Actual: {1}, Previous trials to check: {2}",
                    numSuccessfulRequired, successCount, previousTrialsToCheck));

            }
            else
            {
                for (int i = 0; i < tp.successes.Count; i++)
                {
                    if (tp.successes[i] == 1) successCount++;
                }

                Debug.Log(string.Format(
                    "Number of Successes required: {0}, Actual: {1}, Previous trials to check: ALL",
                    numSuccessfulRequired, successCount));
            }

            return successCount < numSuccessfulRequired;
        }
    }
}