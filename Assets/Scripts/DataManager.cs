using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour {

	public Data Data;


	// Use this for initialization
	private void Start () {
		DataSingleton.Load ();
		Data = DataSingleton.GetData ();
		DontDestroyOnLoad (this);
	}

	private void Update(){
		if (Input.GetKey (KeyCode.Space)) {
			DataSingleton.SetData (Data);
		}
	}
	
}
