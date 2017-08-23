using System;
using states;
using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = main.Loader;
using C = data.Constants;
namespace main
{
	public class ProgressionTextSetter : MonoBehaviour {
		
		// TODOUse this for initialization
		private void Start ()
		{
			
			if (E.Get().CurrentState == State.Start)
			{
				GetComponent<Text>().text = DS.GetInstructions().Instructions;
				
			} else if (E.Get().CurrentState == State.WaitFirst)
			{		
				GetComponent<Text>().text = DS.GetInstructions().First.Replace("%", ComputePickupName());
				Progress();
			} else if (E.Get().CurrentState == State.Won)
			{
				GetComponent<Text>().text = DS.GetInstructions().WinMessage.Replace("%", ComputePickupName());
				Progress();
			} else if (E.Get().CurrentState == State.Lost)
			{
				GetComponent<Text>().text = DS.GetInstructions().LoseMessage.Replace("%", ComputePickupName());
				Progress();
			} else if (E.Get().CurrentState == State.Timeout)
			{
				GetComponent<Text>().text = DS.GetInstructions().EndlessMessage.Replace("%", ComputePickupName());
				Progress();
			}
			else if (E.Get().CurrentState == State.Final)
			{
				GetComponent<Text>().text = DS.GetInstructions().EndMessage;
			} else if (E.Get().CurrentState == State.TwoDim)
			{
				GetComponent<Text>().text = DS.GetInstructions().TwoDMode;
				E.Get().CatchEvent(new TransitionEvent(C.Jungle2D, State.TwoDim));
			}
			E.Get().RunningTime = 0;
		}

		private static void Progress()
		{
			int envType = Loader.Get().CurrTrial.EnvironmentType + 2;
			int waitTime = Loader.Get().CurrTrial.PrewaitTime;
				 
			E.Get().CatchEvent(new TransitionEvent(envType, State.Trial, waitTime));
		}
		
		private static string ComputePickupName()
		{
			int trialIndex = Math.Abs(E.Get().CurrTrial.PickupType);
			return trialIndex == 0 ? null : DS.GetData().PickupItems[trialIndex - 1].Tag;
		}
	
	}
}
