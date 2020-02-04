using data;
using main;
using UnityEngine;
using DS = data.DataSingleton;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {

        private static int _numGenerated;

        public RandomTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            Debug.Log("GenerateTrial");
            AbstractTrial currentTrial = this;
            var i = BlockID;
            var block = DS.GetData().Blocks[i];
            var n = block.RandomlySelect.Count;
            var randomSelection = Random.Range(0, n);

            var d = block.RandomlySelect[randomSelection];

            if (block.Replacement == 0)
            {
                block.RandomlySelect.Remove(d);
            }
            Debug.Log("RANDOM TRIAL CREATION");
            
            
            //Need this to remove the previously generated random trials.
            while (_numGenerated > 0)
            {
                //next = next.next;

                _numGenerated--;
            }
            
            var trueNext = next;
            
            var tCnt = 0;
            foreach (var j in d.Order)
            {
                //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                //here we have a picture as a trial.
                var trialData = DS.GetData().Trials[j-1];
                
                //Control flow here is for deciding what Trial gets spat out from the config

                AbstractTrial t;
                if (trialData.FileLocation != null)
                {
                    Debug.Log("Creating new Loading Screen Trial");
                    t = new InstructionalTrial(i, j-1);
                }
                else if (trialData.TwoDimensional == 1)
                {
                    Debug.Log("Creating new 2D Screen Trial");

                    t = new TwoDTrial(i, j-1);
                }
                else
                {
                    Debug.Log("Creating new 3D Screen Trial");

                    t = new ThreeDTrial(i, j-1);
                }

                
                
                t.isTail = tCnt == d.Order.Count - 1 && isTail;
                t.head = head;
                
                currentTrial.next = t;
                
                currentTrial = currentTrial.next;
                _numGenerated++;
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