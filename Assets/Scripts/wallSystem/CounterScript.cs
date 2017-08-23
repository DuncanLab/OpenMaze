using main;
using UnityEngine;
using UnityEngine.UI;

namespace wallSystem
{
	public class CounterScript : MonoBehaviour {

		// Use this for initialization
		private void Start ()
		{
			Text text = GameObject.Find("CountDown").GetComponent<Text>();
			text.color = Color.white;
			if (Loader.Get().CurrTrial.PickupType < 0)
				text.text = "Endless Mode";
			else if (Loader.Get().CurrTrial.PickupType > 0)
			{
				text.text = "Found: " + PlayerController.ObjectsFound;
			}
		}
	
	}
}
