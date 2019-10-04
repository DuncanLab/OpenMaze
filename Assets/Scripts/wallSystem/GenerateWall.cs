using System;
using System.Collections.Generic;
using data;
using twoDSystem;
using UnityEngine;
using E = main.Loader;
using DS = data.DataSingleton;

// This is a wall, floor and landmark spawner object that will
// generate these objects based on settings from the config file at the start
// of the trial.
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

			SetupColours();
			GenerateWalls();

            //if (E.Get().CurrTrial.Value.GroundTileSides == 2) throw new Exception("Can't have floor tiles with 2 sides!");
            if (DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSides == 2) throw new Exception("Can't have floor tiles with 2 sides!");

            if (DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundColor == null)
            {
                GameObject.Find("Ground").GetComponent<Renderer>().enabled = false;
            }
            else if (DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSides > 2)
            {
                GenerateTileFloor();
            }
            else
            {
                var col = Data.GetColour(DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundColor);
                GameObject.Find("Ground").GetComponent<Renderer>().material.color = col;
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

			var col = Data.GetColour(DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].WallColor);
			//And here we set the color of the wall prefab to the appropriate color
			Wall.GetComponent<Renderer>().sharedMaterial.color = col;

			
		}

		//Generates the landmarks, pretty similar to the data in pickup.
		private void GenerateLandmarks()
		{
			foreach (var p in E.Get().CurrTrial.Value.LandMarks)
			{
				var d =DS.GetData().Landmarks[p - 1];

				var landmark = (GameObject)Instantiate(Resources.Load("Prefabs/" + d.Type));
				
				Debug.Log("D.SCALE");
				Debug.Log(d.Scale[0]);
				Debug.Log(d.Scale[1]);
				Debug.Log(d.Scale[2]);
				landmark.transform.localScale = new Vector3(d.Scale[0], d.Scale[2], d.Scale[1]);
				try
				{
					landmark.transform.position = new Vector3(d.Position[0], d.Position[2], d.Position[1]);
				}
				catch (Exception _)
				{
					landmark.transform.position = new Vector3(d.Position[0], 0.5f, d.Position[1]);
					
				}
				landmark.transform.Rotate(d.Rotation[0], d.Rotation[1], d.Rotation[2]);
			
				
				
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
	
    

		//This function generates the tile floor. We can modify the size of this later.
		private void GenerateTileFloor()
        {
            var val = DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Radius * 2;

            // Setup the polygon mesh (using sensible defaults).
            int numSides = DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSides == 0 ? 4 : DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSides;
            double tileSize = DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSize == 0.0 ? 1.0 : DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundTileSize;
            var col = Data.GetColour(DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].GroundColor);
            Mesh mesh = ConstructTileMesh(numSides, tileSize);

            // Generate a grid of tiles
            for (float i = -val; i < val; i += 2)
            {
                for (float j = -val; j < val; j += 2)
                {
                    var tile = Instantiate(Wall, new Vector3(i, 0.001f, j), Quaternion.identity);
                    tile.GetComponent<MeshFilter>().mesh = mesh;
                    tile.GetComponent<Renderer>().material.color = col;
                    tile.transform.localScale = new Vector3(1, 0.001f, 1);
                    // Use the rotate if the pattern looks off
                    //tile.transform.Rotate(0, -45, 0);
                    _created.Add(tile);
                }
            }
        }

        private static Mesh ConstructTileMesh(int numSides, double tileSize)
        {
            // Generate the vertices to be used for the mesh
            Vector2[] vertices2D = new Vector2[numSides];
            for (var i = 0; i < numSides; i++)
            {
                var x = tileSize * Math.Cos(2 * Math.PI * i / numSides);
                var y = tileSize * Math.Sin(2 * Math.PI * i / numSides);
                Vector2 tempVec = new Vector2((float)x, (float)y);
                vertices2D[i] = tempVec;
            }

            // Use the triangulator to get indices for creating triangles
            Triangulator tr = new Triangulator(vertices2D);
            int[] indices = tr.Triangulate();

            // Create the Vector3 vertices
            Vector3[] vertices = new Vector3[vertices2D.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = indices
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }


        //This function creates the walls
        private void GenerateWalls()
		{
			if ((int)DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].WallHeight == 0) return;

			//This computes the current interior angle of the given side.
			var interiorAngle = 360f / DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Sides; //This is, of course, given as 360 / num sides

			//This sets the initial angle to the one given in the preset
			float currentAngle = 0;

			GameObject.Find("Ground").transform.localScale *= DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Radius / 20f;
			//Here we interate through all the sides
			for (var i = 0; i < DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Sides; i++)
			{
				//We compute the sin and cos of the current angle (essentially plotting points on a circle
				var x = Cos(currentAngle) * DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Radius;
				var y = Sin(currentAngle) * DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Radius;
			
				//This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
				//Such as:
				//Gaps appearing in large wall numbers
				//Desealing some stuff. so, bad.
				Debug.Log("SCENE VALUE");
				Debug.Log(E.Get().CurrTrial.Value.Scene - 1);
				var length = 2 * DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Radius * Tan(180f / DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].Sides);
				
				
				//Here we create the wall
				var obj = Instantiate(Wall,
					new Vector3(x, DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].WallHeight/2 - .1f, y),
					Quaternion.identity
				);


				//So we add 10 because the end user won't be able to notice it anyways
				obj.transform.localScale = new Vector3(length + 10, DS.GetData().Arenas[E.Get().CurrTrial.Value.Scene - 1].WallHeight, 0.5f);

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