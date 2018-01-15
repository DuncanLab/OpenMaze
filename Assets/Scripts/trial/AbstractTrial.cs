using System;
using System.Reflection;
using data;
using main;
using UnityEngine;
using DS = data.DataSingleton;
namespace trial
{
    //A central trial class 
    public abstract class AbstractTrial
    {
        //These two fields register the current block and trial ID in the dataSingleton
        protected int BlockID, TrialID;
        
        protected TrialProgress TrialProgress;

        //This points to the start of the block of trials (if present)
        protected AbstractTrial head;

        protected bool isTail;
        
        public Data.Trial Value;
        
        //This points to the next trial
        protected AbstractTrial next;

        protected float _runningTime;


        protected AbstractTrial(int blockId, int trialId)
        {
            BlockID = blockId;
            TrialID = trialId;
            
            if (DataSingleton.GetData().BlockList.Count == 0) throw new Exception("No trial in block");
            Value = DataSingleton.GetData().TrialData[trialId];
            _runningTime = 0;

        }



        public abstract void LogData(Transform t, bool collided = false);

        public virtual void PreEntry(TrialProgress t)
        {
            ResetTimer();
        }

        public virtual void Update(float deltaTime)
        {
            _runningTime += deltaTime;
        }

        public void ResetTimer()
        {
            _runningTime = 0;
        }

        public float GetTimer()
        {
            return _runningTime;
        }
        
        
        //Essentially, here we load
        public virtual void Progress()
        {

            var blockData = DS.GetData().BlockList[BlockID];
            //Data on how to choose the next trial will be selected here.
            if (isTail)
            {
                if (blockData.EndFunction != null)
                {
                    
                    var tmp = blockData.EndFunction;
                    var func = typeof(Functions).GetMethod(tmp, BindingFlags.Static | BindingFlags.Public);
                    var result = (bool) func.Invoke(null, new object[] {blockData, TrialProgress});
                    if (result)
                    {
                        head.PreEntry(TrialProgress);
                        Loader.Get().CurrTrial = head;
                        return;
                    }
                }
                
            } 
            
            next.PreEntry(TrialProgress); //We don't have any data on the current trail from the loading screen   
            Loader.Get().CurrTrial = next;
            
            
        }
        
    }
}