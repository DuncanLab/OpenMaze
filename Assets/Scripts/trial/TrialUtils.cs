using UnityEngine;
using UnityEngine.UI;
using data;
using value;

namespace trial
{
    public static class TrialUtils
    {
        /**
         * A basic trial is a trial with a trialIdx and trialLocation
         */
        public static AbstractTrial GenerateBasicTrialFromConfig(BlockId blockId, TrialId trialId, Data.Trial trialDataFromIndex)
        {
            // Control flow here is for deciding what Trial gets spat out from the config
            AbstractTrial currTrial;
            if (trialDataFromIndex.FileLocation != null)
            {
                Debug.Log("Creating new Instructional Trial");
                currTrial = new InstructionalTrial(blockId, trialId);
            }
            else if (trialDataFromIndex.TwoDimensional == 1)
            {
                Debug.Log("Creating new 2D Screen Trial");
                currTrial = new TwoDTrial(blockId, trialId);
            }
            else
            {
                Debug.Log("Creating new 3D Screen Trial");
                currTrial = new ThreeDTrial(blockId, trialId);
            }

            return currTrial;
        }
    }
}