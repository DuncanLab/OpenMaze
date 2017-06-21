using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//This is a wall spawner object that will
//generate the walls of the game at the start.
public class GenerateWall : MonoBehaviour {

    //This is the serialized version of the data object
    private Data data;

    

    //This is the wall prefab that represents the walls
    public GameObject wall;

    //This is object that generates distributions.
    public GameObject generator;
    

    //This is the list of objects that this thing created. This will be sacrificed quite frequently.
    private List<GameObject> created;

    
	// Use this for initialization
	void Start () {
        //Here we create the generator at the given location
        GameObject obj = Instantiate(generator, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        created = new List<GameObject>
        {
            obj
        }; //The generator is immediately added to the list for destroed object

        //This line attempts to find the WallCreator object in order to access the data package
        obj = GameObject.Find("WallCreator");

        //Here is the data object inside the script.
        data = obj.GetComponent<GenerateGenerateWall>().globalData;

        Color start = Data.GetColour(data.WallData.StartColour);

        Color end = Data.GetColour(data.WallData.EndColour);
        int maxNumWalls = data.WallData.MaxNumWalls;
        int minNumWalls = data.WallData.MinNumWalls;

        //Here we calculate how much of r g b we shift by
        float redShift = (end.r - start.r) / (maxNumWalls -  minNumWalls);
        float greenShift = (end.g - start.g) / (maxNumWalls - minNumWalls);
        float blueShift = (end.b - start.b) / (maxNumWalls - minNumWalls);


        //And we instantiate the color to the appropriate color on the continuom 
        Color color = new Color()
        {
            r = start.r + redShift * (data.WallData.Sides - minNumWalls),
            g = start.g + greenShift * (data.WallData.Sides - minNumWalls),
            b = start.b + blueShift * (data.WallData.Sides - minNumWalls)
        };

        //And here we set the color of the wall prefab to the appropriate color
        wall.GetComponent<Renderer>().sharedMaterial.color = color;

        //And these functions create the walls as well as the checkerboard
        GenerateWalls();
        GenerateCheckerBoard();
    }

    //This is called when the object is destroyed by Generate Generate Wall. Here we destroy
    //all objects that were created by this object. This is done so the game doesn't lag to hell
    void OnDestroy()
    {
        foreach (var obj in created)
        {
            if (obj != null)
            Destroy(obj);
        }
    }
    
    

    //This function generates the checkerboard. We can modify the size of this later.
    private void GenerateCheckerBoard()
    {
        //Quite simply, this is a 2d for loop
        for (int i = -20; i < 20; i += 2)
        {
            for (int j = -20; j < 20; j += 1)
            {
                GameObject tile = Instantiate(
                    wall, 
                    new Vector3((0.5f + i + j % 2), 0.001f, (0.5f + j)), //With a one offset
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
        float InteriorAngle = 360f / data.WallData.Sides; //This is, of course, given as 360 / num sides

        //This sets the initial angle to the one given in the preset
        float CurrentAngle = data.WallData.InitialAngle;
        
        //Here we interate through all the sides
        for (int i = 0; i < data.WallData.Sides; i++)
        {
            //We compute the sin and cos of the current angle (essentially plotting points on a circle
            float x = Cos(CurrentAngle) * data.WallData.Radius;
            float y = Sin(CurrentAngle) * data.WallData.Radius;


            //Here we create the wall
            GameObject obj = Instantiate(wall,
                new Vector3(x, data.WallData.WallHeight/2, y),
                Quaternion.identity
            );

            //This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
            //Such as:
            //Gaps appearing in large wall numbers
            //Desealing some stuff. so, bad.
            float length = 2 * data.WallData.Radius * Tan(180 / data.WallData.Sides);
            


            //So we add 10 because the end user won't be able to notice it anyways
            obj.transform.localScale = new Vector3(length + 10, data.WallData.WallHeight, 0.5f);

            //This rotates the walls by the current angle + 90
            obj.transform.Rotate(Quaternion.Euler(0, - CurrentAngle - 90, 0).eulerAngles);

            //And we add the wall to the created list as to remove it later
            created.Add(obj);

            //And of course we increment the interior angle.
            CurrentAngle += InteriorAngle;
        }
    }


    //Cosine in degrees, using the current cos in radians used by the unity math library
    private static float Cos(float degrees)
    {
        return Mathf.Cos(degrees * Mathf.PI / 180);
    }

    //Sine in degrees, using the current sin in radians used by the unity math library
    public static float Sin(float degrees)
    {
        return Mathf.Sin(degrees*Mathf.PI/180);
    }

    //Tangent in degrees, using the tan identity.

    private static float Tan(float degrees)
    {
        return Sin(degrees) / Cos(degrees);
    }

}
