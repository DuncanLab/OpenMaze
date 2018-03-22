using System;
using System.Collections.Generic;
using data;
using twoDSystem;
using UnityEngine;
using E = main.Loader;
using DS = data.DataSingleton;

//This is a wall spawner object that will
//generate the walls of the game at the start.
namespace wallSystem
{
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
			if (E.Get().CurrTrial.Value.GroundType.Equals("Checker"))
				GenerateCheckerBoard();
			else if (E.Get().CurrTrial.Value.GroundType.Equals("Solid"))
			{
				var col = Data.GetColour(E.Get().CurrTrial.Value.GroundColor);

				GameObject.Find("Ground").GetComponent<Renderer>().material.color = col;
			}
			else
			{
				GameObject.Find("Ground").GetComponent<Renderer>().enabled = false;

			}

			GenerateLandmarks();
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

			var col = Data.GetColour(E.Get().CurrTrial.Value.WallColor);
			//And here we set the color of the wall prefab to the appropriate color
			Wall.GetComponent<Renderer>().sharedMaterial.color = col;

			
		}


		private void GenerateLandmarks()
		{
			foreach (var p in E.Get().CurrTrial.Value.LandMarks)
			{
				var d =DS.GetData().Landmarks[p - 1];

				var landmark = (GameObject)Instantiate(Resources.Load("Prefabs/" + d.Type));
				
			
				landmark.transform.localScale = new Vector3(d.Length, d.Height, d.Width);
				try
				{
					landmark.transform.position = new Vector3(d.Location[0], d.Location[1], d.Location[2]);
				}
				catch (Exception _)
				{
					landmark.transform.position = new Vector3(d.Location[0], d.Location[1], 0.5f);
					
				}

				landmark.transform.Rotate(new Vector3(0, 1, 0), d.InitialRotation);
				
				
				landmark.GetComponent<Renderer>().material.color = Data.GetColour(d.Color);
				var sprite = d.ImageLoc;
				if (sprite != null)
				{
					var pic = Img2Sprite.LoadNewSprite(Constants.InputDirectory + sprite);
					landmark.GetComponent<SpriteRenderer>().sprite = pic;
				}

				_created.Add(landmark);
			}
		}
	
    

		//This function generates the checkerboard. We can modify the size of this later.
		private void GenerateCheckerBoard()
		{
			var val = E.Get().CurrTrial.Value.Radius * 2;
			var col = Data.GetColour(E.Get().CurrTrial.Value.GroundColor);

			//Quite simply, this is a 2d for loop
			for (var i = -val; i < val; i += 2)
			{
				for (var j = -val; j < val; j += 1)
				{

					var tile = Instantiate(
						Wall, 
						new Vector3((0.5f + i + j % 2), 0.001f, (0.5f + j)), //With a one offset
						Quaternion.identity
					);
					tile.GetComponent<Renderer>().material.color = col;

					tile.transform.localScale = new Vector3(1, 0.001f, 1);
					_created.Add(tile);
				}
			}

		}


		//This function creates the walls
		private void GenerateWalls()
		{
			//This computes the current interior angle of the given side.
			var interiorAngle = 360f / E.Get().CurrTrial.Value.Sides; //This is, of course, given as 360 / num sides

			//This sets the initial angle to the one given in the preset
			float currentAngle = 0;

			GameObject.Find("Ground").transform.localScale *= E.Get().CurrTrial.Value.Radius / 10f;
			//Here we interate through all the sides
			for (var i = 0; i < E.Get().CurrTrial.Value.Sides; i++)
			{
				//We compute the sin and cos of the current angle (essentially plotting points on a circle
				var x = Cos(currentAngle) * E.Get().CurrTrial.Value.Radius;
				var y = Sin(currentAngle) * E.Get().CurrTrial.Value.Radius;
			
				//This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
				//Such as:
				//Gaps appearing in large wall numbers
				//Desealing some stuff. so, bad.
				var length = 2 * E.Get().CurrTrial.Value.Radius * Tan(180f / E.Get().CurrTrial.Value.Sides);
				
			
				//Here we create the wall
				var obj = Instantiate(Wall,
					new Vector3(x, E.Get().CurrTrial.Value.WallHeight/2, y),
					Quaternion.identity
				);


				//So we add 10 because the end user won't be able to notice it anyways
				obj.transform.localScale = new Vector3(length + 10, E.Get().CurrTrial.Value.WallHeight, 0.5f);

				//This rotates the walls by the current angle + 90
				obj.transform.Rotate(Quaternion.Euler(0, - currentAngle - 90, 0).eulerAngles);

				//And we add the wall to the created list as to remove it later
				_created.Add(obj);

				//And of course we increment the interior angle.
				currentAngle += interiorAngle;
			}
		}



		//Cosine in degrees, using the current cos in radians used by the unity math library
		public static float Cos(float degrees)
		{
			return Mathf.Cos(degrees * Mathf.PI / 180);
		}

		//Sine in degrees, using the current sin in radians used by the unity math library
		public static float Sin(float degrees)
		{
			return Mathf.Sin(degrees*Mathf.PI/180);
		}

		//Tangent in degrees, using the tan identity.

		public static float Tan(float degrees)
		{
			return Sin(degrees) / Cos(degrees);
		}
	
	

	}
}
