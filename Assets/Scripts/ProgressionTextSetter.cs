using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DS = DataSingleton;
using L = Loader;

public class ProgressionTextSetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (L.experimentMode) {
			string text = DS.GetData ().PickupItems [L.experiment [L.experimentIndex] [2]].Tag;

			this.GetComponent<Text> ().text = text;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
