using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using DS = DataSingleton;


//This is a wall spawner object that will
//generate the walls of the game at the start.
public class GenerateWall : MonoBehaviour {

    //This is the wall prefab that represents the walls
    public GameObject Wall;  
	//This is object that generates pickups.
	public GameObject Generator;

    //This is the list of objects that GenerateWall has created
	//We need to keep track of this in order to properly garbage collect
    private List<GameObject> _created;

   
	// In start, we call the three initialize functions defined below.
	private void Start () {
		var obj = Instantiate(Generator, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
		_created = new List<GameObject>
		{
			obj
		}; //The generator is immediately added to the list for destroed object

		SetupColours ();
        GenerateWalls();
        GenerateCheckerBoard();
    }

	//This is called when the object is destroyed by Generate Generate Wall. Here we destroy
	//all objects that were created by this object. This is done so the game doesn't lag to hell
	private void OnDestroy()
	{
		foreach (var obj in _created)
		{
			Destroy(obj);
		}
	}



	//Here we setup the colours. This is done as a gradient utilizing data given from input.json
	private void SetupColours(){

		Color start = Data.GetColour(DS.GetData().WallData.StartColour);

		Color end = Data.GetColour(DS.GetData().WallData.EndColour);
		int maxNumWalls = DS.GetData().WallData.MaxNumWalls;
		int minNumWalls = DS.GetData().WallData.MinNumWalls;

		//Here we calculate how much of r g b we shift by
		float redShift = (end.r - start.r) / (maxNumWalls -  minNumWalls);
		float greenShift = (end.g - start.g) / (maxNumWalls - minNumWalls);
		float blueShift = (end.b - start.b) / (maxNumWalls - minNumWalls);


		//And we instantiate the color to the appropriate color on the continuom 
		Color color = new Color()
		{
			r = start.r + redShift * (DS.GetData().WallData.Sides - minNumWalls),
			g = start.g + greenShift * (DS.GetData().WallData.Sides - minNumWalls),
			b = start.b + blueShift * (DS.GetData().WallData.Sides - minNumWalls)
		};

		//And here we set the color of the wall prefab to the appropriate color
		Wall.GetComponent<Renderer>().sharedMaterial.color = color;
			
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
                    Wall, 
                    new Vector3((0.5f + i + j % 2), 0.001f, (0.5f + j)), //With a one offset
                    Quaternion.identity
                );

                tile.transform.localScale = new Vector3(1, 0.001f, 1);
                _created.Add(tile);
            }
        }

    }


	//This function creates the walls
    private void GenerateWalls()
    {
		//This computes the current interior angle of the given side.
		var interiorAngle = 360f / DS.GetData().WallData.Sides; //This is, of course, given as 360 / num sides

		//This sets the initial angle to the one given in the preset
		float currentAngle = DS.GetData().WallData.InitialAngle;
		if (DS.GetData().OnCorner) //This means that the system will be generated with a corner remaining fixed rather than a side
			currentAngle += interiorAngle / 2;


		//Here we interate through all the sides
		for (int i = 0; i < DS.GetData().WallData.Sides; i++)
		{
			//We compute the sin and cos of the current angle (essentially plotting points on a circle
			float x = Cos(currentAngle) * DS.GetData().WallData.Radius;
			float y = Sin(currentAngle) * DS.GetData().WallData.Radius;


			//Here we create the wall
			GameObject obj = Instantiate(Wall,
				new Vector3(x, DS.GetData().WallData.WallHeight/2, y),
				Quaternion.identity
			);

			//This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
			//Such as:
			//Gaps appearing in large wall numbers
			//Desealing some stuff. so, bad.
			float length = 2 * DS.GetData().WallData.Radius * Tan(180 / DS.GetData().WallData.Sides);



			//So we add 10 because the end user won't be able to notice it anyways
			obj.transform.localScale = new Vector3(length + 10, DS.GetData().WallData.WallHeight, 0.5f);

			//This rotates the walls by the current angle + 90
			obj.transform.Rotate(Quaternion.Euler(0, - currentAngle - 90, 0).eulerAngles);

			//And we add the wall to the created list as to remove it later
			_created.Add(obj);

			//And of course we increment the interior angle.
			currentAngle += interiorAngle;
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
