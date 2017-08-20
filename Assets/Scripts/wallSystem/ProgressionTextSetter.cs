using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;
using E = systems.ExperimentManager;

namespace wallSystem
{
	public class ProgressionTextSetter : MonoBehaviour {
	
		// Use this for initialization
		private void Start ()
		{
			if (E.Get().St == E.State.WelcomeScreen)
			{
				GetComponent<Text>().text = DS.GetInstructions().Instructions;
			
			}
			else if (E.Get().St == E.State.Waiting)
			{
			
				if (E.Get().TrialIndex == 0)
				{
					GetComponent<Text>().text = DS.GetInstructions().First;
				}
				else
				{
					GetComponent<Text>().text = DS.GetInstructions().WinMessage;
				}
				E.Get().CatchEvent(E.State.InLoadingScreen);
			}
			else //Should be only when done
			{
				GetComponent<Text>().text = DS.GetInstructions().EndMessage;

			}
		}
	
	}
}
