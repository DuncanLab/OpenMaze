using data;
using main;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Purchasing.Extension;
using DS = data.DataSingleton;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {



        public RandomTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            AbstractTrial currentTrial = this;
            var trueNext = next;
            int i = BlockID;
            var block = DS.GetData().BlockList[i];
            int n = block.RandomTrialType.Count;
            int randomSelection = Random.Range(0, n);

            var d = block.RandomTrialType[randomSelection];

            if (block.Replacement == 0)
            {
                block.RandomTrialType.Remove(d);
            }
            
            int tCnt = 0;
            foreach (int j in d.RandomGroup)
            {
                //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                //here we have a picture as a trial.
                var trialData = DS.GetData().TrialData[j];
                
                //Control flow here is for deciding what Trial gets spat out from the config

                AbstractTrial t;
                if (trialData.FileLocation != null)
                {
                    Debug.Log("Creating new Loading Screen Trial");
                    t = new LoadingScreenTrial(i, j);
                }
                else if (trialData.TwoDimensional == 1)
                {
                    Debug.Log("Creating new 2D Screen Trial");

                    t = new TwoDTrial(i, j);
                }
                else
                {
                    Debug.Log("Creating new 3D Screen Trial");

                    t = new ThreeDTrial(i, j);
                }

                
                
                t.isTail = tCnt == d.RandomGroup.Count - 1 && isTail;
                t.head = head;
                
                currentTrial.next = t;
                
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