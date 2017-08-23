using System.IO;
using data;
using states;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;
using C = data.Constants;
using Event = states.Event;

namespace main
{

	/// <summary>
	/// Main entry point of the app as well as the game object that stays alive for all scenes.
	/// </summary>
	public class Loader : MonoBehaviour
	{
		public static Loader Get()
		{
			return GameObject.Find("Loader").GetComponent<Loader>();
		}
		
		
		public Data.Trial CurrTrial;
		public float RunningTime;
		public Event CurrentEvent;
		public State CurrentState;
		
		private void Start () {
			DontDestroyOnLoad(this);
			DS.Load ();
			CurrTrial = DS.GetData().TrialData[0];

			CatchEvent(new TransitionEvent(C.LoadingScreen, State.Start));
			Directory.CreateDirectory(C.OutputDirectory);
		}


		public void CatchEvent(Event e)
		{

			RunningTime = 0;

			if (e.State != State.None) CurrentState = e.State;
			
			CurrentEvent = e;
		}
		
		private void Update()
		{
			if (CurrentEvent != null) CurrentEvent.Act();
		
			HandleInput();
			
			RunningTime += Time.deltaTime;

			if (CurrentState == State.Trial && RunningTime > CurrTrial.TimeAllotted)
			{
				Progress();

				
				if (CurrTrial.PickupType > 0)
				{
					CurrentState = State.Lost;
				}
				else if (CurrTrial.PickupType == 0)
				{
					CurrentState = State.TwoDim;
				}
				else
				{
					CurrentState = State.Timeout;
				}
				
				
				SceneManager.LoadScene(C.LoadingScreen);
			}
			
		}

		private void HandleInput()
		{
			if (Input.GetKey(KeyCode.Space))
			{
				if (CurrentState == State.Start)
				{
					CatchEvent(new TransitionEvent(C.LoadingScreen, State.WaitFirst));
				}
			}
		}

		public void ResetEvent()
		{
			CurrentEvent = null;
		}

		public bool Progress()
		{
			if (CurrTrial.Index == DS.GetData().TrialData.Count)
			{
				CurrentState = State.Final;
				SceneManager.LoadScene(C.LoadingScreen);
				return false;
			}
			
			CurrTrial = DS.GetData().TrialData[CurrTrial.Index];
			return true;
		}
	}
}
