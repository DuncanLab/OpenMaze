using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DS = DataSingleton;


//This script is the Generate (GenerateWall) script
//It generates the GenerateWall object.
public class GenerateGenerateWall : MonoBehaviour {
	public GameObject create; //This create is prefab of the create object.
	public Camera cam;	
	private DataManager dataManager;
	private GameObject currCreate; //current generate wall object that exists.

	private AudioClip audioClip;

	public void SetWaveSrc(string file)
	{

		string wavSrc = "Audio\\" + file;

		audioClip = Resources.Load(wavSrc) as AudioClip;
	}

	public AudioClip GetWaveSrc()
	{
		return audioClip;
	}

	public Text timer;


	public void ResetCreate()
	{
		DataSingleton.SetData (dataManager.data);
		Destroy(currCreate);
		currCreate = Instantiate(create);
	}


	//This is the delay value between key presses of changing number of walls in seconds.
	private const float delay = 0.1f;

	private static bool loaded = false;



	//This is the current running timestamp that is outputted to console.	
	private float timestamp;


	// Use this for initialization
	void Start () {
		create.transform.position = Vector3.zero;
		timestamp = 0;
		currCreate = Instantiate(create);
		dataManager = GameObject.Find ("Data").GetComponent<DataManager> ();
	}


	void TakeScreenshot(string fileName){
		int resWidthN = 1920;
		int resHeightN = 1080;
		RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
		cam.targetTexture = rt;

		TextureFormat tFormat;
		tFormat = TextureFormat.RGB24;


		Texture2D screenShot = new Texture2D(resWidthN, resHeightN, tFormat,false);
		cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
		cam.targetTexture = null;
		RenderTexture.active = null; 
		byte[] bytes = screenShot.EncodeToPNG();

		System.IO.File.WriteAllBytes(fileName, bytes);
	}


	private bool begin = false;
	// Update is called once per frame
	void Update () {
		if (Time.time - timestamp >=  delay) //Checking to see if the current time is > than timestamp
		{

			if (DS.GetData().WallData.Sides <= DS.GetData().WallData.MaxNumWalls && begin) {


				TakeScreenshot ("Assets/OutputFiles~/" + DS.GetData().OutputFolderName + "/Wall" + DS.GetData().WallData.Sides + ".png");



				DS.GetData().WallData.Sides += DS.GetData().WallData.WallStep;


				ResetCreate ();

			} else {
				begin = false;
			}


			if (Input.GetKey (KeyCode.H)) {
				System.IO.Directory.CreateDirectory ("Assets/OutputFiles~/" + DS.GetData ().OutputFolderName);

				DS.GetData ().WallData.Sides = 4;
				begin = true;
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
			timestamp = Time.time;
		}
	}
	public void NewZoneRandom()
	{
		System.Random r = new System.Random();
		int diff = DS.GetData().WallData.MaxNumWalls - DS.GetData().WallData.MinNumWalls + 1;
		int val = r.Next(diff) + DS.GetData().WallData.MinNumWalls;
		DS.GetData().WallData.Sides = val;
		ResetCreate();
	}
}
