using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DS = DataSingleton;
using L = Loader;


public class ProgressionTextSetter : MonoBehaviour {
	
	// Use this for initialization
	private void Start ()
	{
	
		
		
		if (L.ExperimentMode) {
			Data.PickupItem p = DS.GetData ().PickupItems [L.ExperimentCsv [L.ExperimentIndex] [2]];


			Text gText = GetComponent<Text> ();

			switch (L.EndSrc)
			{
				case L.ExperimentEndSrc.External:
					gText.text = L.InstructionData.WinMessage.Replace("%", p.Tag);
					break;
				case L.ExperimentEndSrc.Internal:
					gText.text = L.InstructionData.LoseMessage.Replace("%", p.Tag);
					break;
				case Loader.ExperimentEndSrc.Never:
					gText.text = L.InstructionData.First.Replace("%", p.Tag);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}




			gText.color = Data.GetColour (p.Color);

		} else {
			
			if (L.Ep == L.ExperimentProgression.Ended) {
				GetComponent<Text> ().text = L.InstructionData.EndMessage; 
					
			} else
			{
				GetComponent<Text>().text = L.InstructionData.Instructions;
			}
		}
	}
	
}
