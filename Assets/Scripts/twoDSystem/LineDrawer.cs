using System;
using System.Collections.Generic;
using data;
using UnityEngine;
using UnityEngine.UI;
using wallSystem;

using DS = data.DataSingleton;
using E = main.Loader;

// TODO : Refactor this class and GenerateWall to use the same functions
namespace twoDSystem
{
    // See generatewall but more ghetto
    // Attached to the 2DWallGen Object (in 2d scenes)
    public class LineDrawer : MonoBehaviour
    {
        public GameObject Wall;

        //This is the list of objects that LineDrawer has created
        //We need to keep track of this in order to properly garbage collect
        private List<GameObject> _created;

        private void Start()
        {
            _created = new List<GameObject>();

            GameObject.Find("CountDown").GetComponent<Text>().text = "";

            var goalText = GameObject.Find("Goal").GetComponent<Text>();
            goalText.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 40);
            goalText.text = E.Get().CurrTrial.trialData.DisplayText ?? "Test";
            goalText.color = Color.black;

            Generate2dWalls();
            Generate2dLandmarks();
            var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;
            GameObject.Find("Plane").transform.localScale *= previousTrial.enclosure.Radius / 10f;
        }

        // This is called when the object is destroyed. Here we destroy
        // all objects that were created by this object. This is done so the game doesn't lag to hell.
        private void OnDestroy()
        {
            foreach (var obj in _created)
            {
                Destroy(obj);
            }
        }

        // Generate walls based on the parameters of the previous enclosure
        // TODO: Make 2d trials work with their own enclosures.
        private void Generate2dWalls()
        {
            var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;
            //This computes the current interior angle of the given side.
            var interiorAngle = 360f / previousTrial.enclosure.Sides; //This is, of course, given as 360 / num sides

            //This sets the initial angle to the one given in the preset
            float currentAngle = 0;

            //Here we interate through all the sides
            for (var i = 0; i < previousTrial.enclosure.Sides; i++)
            {
                //We compute the sin and cos of the current angle (essentially plotting points on a circle
                var x = GenerateWall.Cos(currentAngle) * previousTrial.enclosure.Radius + previousTrial.enclosure.Position[0];
                var y = GenerateWall.Sin(currentAngle) * previousTrial.enclosure.Radius + previousTrial.enclosure.Position[1];

                //This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
                //Such as:
                //Gaps appearing in large wall numbers
                //Desealing some stuff. so, bad.
                var length = 2 * previousTrial.enclosure.Radius * GenerateWall.Tan(180f / previousTrial.enclosure.Sides);

                //Here we create the wall
                var obj = Instantiate(Wall,
                    new Vector3(x, 0.001F, y),
                    Quaternion.identity
                );

                //So we add 10 because the end user won't be able to notice it anyways
                obj.transform.localScale = new Vector3(length, 4F, 0.5f);

                //This rotates the walls by the current angle + 90
                obj.transform.Rotate(Quaternion.Euler(0, -currentAngle - 90, 0).eulerAngles);

                //And we add the wall to the created list as to remove it later
                _created.Add(obj);

                //And of course we increment the interior angle.
                currentAngle += interiorAngle;
            }
        }

        //Generates the landmarks, pretty similar to the data in pickup.
        private void Generate2dLandmarks()
        {
            foreach (var p in E.Get().CurrTrial.TrialProgress.PreviousTrial.trialData.LandMarks)
            {
                var d = DS.GetData().Landmarks[p - 1];

                GameObject prefab;
                GameObject landmark;
                if (d.Type.ToLower().Equals("3d"))
                {
                    prefab = (GameObject)Resources.Load("3D_Objects/" + d.Object.ToUpper(), typeof(GameObject));
                    landmark = Instantiate(prefab);
                    landmark.AddComponent<RotateBlock>();
                    landmark.GetComponent<Renderer>().material.color = Data.GetColour(d.Color);
                }
                else
                {
                    // Load the "2D" prefab here, so we have the required components
                    prefab = (GameObject)Resources.Load("2D_Objects/" + d.Object.ToUpper(), typeof(GameObject));
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

                landmark.transform.Rotate(new Vector3(0, 1, 0), d.InitialRotation);

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
    }
}
