using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = DataSingleton;

//This class is pretty simple
//We use this to generate the scene and then load it properly at the very beginning
//A dummy scene was made for this.
public class Loader : MonoBehaviour {
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
		DS.Load ();

		SceneManager.LoadScene (DS.GetData().EnvironmentType);

	
	}
	
	// Update is called once per frame
	void Update () {





		if (Input.GetKey (KeyCode.K)) {
			SceneManager.LoadScene (0);

		} else if (Input.GetKey (KeyCode.L)) {
			SceneManager.LoadScene (1);
		}

	}
}
