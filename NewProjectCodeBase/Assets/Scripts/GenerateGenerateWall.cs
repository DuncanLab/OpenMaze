using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenerateGenerateWall : MonoBehaviour {
    public GameObject create;
    public GenerateWall.Data globalData;
    private GameObject currCreate;
    public float delay = 0.01f; 

    private float timestamp;
    private GenerateWall.Data GetData()
    {

        string file = System.IO.File.ReadAllText("Assets/InputFiles/input.json");
        GenerateWall.Data data = JsonUtility.FromJson<GenerateWall.Data>(file);
        return data;
    }


    // Use this for initialization
    void Start () {
        timestamp = 0;
        globalData = GetData();
        currCreate = Instantiate(create);
    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= timestamp)
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                if (globalData.Sides > GenerateWall.minNumWalls)
                {
                    globalData.Sides--;
                    Destroy(currCreate);
                    currCreate = Instantiate(create);
                }
            }

            else if (Input.GetKey(KeyCode.Alpha2))
            {
                if (globalData.Sides < GenerateWall.maxNumWalls)
                {
                    globalData.Sides++;
                    Destroy(currCreate);
                    currCreate = Instantiate(create);
                }
            }

            else if (Input.GetKey(KeyCode.Alpha3))
            {
                int diff = GenerateWall.maxNumWalls - GenerateWall.minNumWalls + 1;
                int val = (int)(Random.value * diff) + GenerateWall.minNumWalls;
                globalData.Sides = val;
                Destroy(currCreate);
                currCreate = Instantiate(create);
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                Destroy(currCreate);
                currCreate = Instantiate(create);
            }
            timestamp = Time.time + delay;
        }
    }
}
