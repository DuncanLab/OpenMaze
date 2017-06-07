using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This is a wall spawner object that will
//generate the walls of the game at the start.
public class GenerateWall : MonoBehaviour {
    private Data data;
    public static int minNumWalls = 4;
    public static int maxNumWalls = 40;
    public GameObject wall;
    public GameObject parent;
    public float yScale = 10f;
    public List<GameObject> created;

    //This is an inner data class
    //The reason this inner class exists is for easy serialization from an input file
    [System.Serializable]
    public class Data
    {
        public int Sides;
        public string StartColour;
        public string EndColour;
        public int Radius;
        public int InitialAngle;

       
    }
    
    
	// Use this for initialization
	void Start () {
        created = new List<GameObject>();
        GameObject obj = GameObject.Find("WallCreator");
        data = obj.GetComponent<GenerateGenerateWall>().globalData;

        Debug.Log(data.EndColour);

        Color start = GetColour(data.StartColour);
        
        Color end = GetColour(data.EndColour);

        Debug.Log(start);
        Debug.Log(end);


        float redShift = (end.r - start.r) / (maxNumWalls -  minNumWalls);
        float greenShift = (end.g - start.g) / (maxNumWalls - minNumWalls);
        float blueShift = (end.b - start.b) / (maxNumWalls - minNumWalls);



        Color color = new Color()
        {
            r = start.r + redShift * (data.Sides - minNumWalls),
            g = start.g + greenShift * (data.Sides - minNumWalls),
            b = start.b + blueShift * (data.Sides - minNumWalls)
        };
        Debug.Log(data.Sides);

        wall.GetComponent<Renderer>().sharedMaterial.color = color;

        GenerateWalls();
        GenerateCheckerBoard();
    }

    void OnDestroy()
    {
        foreach (GameObject obj in created)
        {
            Destroy(obj);
        }
    }

    void Update()
    {
        
    }
    
    private static Color GetColour(string hex)
    {
        float[] l = { 0, 0, 0 };
        for (int i = 0; i < 6; i += 2)
        {
            float decValue = int.Parse(hex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
            l[i/2] = decValue / 255;
        }
        return new Color(l[0], l[1], l[2]);

    }
    
    private void GenerateCheckerBoard()
    {
        
        for (int i = -20; i < 20; i += 2)
        {
            for (int j = -20; j < 20; j += 1)
            {
                GameObject tile = Instantiate(
                    wall, 
                    new Vector3((0.5f + i + j % 2), 0.001f, (0.5f + j)), 
                    Quaternion.identity
                    );

                tile.transform.localScale = new Vector3(1, 0.001f, 1);
                created.Add(tile);
            }
        }
        

    }

    private void GenerateWalls()
    {
        //This computes the current interior angle of the given side.
        float InteriorAngle = 360f / data.Sides;
        float CurrentAngle = data.InitialAngle;
        
        CurrentAngle = data.InitialAngle;

        for (int i = 0; i < data.Sides; i++)
        {
            float x = Cos(CurrentAngle) * data.Radius;
            float y = Sin(CurrentAngle) * data.Radius;



            GameObject obj = Instantiate(wall,
                new Vector3(x, yScale/2, y),
                Quaternion.identity
            );


            float length = 2 * data.Radius * Tan(180 / data.Sides);
            

            obj.transform.localScale = new Vector3(length + 10, yScale, 0.5f);

            obj.transform.Rotate(Quaternion.Euler(0, - CurrentAngle - 90, 0).eulerAngles);
            created.Add(obj);
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
