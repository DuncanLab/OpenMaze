using System.Collections.Generic;
using UnityEngine;
using data;
using value;

namespace trial
{
    public static class TrialUtils
    {
        public static AbstractTrial GenerateContingentTrialFromConfig(BlockId blockId, int contingencyCount)
        {
            var data = DataSingleton.GetData();
            var block = data.Blocks[blockId.Value];
            if (block.ContingencyGraph == null || block.ContingencyGraph.Count <= contingencyCount)
            {
                Debug.LogError($"Contingency Graph List for block {blockId} doesn't have enough entries for the contingency count");
                Application.Quit();
                return null;
            }

            var contingencyGraph = new List<Data.ContingencyData>(block.ContingencyGraph[contingencyCount]);
            if (contingencyGraph.Count == 0)
            {
                Debug.LogError($"The {contingencyGraph.Count}th {blockId} doesn't have enough entries (need at least one)");
                Application.Quit();
                return null;
            }

            var currentContingency = contingencyGraph[0];
            
            var trialId = new TrialId(currentContingency.TrialIndex);
            var trial = GenerateBasicTrialFromConfig(blockId, trialId, data.Trials[trialId.Value]);
            trial.SetContingency(contingencyGraph, 0);

            return trial;
        }
        
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