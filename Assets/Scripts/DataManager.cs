using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class exists mainly as a helper for the unity UI; it has no effect on
/// the program. The reason it exists is because the unity UI only picks up
/// public instance fields, and thus the singleton approach doesn't allow for dynamic
/// in editor editing like i wanted.
/// </summary>
public class DataManager : MonoBehaviour {

	/// <summary>
	/// This is the data field
	/// </summary>
	public Data Data;


	/// <summary>
	/// This function 
	/// </summary>
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
