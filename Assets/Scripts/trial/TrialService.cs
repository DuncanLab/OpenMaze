using System.Collections.Generic;
using System.Linq;
using contingency;
using data;
using UnityEngine;
using value;

namespace trial
{
    public class TrialService : ITrialService
    {
        private readonly Data _data;
        private readonly IContingencyServiceFactory _contingencyServiceFactory;

        private TrialService(Data data, IContingencyServiceFactory contingencyServiceFactory)
        {
            _data = data;
            _contingencyServiceFactory = contingencyServiceFactory;
        }

        public static ITrialService Create()
        {
            return new TrialService(DataSingleton.GetData(), ContingencyService.ContingencyServiceFactory.Create());
        }

        private Dictionary<TrialId, Data.Contingency> ConstructContingencyByTrialMap(BlockId blockId)
        {
            var contingencies = _data.Blocks[blockId.Value].Contingencies;
            
            var contingencyByTrial = new Dictionary<TrialId, Data.Contingency>();

            foreach (var contingency in contingencies)
            {
                foreach (var trialId in contingency.ForTrials.Select(x => new TrialId(x)))
                {
                    contingencyByTrial[trialId] = contingency;
                }
            }

            return contingencyByTrial;
        }

        public void AddContingencyServiceToTrial(AbstractTrial trial)
        {
            var blockId = trial.BlockId;
            var trialId = trial.TrialId;
            var trialContingenciesForBlock =
                ConstructContingencyByTrialMap(blockId);
            if (trialContingenciesForBlock.ContainsKey(trialId))
            {
                // allows the trial to repeat itself even if it doesn't generate a new trial.
                trial.SourceTrial = trial;
                var contingencyService =
                    _contingencyServiceFactory.Create(trialContingenciesForBlock[trialId], trial);
                trial.SetContingency(contingencyService);
            }
            else
            {
                var contingencyService = _contingencyServiceFactory.CreateEmpty();
                trial.SetContingency(contingencyService);
            }
        }
        
        public void GenerateAllStartingTrials(AbstractTrial currentTrial)
        {
            foreach (var blockDisplayIndex in _data.BlockOrder)
            {
                var blockId = new BlockId(blockDisplayIndex);
                var block = _data.Blocks[blockId.Value];
                var newBlock = true;
                AbstractTrial currHead = null;
                
                var trialCount = 0;

                foreach (var trialDisplayIndex in block.TrialOrder)
                {
                    AbstractTrial newTrial;
                    var trialId = new TrialId(trialDisplayIndex);
                    switch (trialId.Value)
                    {
                        // Here we decide what each trial is, I guess we could do this with a function map, but later. 
                        // here we have a picture as a trial.
                        case -1:
                            newTrial = new RandomTrial(_data, this, _contingencyServiceFactory, blockId);
                            break;
                        default:
                            var newTrialData = _data.Trials[trialId.Value];
                            newTrial = GenerateBasicTrialFromConfig(blockId, trialId, newTrialData);
                            break;
                    }

                    AddContingencyServiceToTrial(newTrial);
                    

                    if (newBlock) currHead = newTrial;

                    newTrial.isTail = trialCount == block.TrialOrder.Count - 1;
                    newTrial.head = currHead;

                    currentTrial.next = newTrial;
                    currentTrial = currentTrial.next;

                    newBlock = false;
                    trialCount++;
                }

                currentTrial.next = new CloseTrial();
            }
        }

        /**
         * A basic trial is a trial with a trialIdx and trialLocation
         */
        public AbstractTrial GenerateBasicTrialFromConfig(BlockId blockId, TrialId trialId,
            Data.Trial trialDataFromIndex)
        {
            // Control flow here i                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                s for deciding what Trial gets spat out from the config
            AbstractTrial currTrial;
            if (trialDataFromIndex.FileLocation != null)
            {
                Debug.Log("Creating new Instructional Trial");
                currTrial = new InstructionalTrial(_data, blockId, trialId);
            }
            else if (trialDataFromIndex.TwoDimensional == 1)
            {
                Debug.Log("Creating new 2D Screen Trial");
                currTrial = new TwoDTrial(_data, blockId, trialId);
            }
            else
            {
                Debug.Log("Creating new 3D Screen Trial");
                currTrial = new ThreeDTrial(_data, blockId, trialId);
            }
            
            return currTrial;
        }
    }
}