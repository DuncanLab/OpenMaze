using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This is a wall spawner object that will
//generate the walls of the game at the start.
public class GenerateWall : MonoBehaviour {
    private Data data;
    public GameObject wall;
    public float yScale = 10f;

    //This is an inner data class
    //The reason this inner class exists is for easy serialization from an input file
    private class Data
    {
        public int Sides { get; set; }
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public int Radius { get; set; }
        public int InitialAngle { get; set; }
    }
    
    //This function will return a new data object
    //For now, we will hardcode the values for testing.
    //In the future, a configuration json file will be loaded in for testing.
    private Data GetData()
    {
        Data data = new Data()
        {
            Red = 1,
            Green = 0.5f,
            Blue = 0.5f,
            Sides = 10,
            Radius = 10,
            InitialAngle = 0 
        };
        return data;
    }

	// Use this for initialization
	void Start () {
        this.data = GetData();
        Generate();
    }

    void Update()
    {
        Vector3 [] arr = { 
             new Vector3(0, 0, data.Radius),
             new Vector3(data.Radius/Mathf.Sqrt(2), 0, data.Radius/Mathf.Sqrt(2)),
             new Vector3(-data.Radius/Mathf.Sqrt(2), 0, data.Radius/Mathf.Sqrt(2)),
             new Vector3(data.Radius, 0, 0),   
        };
        foreach (Vector3 point in arr)
        {
            
            Debug.DrawLine(-point, point, Color.red);

        }
    }


    private void Generate()
    {
        //This computes the current interior angle of the given side.
        float InteriorAngle = 360f / data.Sides;
        float CurrentAngle = data.InitialAngle;

        Color rgb = new Color(data.Red, data.Green, data.Blue);
        
        CurrentAngle = data.InitialAngle;

        for (int i = 0; i < data.Sides; i++)
        {
            float x = Cos(CurrentAngle) * data.Radius;
            float y = Sin(CurrentAngle) * data.Radius;



            GameObject obj = Instantiate(wall,
                new Vector3(x, yScale/2, y),
                Quaternion.identity
            );

            obj.GetComponent<Renderer>().material.color = rgb;

            float length = 2 * data.Radius * Tan(180 / data.Sides);
            

            obj.transform.localScale = new Vector3(length + 10, yScale, 0.5f);

            obj.transform.Rotate(Quaternion.Euler(0, - CurrentAngle - 90, 0).eulerAngles);

            CurrentAngle += InteriorAngle;
        }
    }


  
    private static float Cos(float degrees)
    {
        return Mathf.Cos(degrees * Mathf.PI / 180);
    }

    private static float Sin(float degrees)
    {
        return Mathf.Sin(degrees*Mathf.PI/180);
    }

    private static float Tan(float degrees)
    {
        return Sin(degrees) / Cos(degrees);
    }

}
