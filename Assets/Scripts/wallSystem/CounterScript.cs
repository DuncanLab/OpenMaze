using main;
using UnityEngine;
using UnityEngine.UI;

namespace wallSystem
{
	public class CounterScript : MonoBehaviour {


		public void Start()
		{
			var text = GameObject.Find("CountDown").GetComponent<Text>();
			text.color = Color.white;
			if (Loader.Get().CurrTrial.Value.PickupType < 0)
				text.text = "Endless Mode";
			else if (Loader.Get().CurrTrial.Value.PickupType > 0)
			{
				text.text = "Found: " + BlockState.GetNumberItemsFound();
			}
		}
	
	}
}
