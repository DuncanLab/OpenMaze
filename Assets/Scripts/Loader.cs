using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = DataSingleton;

//This class ex
public class LevelConfigData{

}


//We will convert the initial loader into what is known as the level controller. In order to properly set this up, there must exist a level config.
//We must also construct a level config builder.
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
