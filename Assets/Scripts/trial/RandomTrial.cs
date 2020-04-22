using data;
using main;
using UnityEngine;
using value;
using DS = data.DataSingleton;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {
        private readonly ITrialService _trialService;

        public RandomTrial(Data data, ITrialService trialService, BlockId blockId) : base(data, blockId, TrialId.EMPTY)
        {
            _trialService = trialService;
        }

        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            Debug.Log("GenerateTrial");
            AbstractTrial currentTrial = this;
            var block = DS.GetData().Blocks[BlockId.Value];
            var numRandomTrials = block.RandomlySelect.Count;
            var randomSelection = Random.Range(0, numRandomTrials);

            var randomTrialIndices = block.RandomlySelect[randomSelection];
            while (next.IsGenerated) next = next.next;
            if (block.Replacement == 0) block.RandomlySelect.Remove(randomTrialIndices);
            Debug.Log("RANDOM TRIAL CREATION");

            var trueNext = next;

            var tCnt = 0;
            foreach (var trialDisplayIndex in randomTrialIndices.Order)
            {
                var trialId = new TrialId(trialDisplayIndex);
                //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                //here we have a picture as a trial.
                var targetTrialData = _data.Trials[trialId.Value];

                //Control flow here is for deciding what Trial gets spat out from the config

                var targetTrial = _trialService.GenerateBasicTrialFromConfig(BlockId, trialId, targetTrialData);
                targetTrial.isTail = tCnt == randomTrialIndices.Order.Count - 1 && isTail;
                targetTrial.head = head;
                targetTrial.IsGenerated = true;

                currentTrial.next = targetTrial;

                currentTrial = currentTrial.next;
                tCnt++;
            }

            currentTrial.next = trueNext;
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            GenerateTrials();
            Progress();
        }


        public override void Progress()
        {
            Loader.Get().CurrTrial = next;
            next.PreEntry(TrialProgress);
        }
    }
}