using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {

	public Data data;


	// Use this for initialization
	void Start () {
		DataSingleton.Load ();
		data = DataSingleton.GetData ();
		DontDestroyOnLoad (this);
	}

	void Update(){
		if (Input.GetKey (KeyCode.Space)) {
			DataSingleton.SetData (data);

		}

	}
	
}
