using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DS = DataSingleton;
using L = Loader;

public class ProgressionTextSetter : MonoBehaviour {

	// Use this for initialization
	private void Start () {
		if (L.experimentMode) {
			Data.PickupItem p = DS.GetData ().PickupItems [L.experiment [L.experimentIndex] [2]];


			Text gText = GetComponent<Text> ();

			switch (L.experimentEndSrc)
			{
				case L.ExperimentEndSrc.External:
					gText.text = "Nice job you found it!\n";
					break;
				case L.ExperimentEndSrc.Internal:
					gText.text = "Sorry, you ran out of time\n";
					break;
				default:
					gText.text = "";
					break;
			}


			gText.text += "Looking for: " + p.Tag;


			gText.color = Data.GetColour (p.Color);

		} else {
			
			if (L.ep == L.ExperimentProgression.Ended) {
				GetComponent<Text> ().text = "Well done, the experiment has ended!"; 
					
			} else {
				GetComponent<Text> ().text = 
					"Welcome to the experiment participant\n" +
					"Press <N> to begin";
				
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
