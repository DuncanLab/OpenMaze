using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public long TrialStartTime;
        
        // Variable only used for easy manipulation of AbstractTrials.        
        public static readonly AbstractTrial TempHead = new CloseTrial();
        
        private List<ContingencyNode> _contingencyGraph;
        // The index of the node in the contingencyGraph which represents the current trial.
        private int _contingencyIndex;
        
        // These two fields register the current block and trial ID in the dataSin1gleton
        public readonly BlockId BlockId;

        public readonly TrialId TrialId;
        
        public bool IsGenerated { get; set; }
        
        public bool isSuccessful;

        public int NumCollected; // The number of goals collected for this trial

        public TrialProgress TrialProgress;

        // This points to the start of the block of trials (if present)
        public AbstractTrial head;

        public bool isTail;

        public Trial trialData;

        public Enclosure enclosure;

        // This points to the next trial
        // ReSharper disable once InconsistentNaming
        public AbstractTrial next;

        protected float _runningTime;

        public bool progressionComplete = false;

        protected AbstractTrial(BlockId blockId, TrialId trialId)
        {
            BlockId = blockId;
            TrialId = trialId;

            isSuccessful = false;

            if (blockId == BlockId.EMPTY) return;
            if (DataSingleton.GetData().Blocks.Count == 0) throw new Exception("No trial in block");

            trialData = DataSingleton.GetData().Trials[trialId.Value];

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
            Debug.Log("Entering trial: " + TrialId);
            if (head == this && first)
            {
                Debug.Log(string.Format("Entered Block: {0} at time: {1}", BlockId.Value, DateTime.Now));
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

        private bool ComputeEndFunction()
        {
            var blockData = DS.GetData().Blocks[BlockId.Value];

            // Data on how to choose the next trial will be selected here.
            if (!isTail || blockData.EndFunction == null) return false;
            
            Debug.Log("Computing EndFunction");

            
            var tmp = blockData.EndFunction;
            var func = typeof(ExitFunctions).GetMethod(tmp, BindingFlags.Static | BindingFlags.Public);

            var result = func != null && (bool) func.Invoke(null, new object[] {TrialProgress});

            if (!result) return false;
            
            Debug.Log($"Loop current block since {blockData.ExitFunction} returned true");
            
            Loader.Get().CurrTrial = head;
            head.PreEntry(TrialProgress, false);
            return true;

        }
        private bool ComputeExitFunction()
        {
            var blockData = DS.GetData().Blocks[BlockId.Value];
            if (blockData.ExitFunction == null) return false;
            var exitFunction = blockData.ExitFunction;
            var func =
                typeof(ExitFunctions).GetMethod(exitFunction, BindingFlags.Static | BindingFlags.Public);
            var result = func != null && (bool) func.Invoke(null, new object[] {TrialProgress});
            if (result) return false;
            
            Debug.Log($"Exiting current block since {blockData.ExitFunction} returned false");

            var tmp = next;
            
            while (!tmp.isTail)
            {
                tmp = tmp.next;
            }

            Loader.Get().CurrTrial = tmp.next;
            next.PreEntry(TrialProgress);
            return true;
        }

        private bool HasContingency()
        {
            return _contingencyGraph != null;
        }

        private string InvokeContingencyFunction(string contingencyFunction)
        {
            var func =
                typeof(ContingencyFunctions).GetMethod(contingencyFunction, BindingFlags.Static | BindingFlags.Public);
            
            if (func == null)
            {
                Debug.LogError($"Contingency Function {contingencyFunction} doesn't exist.");
                Application.Quit();
                return null;
            }
            
            var result = (string) func.Invoke(null, new object[] {TrialProgress});
            Debug.Log($"Output from the Contingency Function is {result}");
            return result;
        }

        private ContingencyData GetContingencyDataFromResult(string contingencyResult)
        {
            var contingencyData = _contingencyGraph[_contingencyIndex];
            
            var path = contingencyData.TrialIndicesByOutput[contingencyResult];
            
            if (path == null)
            {
                Debug.LogError($"Result {contingencyResult} from contingencyFunction " +
                               $"${contingencyData.ContingencyFunction} has no associated trials");
                Application.Quit();
                throw new Exception();
            }

            return path;
        }
        
        private void HandleContingency()
        {
            
            // The first trial in the contingency graph won't be generated.
            // We remove the generated trials in case we had a loop in the block
            if (!IsGenerated)
            {
                while (next.IsGenerated)
                {
                    next = next.next;
                }
            }
            var contingencyNode = _contingencyGraph[_contingencyIndex];


            var contingencyResult = InvokeContingencyFunction(contingencyNode.ContingencyFunction);

            var contingencyData = GetContingencyDataFromResult(contingencyResult);
            
            var nextTrials = new List<int>(contingencyData.Trials);
            
            var contingencyNodeId = new ContingencyNodeId(contingencyData.NextNodeIndex);
            
            if (contingencyNodeId.Value >= 0)
            { 
                nextTrials.Add(_contingencyGraph[contingencyNodeId.Value].InitialTrial);
            }

            var nextTrialIds = from id in nextTrials select new TrialId(id);
            var tmp = next;
            var curr = TempHead;
            foreach (var nextTrialId in nextTrialIds)
            {
                curr.next = TrialUtils.GenerateBasicTrialFromConfig(BlockId, nextTrialId,
                    DS.GetData().Trials[nextTrialId.Value]);
                curr.next.IsGenerated = true;
                curr = curr.next;
            }

            if (contingencyNodeId.Value >= 0)
            {
                curr.SetContingency(_contingencyGraph, contingencyNodeId.Value);
            }
            
            
            
            curr.next = tmp;
            next = TempHead.next;
        }
        
        public void SetContingency(List<ContingencyNode> contingencyGraph, int contingencyIndex)
        {
            _contingencyGraph = contingencyGraph;
            _contingencyIndex = contingencyIndex;
        }
        // Load the next trial
        public virtual void Progress()
        {
            Debug.Log("Progressing...");

            if (HasContingency())
            {
                Debug.Log("Progressing with contingency");
                HandleContingency();
            }
            
            // Exiting current trial
            TrialProgress.PreviousTrial = this;
            if (TrialProgress.Instructional != 1)
            {
                // We exit the current trial.
                if (ComputeEndFunction() || ComputeExitFunction())
                {
                    return;
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