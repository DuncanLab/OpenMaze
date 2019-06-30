using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using data;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;
namespace trial
{
    //A central trial class 
    public abstract class AbstractTrial
    {
        //These two fields register the current block and trial ID in the dataSingleton
        public int BlockID;

        public int TrialID;

        public bool isSuccessful;

        public int NumCollected; // The number of goals collected for this trial

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

            isSuccessful = false;

            if (blockId == -1 || trialId == -1) return;
            if (DataSingleton.GetData().BlockList.Count == 0) throw new Exception("No trial in block");

            Value = DataSingleton.GetData().TrialData[trialId];
        }

        public virtual void PreEntry(TrialProgress t, bool first = true)
        {
            //Prentry into the next trial
            Debug.Log("Entering trial: " + TrialID);
            if (head == this && first)
            {
                Debug.Log(string.Format("Entered Block: {0} at time: {1}", BlockID, DateTime.Now));
                t.ResetOngoing();
                t.successes = new List<int>();
                int NumBlocks = DS.GetData().BlockList.Count;
                t.NumCollectedPerBlock = new int[NumBlocks];
            }
            _runningTime = 0;

            t.TrialNumber++;

            Debug.Log("Current Trial Increment: " + data.DataSingleton.GetData().TrialInitialValue);

            // increment the trial sequence value
            data.DataSingleton.GetData().TrialInitialValue++;

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

                    var result = func != null && (bool)func.Invoke(null, new object[] { TrialProgress });

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

        protected void LoadNextSceneWithTimer(int environmentType)
        {
            Loader.Get().StartCoroutine(LoadNextAsyncScene(environmentType));
        }

        private IEnumerator LoadNextAsyncScene(int environmentType)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(environmentType);

            // Wait until the specified timeout to load the scene
            var timer = 0.0f;
            var delayAmount = 5.0f;

            while (timer < delayAmount)
            {
                timer += Time.deltaTime;
                op.allowSceneActivation = false;
                yield return null;
            }

            op.allowSceneActivation = true;
            yield return null;
        }
    }
}