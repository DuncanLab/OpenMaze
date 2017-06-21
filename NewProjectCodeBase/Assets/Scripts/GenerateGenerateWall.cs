using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is the Generate (GenerateWall) script
//It generates the GenerateWall object.
public class GenerateGenerateWall : MonoBehaviour {
    public GameObject create; //This create is prefab of the create object.
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
        Destroy(currCreate);
        currCreate = Instantiate(create);
    }

    public Data globalData; //Here is the data object. This object stores the current state of the data.

    //This is the delay value between key presses of changing number of walls in seconds.
    private const float delay = 0.1f;
    


    //This is the current running timestamp that is outputted to console.
    private float timestamp;

    //Here is the file that reads data from a json file.
    private Data GetData()
    {
        string file = System.IO.File.ReadAllText("Assets/InputFiles/input.json");
        Data data = JsonUtility.FromJson<Data>(file);
        return data;
    }


    // Use this for initialization
    void Start () {
        create.transform.position = Vector3.zero;
        timestamp = 0;
        globalData = GetData();
        currCreate = Instantiate(create);
    }
	

	// Update is called once per frame
	void Update () {
        if (Time.time - timestamp >=  delay) //Checking to see if the current time is > than timestamp
        {
            //This is the input key for decreasing walls. This is button 1
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (globalData.WallData.Sides > globalData.WallData.MinNumWalls)
                {
                    globalData.WallData.Sides -= globalData.WallData.WallStep;
                    ResetCreate();
                }
            }

            //This is the input key for increasing walls. This is button 2
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                if (globalData.WallData.Sides < globalData.WallData.MaxNumWalls)
                {
                    globalData.WallData.Sides+= globalData.WallData.WallStep;
                    ResetCreate();

                }
            }

            //This is the input key for increasing walls. This is button 3
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                NewZoneRandom();

            }

            //This is the input key for increasing walls. This is space key
            else if (Input.GetKey(KeyCode.Space))
            {
                ResetCreate();

            }

            //Increment the timestamp
            timestamp = Time.time;
        }
    }
    public void NewZoneRandom()
    {
        System.Random r = new System.Random();
        int diff = globalData.WallData.MaxNumWalls - globalData.WallData.MinNumWalls + 1;
        int val = r.Next(diff) + globalData.WallData.MinNumWalls;
        globalData.WallData.Sides = val;
        ResetCreate();
    }
}
