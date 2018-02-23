using System;
using System.Reflection;
using data;
using main;
using NUnit.Framework;
using UnityEngine;
using DS = data.DataSingleton;
namespace trial
{
    //A central trial class 
    public abstract class AbstractTrial
    {
        //These two fields register the current block and trial ID in the dataSingleton
        public int BlockID;

        public int TrialID;

        public TrialProgress TrialProgress;

        //This points to the start of the block of trials (if present)
        public AbstractTrial head;

        public bool isTail;
        
        public Data.Trial Value;
        
        //This points to the next trial
        // ReSharper disable once InconsistentNaming
        public AbstractTrial next;

        protected float _runningTime;

        
        
        
        protected AbstractTrial(int blockId, int trialId)
        {
            BlockID = blockId;
            TrialID = trialId;
            
            if (DataSingleton.GetData().BlockList.Count == 0) throw new Exception("No trial in block");
            
            if (!(blockId == -1 || trialId == -1))
                Value = DataSingleton.GetData().TrialData[trialId];
    
        }




        public virtual void PreEntry(TrialProgress t, bool first = true)
        {
            //Prentry into the next trial
            Debug.Log("Entering trial: " + TrialID);
            if (head == this && first)
            {
                Debug.Log(string.Format("Entered Block: {0} at time: {1}", BlockID,  DateTime.Now));
                t.ResetOngoing();
            }
            _runningTime = 0;
            
            t.TrialNumber++;            
            
            TrialProgress = t;
        }

        public virtual void Update(float deltaTime)
        {
            _runningTime += deltaTime;
        }

        public void ResetTime()
        {
            _runningTime = 0;
        }
        
        //Function for stuff to know that things have happened
        public virtual void Notify()
        {
            
        }
        
        
        //Essentially, here we load
        public virtual void Progress()
        {
            
            Debug.Log("Progressing...");
            //Exiting current trial
            TrialProgress.PreviousTrial = this; 
            

            var blockData = DS.GetData().BlockList[BlockID];
            //Data on how to choose the next trial will be selected here.
            if (isTail)
            {
                if (blockData.EndFunction != null)
                {    
                    
                    var tmp = blockData.EndFunction;    
                    var func = typeof(Functions).GetMethod(tmp, BindingFlags.Static | BindingFlags.Public);
                    
                    
                    var result = func != null && (bool) func.Invoke(null, new object[] {TrialProgress});
                    
                    if (result)
                    {
                        Loader.Get().CurrTrial = head;
                        head.PreEntry(TrialProgress, false);
                        return;
                    }
                }
                
            } 
            Loader.Get().CurrTrial = next;
            next.PreEntry(TrialProgress);
            
            
        }

        public float GetRunningTime()
        {
            return _runningTime;
        }
    }
}