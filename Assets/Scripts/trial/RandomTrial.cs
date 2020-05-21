﻿using System.Collections.Generic;
using contingency;
using data;
using main;
using UnityEngine;
using value;
using DS = data.DataSingleton;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {
        private readonly IContingencyServiceFactory _contingencyServiceFactory;
        private readonly ITrialService _trialService;
        private AbstractTrial _generatedTrialHead;
        private readonly List<Data.RandomData> _randomSelection;
        private readonly bool _replacement;

        public RandomTrial(Data data, ITrialService trialService, IContingencyServiceFactory serviceFactory,
            BlockId blockId) : base(data, blockId, TrialId.EMPTY)
        {
            _trialService = trialService;
            _contingencyServiceFactory = serviceFactory;
            var block = data.Blocks[BlockId.Value];
            _randomSelection = block.RandomlySelect;
            _replacement = block.Replacement == 0;
        }

        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            Debug.Log("GenerateTrial");
            var curr = TempHead;
            var numRandomTrials = _randomSelection.Count;
            var randomSelection = Random.Range(0, numRandomTrials);

            var randomTrialIndices = _randomSelection[randomSelection];
            
            if (!_replacement) _randomSelection.Remove(randomTrialIndices);
            Debug.Log("RANDOM TRIAL CREATION");


            var tCnt = 0;
            foreach (var trialDisplayIndex in randomTrialIndices.Order)
            {
                var trialId = new TrialId(trialDisplayIndex);
                //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                //here we have a picture as a trial.
                var targetTrialData = _data.Trials[trialId.Value];

                //Control flow here is for deciding what Trial gets spat out from the config

                var targetTrial = _trialService.GenerateBasicTrialFromConfig(BlockId, trialId, targetTrialData);
                targetTrial.IsHead = tCnt == 0 && IsHead;
                targetTrial.head = curr;
                targetTrial.IsGenerated = true;
                targetTrial.SetContingency(_contingencyServiceFactory.CreateEmpty(targetTrial));
                curr.next = targetTrial;

                curr = curr.next;
                tCnt++;
            }

            curr.next = next;
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            GenerateTrials();
            Progress();
        }


        public override void Progress()
        {
            Loader.Get().CurrTrial = TempHead.next;
            TempHead.next.PreEntry(TrialProgress);
        }
    }
}
