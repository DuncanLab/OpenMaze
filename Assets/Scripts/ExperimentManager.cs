using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = DataSingleton;
using C = Constants;

/// <summary>
/// This class is the central experiment 
/// </summary>
public class ExperimentManager
{
    private AsyncOperation op;
    public enum State
    {
        Null,
        PlayingSound,
        Waiting,
        WelcomeScreen,
        SpaceDown,
        DonePlaying,
        Moving,
        Done,
        InLoadingScreen
    }

    public int TrialIndex;
    public Data.Trial CurrTrial;
    
    public float RunningTime;
    public State St;
    
    
    //Singleton instance for Experiment Manager
    private static ExperimentManager _myInstance;

    /// <summary>
    /// Singleton Get function
    /// </summary>
    /// <returns>Singleton instance</returns> 
    public static ExperimentManager Get()
    {
        return _myInstance;
    }

    public static void Init()
    {
        _myInstance = new ExperimentManager();
    }
    
    private ExperimentManager()
    {
        Directory.CreateDirectory ("Assets\\OutputFiles~");
        St = State.WelcomeScreen;
        TrialIndex = 0;
    }

    
    public void Update(float deltaTime)
    {
        RunningTime += deltaTime;
        if (RunningTime > CurrTrial.PrewaitTime)
        {

        }
    }
       
    public void CatchEvent(State s)
    {
 
        if (s == State.SpaceDown)
        {
            if (St == State.WelcomeScreen)
            {
                CurrTrial = DS.GetData().TrialData[TrialIndex++];
                _toLoading(State.Waiting);
            }
        } 
        else if (s == State.InLoadingScreen)
        {
            _loadNext();
        }
        else if (s == State.DonePlaying)
        {
            if (CurrTrial.PickupType < 0)
            {
                if (RunningTime <= CurrTrial.TimeAllotted)
                {
                    GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>().ResetCreate();
                    return;
                }
            }
            if (TrialIndex < DS.GetData().TrialOrder.Count)
            {
                CurrTrial = DS.GetData().TrialData[TrialIndex++];
                _toLoading(State.Waiting);
            }
            else
            {
                _toLoading(State.Done);
            }
        }
        else
        {
            St = s;
        }
    }

    private void _toLoading(State st)
    {
        St = st;
        
        SceneManager.LoadScene(C.LoadingScreen);
    }
    
    private void _loadNext()
    {
        if (CurrTrial.EnvironmentType == 0)
            SceneManager.LoadScene(C.CityTerrain);
        else
            SceneManager.LoadScene(C.JungleTerrain);
        RunningTime = 0;
    }
   

    

}
