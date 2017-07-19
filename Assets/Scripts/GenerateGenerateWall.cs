using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DS = DataSingleton;


//This script is the Generate (GenerateWall) script
//It generates the GenerateWall object.
public class GenerateGenerateWall : MonoBehaviour {
	public GameObject Create; //This create is prefab of the create object.
	public Camera Cam;	
	public Text Timer;
	
	private bool _begin;
	private DataManager _dataManager;
	private GameObject _currCreate; //current generate wall object that exists.
	
	//This is the delay value between key presses of changing number of walls in seconds.
	private const float Delay = 0.1f;

	private AudioClip _audioClip;
	
	//This is the current running timestamp that is outputted to console.	
	private float _timestamp;

	public void SetWaveSrc(string file)
	{

		var wavSrc = "Audio\\" + file;

		_audioClip = Resources.Load(wavSrc) as AudioClip;
	}

	public AudioClip GetWaveSrc()
	{
		return _audioClip;
	}



	public void ResetCreate()
	{
		DataSingleton.SetData (_dataManager.Data);
		Destroy(_currCreate);
		_currCreate = Instantiate(Create);
	}






	// Use this for initialization
	private void Start () {
		Create.transform.position = Vector3.zero;
		_timestamp = 0;
		_currCreate = Instantiate(Create);
		_dataManager = GameObject.Find ("Data").GetComponent<DataManager> ();
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

		System.IO.File.WriteAllBytes(fileName, bytes);
	}


	// Update is called once per frame
	private void Update () {
		if (!(Time.time - _timestamp >= Delay) || !DS.GetData().DeveloperMode || Loader.experimentMode) return;
		if (DS.GetData().WallData.Sides <= DS.GetData().WallData.MaxNumWalls && _begin) {


			TakeScreenshot ("Assets/OutputFiles~/" + DS.GetData().OutputFolderName + "/Wall" + DS.GetData().WallData.Sides + ".png");



			DS.GetData().WallData.Sides += DS.GetData().WallData.WallStep;


			ResetCreate ();

		} else {
			_begin = false;
		}


		if (Input.GetKey (KeyCode.H)) {
			System.IO.Directory.CreateDirectory ("Assets/OutputFiles~/" + DS.GetData ().OutputFolderName);

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
				
		//Increment the timestamp
		_timestamp = Time.time;
	}
	public void NewZoneRandom()
	{
		var r = new System.Random();
		var diff = DS.GetData().WallData.MaxNumWalls - DS.GetData().WallData.MinNumWalls + 1;
		var val = r.Next(diff) + DS.GetData().WallData.MinNumWalls;
		DS.GetData().WallData.Sides = val;
		ResetCreate();
	}
}
