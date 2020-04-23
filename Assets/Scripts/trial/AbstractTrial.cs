using System;
using System.Collections;
using System.Collections.Generic;
using contingency;
using data;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;
using value;
using static data.Data;
using DS = data.DataSingleton;

namespace trial
{
    // A central trial class 
    public abstract class AbstractTrial
    {

        protected Data _data;
        // Variable only used for easy manipulation of AbstractTrials.        
        public static readonly AbstractTrial TempHead = new CloseTrial();

        // These two fields register the current block and trial ID in the dataSin1gleton
        public readonly BlockId BlockId;

        public readonly TrialId TrialId;

        // The index of the node in the contingencyGraph which represents the current trial.
        private IContingencyService _contingencyService;

        protected float _runningTime;

        public Enclosure enclosure;

        // This points to the start of the block of trials (if present)
        public AbstractTrial head;
        public AbstractTrial SourceTrial;
        
        public bool isSuccessful;

        public bool isTail;

        // This points to the next trial
        // ReSharper disable once InconsistentNaming
        public AbstractTrial next;

        public AbstractTrial Prev;

        public int NumCollected; // The number of goals collected for this trial

        public bool progressionComplete;
        
        public Trial trialData;

        public TrialProgress TrialProgress;
        public long TrialStartTime;

        protected AbstractTrial(Data data, BlockId blockId, TrialId trialId)
        {
            var factory = ContingencyService.ContingencyServiceFactory.Create();
            _data = data;
            BlockId = blockId;
            TrialId = trialId;

            isSuccessful = false;

            if (Equals(blockId, BlockId.EMPTY)) return;
            if (data.Blocks.Count == 0) throw new Exception("No trial in block");

            trialData = data.Trials[trialId.Value];

            if (trialData.Enclosure > 0)
                enclosure = data.Enclosures[trialData.Enclosure - 1];

            // If the user hasn't set an Enclosure index we want to set the Enclosure to be
            // unobtrusive, So the ground generates but nothing else.
            else
                enclosure = new Enclosure
                {
                    WallHeight = 0,
                    WallColor = "1B5E20",
                    Sides = 4,
                    GroundTileSides = 0,
                    GroundTileSize = 0,
                    GroundColor = null,
                    Radius = 4,
                    Position = new List<float> {0, 0}
                };
        }

        public bool IsGenerated { get; set; }

        public void SetContingency(IContingencyService contingencyService)
        {
            _contingencyService = contingencyService;
        }
        
        public virtual void PreEntry(TrialProgress t, bool first = true)
        {
            Debug.Log("Entering trial: " + TrialId);
            if (head == this && first)
            {
                Debug.Log($"Entered Block: {BlockId.Value} at time: {DateTime.Now}");
                t.ResetOngoing();
                t.successes = new List<int>();
                var NumBlocks = DS.GetData().Blocks.Count;
                t.NumCollectedPerBlock = new int[NumBlocks];
            }

            Debug.Log("Current Trial Increment: " + DS.GetData().TrialInitialValue);

            if (t.TrialNumber < 2) t.TimeSinceExperimentStart = 0.0f;

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
            if (TrialProgress.TrialNumber < 2) TrialProgress.TimeSinceExperimentStart = 0.0f;
            _runningTime = 0;
        }

        // Function for stuff to know that things have happened
        public virtual void Notify()
        {
        }

        public override string ToString()
        {
            return $"Block {BlockId}, Trial {TrialId}";
        }

        // Load the next trial
        public virtual void Progress()
        {
            Debug.Log("Progressing...");
            // Exiting current trial
            TrialProgress.PreviousTrial = this;

            var nextTrial = _contingencyService.ExecuteContingency(TrialProgress);

            Loader.Get().CurrTrial = nextTrial;
            next.PreEntry(TrialProgress);
            progressionComplete = true;
        }

        protected void LoadNextSceneWithTimer(int environmentType)
        {
            Loader.Get().StartCoroutine(LoadNextAsyncScene(environmentType));
        }

        private IEnumerator LoadNextAsyncScene(int environmentType)
        {
            var ao = SceneManager.LoadSceneAsync(environmentType);
            TrialProgress.isLoaded = false;

            // Wait until the specified timeout to load the scene
            var timer = 0.0f;

            while (!ao.isDone && !progressionComplete)
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