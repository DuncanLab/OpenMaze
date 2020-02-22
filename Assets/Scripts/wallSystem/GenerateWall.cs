using System;
using System.Collections.Generic;
using data;
using UnityEngine;
using DS = data.DataSingleton;
using E = main.Loader;

// This is a wall, floor and landmark spawner object that will
// generate these objects based on settings from the config file at the start
// of the trial.
namespace wallSystem
{
    public class GenerateWall : MonoBehaviour
    {
        public bool MakeFloor = true;
        // This is the wall prefab that represents the walls
        public GameObject Wall;
        // This is object that generates pickups.
        public GameObject Generator;

        // This is the list of objects that GenerateWall has created
        // We need to keep track of this in order to properly garbage collect
        private List<GameObject> _created;


        // In start, we call the three initialize functions defined below.
        private void Start()
        {
            var obj = Instantiate(Generator, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            _created = new List<GameObject>
            {
                obj
            }; // The generator is immediately added to the list for destroyed objects

            SetupColours();
            GenerateWalls();

            if (E.Get().CurrTrial.enclosure.GroundTileSides == 2) throw new Exception("Can't have floor tiles with 2 sides!");

            if (E.Get().CurrTrial.enclosure.GroundColor == null)
            {
                GameObject.Find("Ground").GetComponent<Renderer>().enabled = false;
            }
            else if (E.Get().CurrTrial.enclosure.GroundTileSides > 2)
            {
                GenerateTileFloor();
            }
            else
            {
                var col = Data.GetColour(E.Get().CurrTrial.enclosure.GroundColor);
                GameObject.Find("Ground").GetComponent<Renderer>().material.color = col;
            }

            GenerateLandmarks();
        }

        // This is called when the object is destroyed by Generate Generate Wall. Here we destroy
        // all objects that were created by this object. This is done so the game doesn't lag to hell
        private void OnDestroy()
        {
            foreach (var obj in _created)
            {
                Destroy(obj);
            }
        }

        // Here we setup the colours. This is done as a gradient utilizing data given from input.json
        private void SetupColours()
        {
            var col = Data.GetColour(E.Get().CurrTrial.enclosure.WallColor);
            MakeFloor = true;
            //And here we set the color of the wall prefab to the appropriate color
            if (E.Get().CurrTrial.enclosure.WallColor == "ffffff00")
            {
                Wall.GetComponent<Renderer>().enabled = false;
                GameObject.Find("Ground").GetComponent<Renderer>().enabled = false;
                E.Get().CurrTrial.enclosure.GroundTileSize = 0;
                MakeFloor = false;
            }
            if (E.Get().CurrTrial.enclosure.GroundColor == "ffffff00")
            {
                GameObject.Find("Ground").GetComponent<Renderer>().enabled = false;
                E.Get().CurrTrial.enclosure.GroundTileSize = 0;
                MakeFloor = false;
            }
          
            Wall.GetComponent<Renderer>().sharedMaterial.color = col;
        }

        // Generates the landmarks, pretty similar to the data in pickup.
        private void GenerateLandmarks()
        {
            foreach (var p in E.Get().CurrTrial.trialData.LandMarks)
            {
                var d = DS.GetData().Landmarks[p - 1];
                GameObject prefab;
                GameObject landmark;
                if (d.Type.ToLower().Equals("3d"))
                {
                    prefab = (GameObject)Resources.Load("3D_Objects/" + d.Object, typeof(GameObject));
                    landmark = Instantiate(prefab);
                    landmark.AddComponent<RotateBlock>();
                    landmark.GetComponent<Renderer>().material.color = Data.GetColour(d.Color);
                }
                else
                {
                    // Load the "2D" prefab here, so we have the required components
                    prefab = (GameObject)Resources.Load("3D_Objects/2D", typeof(GameObject));
                    landmark = Instantiate(prefab);
                    var spriteName = d.Object;
                    var pic = Img2Sprite.LoadNewSprite(DataSingleton.GetData().SpritesPath + spriteName);
                    landmark.GetComponent<SpriteRenderer>().sprite = pic;
                }
                
                landmark.transform.localScale = d.ScaleVector;
                try
                {
                    landmark.transform.position = d.PositionVector;
                }
                catch (Exception _)
                {
                    landmark.transform.position = new Vector3(d.PositionVector.x, 0.5f, d.PositionVector.z);
                }

                //landmark.transform.Rotate(new Vector3(0, 1, 0), d.InitialRotation);
                landmark.transform.Rotate(d.RotationVector);


                landmark.GetComponent<Renderer>().material.color = Data.GetColour(d.Color);
                var sprite = d.ImageLoc;
                if (sprite != null)
                {
                    var pic = Img2Sprite.LoadNewSprite(DataSingleton.GetData().SpritesPath + sprite);
                    landmark.GetComponent<SpriteRenderer>().sprite = pic;
                }

                _created.Add(landmark);
            }
        }

        // This function generates the tile floor. We can modify the size of this later.
        private void GenerateTileFloor()
        {
            if (MakeFloor == true)
            {
                var val = E.Get().CurrTrial.enclosure.Radius * 2;

                // Setup the polygon mesh (using sensible defaults).
                int numSides = E.Get().CurrTrial.enclosure.GroundTileSides == 0 ? 4 : E.Get().CurrTrial.enclosure.GroundTileSides;
                double tileSize = E.Get().CurrTrial.enclosure.GroundTileSize == 0.0 ? 1.0 : E.Get().CurrTrial.enclosure.GroundTileSize;
                var col = Data.GetColour(E.Get().CurrTrial.enclosure.GroundColor);
                Mesh mesh = ConstructTileMesh(numSides, tileSize);


                // Generate a grid of tiles
                var xStart = E.Get().CurrTrial.enclosure.Position[0] - val;
                var yStart = E.Get().CurrTrial.enclosure.Position[1] - val;
                var xEnd = xStart + (val * 2);
                var yEnd = yStart + (val * 2);
                for (float i = xStart; i <= xEnd; i += 2)
                {
                    for (float j = yStart; j <= yEnd; j += 2)
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

        private void GenerateWalls()
        {
            if ((int)E.Get().CurrTrial.enclosure.WallHeight == 0) return;

            //This computes the current interior angle of the given side.
            var interiorAngle = 360f / E.Get().CurrTrial.enclosure.Sides; //This is, of course, given as 360 / num sides

            //This sets the initial angle to the one given in the preset
            float currentAngle = 0;

            GameObject.Find("Ground").transform.localScale *= E.Get().CurrTrial.enclosure.Radius / 20f;
            GameObject.Find("Ground").transform.position = new Vector3(E.Get().CurrTrial.enclosure.Position[0], 0, E.Get().CurrTrial.enclosure.Position[1]);

            //Here we interate through all the sides
            for (var i = 0; i < E.Get().CurrTrial.enclosure.Sides; i++)
            {
                //We compute the sin and cos of the current angle (essentially plotting points on a circle
                var x = Cos(currentAngle) * E.Get().CurrTrial.enclosure.Radius + E.Get().CurrTrial.enclosure.Position[0];
                var y = Sin(currentAngle) * E.Get().CurrTrial.enclosure.Radius + E.Get().CurrTrial.enclosure.Position[1];

                //This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
                //Such as:
                //Gaps appearing in large wall numbers
                //Desealing some stuff. so, bad.
                var length = 2 * E.Get().CurrTrial.enclosure.Radius * Tan(180f / E.Get().CurrTrial.enclosure.Sides);

                //Here we create the wall
                var obj = Instantiate(Wall,
                    new Vector3(x, E.Get().CurrTrial.enclosure.WallHeight / 2 - .1f, y),
                    Quaternion.identity
                );

                // So we add 10 because the end user won't be able to notice it anyways
                obj.transform.localScale = new Vector3(length + 10, E.Get().CurrTrial.enclosure.WallHeight, 0.5f);

                // This rotates the walls by the current angle + 90
                obj.transform.Rotate(Quaternion.Euler(0, -currentAngle - 90, 0).eulerAngles);

                // And we add the wall to the created list as to remove it later
                _created.Add(obj);

                // And of course we increment the interior angle.
                currentAngle += interiorAngle;
            }
        }

        // Cosine in degrees, using the current cos in radians used by the unity math library
        public static float Cos(float degrees)
        {
            return Mathf.Cos(degrees * Mathf.PI / 180);
        }

        // Sine in degrees, using the current sin in radians used by the unity math library
        public static float Sin(float degrees)
        {
            return Mathf.Sin(degrees * Mathf.PI / 180);
        }

        // Tangent in degrees, using the tan identity.
        public static float Tan(float degrees)
        {
            return Sin(degrees) / Cos(degrees);
        }
    }
}