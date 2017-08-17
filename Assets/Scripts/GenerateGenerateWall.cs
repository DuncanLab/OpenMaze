using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DS = DataSingleton;


//This script is the Generate (GenerateWall) script
//This is essentially the god class, the backbone of the project.
public class GenerateGenerateWall : MonoBehaviour {
	//-------------------------- Fields -------------------------------------
	public GameObject Create; //This is the prefab of the GenerateWall object.
	public Camera Cam;		  //This is the main camera of the player.
	public Text Timer;		  //This exists as the timer text.
	public GameObject Player;

	
	
	//current generate wall object that exists. This is intrinsically different from 
	//the Create object as that is a prefab while this is the instance.
	private GameObject _currCreate; 
	
	//This is the current audioclip associated with the pickup item. This is retrived from the input.json file
	private AudioClip _audioClip;
	
	//This is the current running timestamp that is outputted to the outputfile.
	private float _timestamp;
	
	//------------------------------------------------------
	//These two functions are getters and setters to the wave src file.	
	
	public void SetWaveSrc(string file)
	{

		var wavSrc = "Audio\\" + file;

		_audioClip = Resources.Load(wavSrc) as AudioClip;
	}

	public AudioClip GetWaveSrc()
	{
		return _audioClip;
	}

	//-----------------------------This is the ResetCreate data structure----------------
	public void ResetCreate()
	{	
		//We destroy the current currcreate object
		Destroy(_currCreate);
		
		//And then we reinstantiate it
		_currCreate = Instantiate(Create);
	}
	

	//------------------- This is the inbuilt unity functions

	// Use this for initialization
	private void Start () {
		Create.transform.position = Vector3.zero;
		_currCreate = Instantiate(Create);

	}



}
