using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Configuration;
using UnityEngine;
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
	
	//This field is used for the screenshot system. We should move that to another class (probably).
	private bool _begin;
	private DataManager _dataManager;
	
	//current generate wall object that exists. This is intrinsically different from 
	//the Create object as that is a prefab while this is the instance.
	private GameObject _currCreate; 
	
	//This is the delay value between key presses of changing number of walls in seconds.
	private const float Delay = 0.1f;
	
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
		
		//This field loads the data from datamanager into the data singleton
		DataSingleton.SetData (_dataManager.Data);
		
		//We destroy the current currcreate object
		Destroy(_currCreate);
		
		//And then we reinstantiate it
		_currCreate = Instantiate(Create);
	}


	//This code is copy and pasted from https://www.assetstore.unity3d.com/en/#!/content/24122
	public void TakeScreenshot(string fileName){
		const int resWidthN = 1920;
		const int resHeightN = 1080;
		var rt = new RenderTexture(resWidthN, resHeightN, 24);
		Cam.targetTexture = rt;

		const TextureFormat tFormat = TextureFormat.RGB24;


		var screenShot = new Texture2D(resWidthN, resHeightN, tFormat,false);
		Cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
		Cam.targetTexture = null;
		RenderTexture.active = null; 
		var bytes = screenShot.EncodeToPNG();

		File.WriteAllBytes(fileName, bytes);
	}
	
	//This function creates a new zone with random number of walls
	public void NewZoneRandom()
	{
		var r = new System.Random();
		var diff = DS.GetData().WallData.MaxNumWalls - DS.GetData().WallData.MinNumWalls + 1;
		var val = r.Next(diff) + DS.GetData().WallData.MinNumWalls;
		DS.GetData().WallData.Sides = val;
		ResetCreate();
	}

	//------------------- This is the inbuilt unity functions

	// Use this for initialization
	private void Start () {
		Create.transform.position = Vector3.zero;
		_timestamp = 0;
		_currCreate = Instantiate(Create);
		_dataManager = GameObject.Find ("Data").GetComponent<DataManager> ();
	}


	private void TakeRotationalSceenshot()
	{
		//First we generate the wall and then get the value.
		var fileOutputVal = Constants.OutputDirectory + "/" + DS.GetData().OutputFolderName + "/AllAngelsWall" + DS.GetData().WallData.Sides;
		Directory.CreateDirectory(fileOutputVal);

		for (var i = 0; i < 8; i ++)
		{
			
			
			var fileName = DS.GetData().WallData.Sides + "A" + (i+1)*45 + ".png";
			Player.transform.Rotate(0, 45, 0);
			TakeScreenshot(fileOutputVal + "/" + fileName);

		}
		
	}

	// Update is called once per frame
	private void Update () {
		if (!(Time.time - _timestamp >= Delay) || !DS.GetData().DeveloperMode || Loader.ExperimentMode) return;
		if (DS.GetData().WallData.Sides <= DS.GetData().WallData.MaxNumWalls && _begin) {

			TakeRotationalSceenshot();
			//TakeScreenshot ("Assets/OutputFiles~/" + DS.GetData().OutputFolderName + "/Wall" + DS.GetData().WallData.Sides + ".png");


			DS.GetData().WallData.Sides += DS.GetData().WallData.WallStep;


			ResetCreate ();

		} else {
			_begin = false;
		}


		if (Input.GetKey (KeyCode.H)) {
			Directory.CreateDirectory ("Assets/OutputFiles~/" + DS.GetData ().OutputFolderName);

			DS.GetData ().WallData.Sides = 4;
			_begin = true;
		}


		//This is the input key for decreasing walls. This is button 1		
		else if (Input.GetKey (KeyCode.Alpha1)) {
			if (DS.GetData ().WallData.Sides > DS.GetData ().WallData.MinNumWalls) {
				DS.GetData ().WallData.Sides -= DS.GetData ().WallData.WallStep;
				ResetCreate ();
			}
		}

		//This is the input key for increasing walls. This is button 2
		else if (Input.GetKey (KeyCode.Alpha2)) {
			if (DS.GetData ().WallData.Sides < DS.GetData ().WallData.MaxNumWalls) {
				DS.GetData ().WallData.Sides += DS.GetData ().WallData.WallStep;
				ResetCreate ();

			}
		}

		//This is the input key for increasing walls. This is button 3
		else if (Input.GetKey (KeyCode.Alpha3)) {
			NewZoneRandom ();

		}

		//This is the input key for increasing walls. This is space key
		else if (Input.GetKey (KeyCode.Space)) {
			ResetCreate ();

		} else if (Input.GetKey (KeyCode.F)) {
			DataSingleton.Save ();
			
		}
		//This is the input key for screenshots. For now, I'm going to set it to be the G key
		else if (Input.GetKey (KeyCode.G)) {
			TakeScreenshot ("Assets/OutputFiles~/Screenshot/Screenshot.png");
		} 
		
		else if (Input.GetKey(KeyCode.J))
		{
			TakeRotationalSceenshot();
		}
		
		
		//Increment the timestamp
		_timestamp = Time.time;
	}

}
