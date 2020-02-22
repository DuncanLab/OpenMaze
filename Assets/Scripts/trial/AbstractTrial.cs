using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using data;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;
using static data.Data;
using DS = data.DataSingleton;

namespace trial
{
    // A central trial class 
    public abstract class AbstractTrial
    {
        public long TrialStartTime;

        // These two fields register the current block and trial ID in the dataSingleton
        public int BlockID;

        public int TrialID;

        public bool isSuccessful;

        public int NumCollected; // The number of goals collected for this trial

        public TrialProgress TrialProgress;

        // This points to the start of the block of trials (if present)
        public AbstractTrial head;

        public bool isTail;

        public Data.Trial trialData;

        public Enclosure enclosure;

        // This points to the next trial
        // ReSharper disable once InconsistentNaming
        public AbstractTrial next;

        protected float _runningTime;

        public bool progressionComplete = false;

        protected AbstractTrial(int blockId, int trialId)
        {
            BlockID = blockId;
            TrialID = trialId;

            isSuccessful = false;

            if (blockId == -1 || trialId == -1) return;
            if (DataSingleton.GetData().Blocks.Count == 0) throw new Exception("No trial in block");

            trialData = DataSingleton.GetData().Trials[trialId];

            if (trialData.Enclosure > 0)
            {
                enclosure = DataSingleton.GetData().Enclosures[trialData.Enclosure - 1];
            }

            // If the user hasn't set an Enclosure index we want to set the Enclosure to be
            // unobtrusive, So the ground generates but nothing else.
            else
            {
                enclosure = new Enclosure
                {
                    WallHeight = 0,
                    WallColor = "1B5E20",
                    Sides = 4,
                    GroundTileSides = 0,
                    GroundTileSize = 0,
                    GroundColor = null,
                    Radius = 4,
                    Position = new List<float> { 0, 0 }
                };
            }
        }

        public virtual void PreEntry(TrialProgress t, bool first = true)
        {
            Debug.Log("Entering trial: " + TrialID);
            if (head == this && first)
            {
                Debug.Log(string.Format("Entered Block: {0} at time: {1}", BlockID, DateTime.Now));
                t.ResetOngoing();
                t.successes = new List<int>();
                int NumBlocks = DS.GetData().Blocks.Count;
                t.NumCollectedPerBlock = new int[NumBlocks];
            }

            Debug.Log("Current Trial Increment: " + data.DataSingleton.GetData().TrialInitialValue);

            if (t.TrialNumber < 2)
            {
                t.TimeSinceExperimentStart = 0.0f;
            }

            TrialStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            _runningTime = 0;
            TrialProgress = t;
        }

        public virtual void Update(float deltaTime)
        {
            TrialProgress.TimeSinceExperimentStart += deltaTime;
            _runningTime += deltaTime;
        }

        public void ResetTime()
        {
            if (TrialProgress.TrialNumber < 2)
            {
                TrialProgress.TimeSinceExperimentStart = 0.0f;
            }
            _runningTime = 0;
        }

        // Function for stuff to know that things have happened
        public virtual void Notify()
        {

        }

        // Load the next trial
        public virtual void Progress()
        {
            Debug.Log("Progressing...");

            // Exiting current trial
            TrialProgress.PreviousTrial = this;

            var blockData = DS.GetData().Blocks[BlockID];

            // Data on how to choose the next trial will be selected here.
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
            progressionComplete = true;
        }

        protected void LoadNextSceneWithTimer(int environmentType)
        {
            Loader.Get().StartCoroutine(LoadNextAsyncScene(environmentType));
        }

        private IEnumerator LoadNextAsyncScene(int environmentType)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(environmentType);
            TrialProgress.isLoaded = false;

            // Wait until the specified timeout to load the scene
            var timer = 0.0f;

            while (!ao.isDone && (!progressionComplete))
            {
                timer += Time.deltaTime;
                yield return null;
            }

            // Reset when loading is complete
            timer = 0.0f;
            progressionComplete = false;
        }
    }
}