﻿using data;
using UnityEngine;
using UnityEngine.UI;
using wallSystem;
using E = main.Loader;
using DS = data.DataSingleton;
using System;
using System.Collections.Generic;

// TODO : Refactor this class and GenerateWall to use the same functions
namespace twoDSystem
{
	//See generatewall but more ghetto
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
			goalText.text = E.Get().CurrTrial.Value.Header ?? "Test";
			goalText.color = Color.black;
			
			Generate2dWalls();
            Generate2dLandmarks();
			var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;
			// GameObject.Find("Plane").transform.localScale *= previousTrial.Value.Radius / 10f;
			GameObject.Find("Plane").transform.localScale *= DS.GetData().Arenas[previousTrial.Value.Scene - 1].Radius / 10f;

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

        private void Generate2dWalls()
		{
			var previousTrial = E.Get().CurrTrial.TrialProgress.PreviousTrial;
			//This computes the current interior angle of the given side.
			
			//var interiorAngle = 360f / previousTrial.Value.Sides; //This is, of course, given as 360 / num sides
			var interiorAngle = 360f / DS.GetData().Arenas[previousTrial.Value.Scene - 1].Sides; //This is, of course, given as 360 / num sides

			//This sets the initial angle to the one given in the preset
			float currentAngle = 0;
		
	
			//Here we interate through all the sides
			for (var i = 0; i < DS.GetData().Arenas[previousTrial.Value.Scene - 1].Sides; i++)
			{
				//We compute the sin and cos of the current angle (essentially plotting points on a circle
				var x = GenerateWall.Cos(currentAngle) * DS.GetData().Arenas[previousTrial.Value.Scene - 1].Radius;
				var y = GenerateWall.Sin(currentAngle) * DS.GetData().Arenas[previousTrial.Value.Scene - 1].Radius;
			
				//This is theoreticially the perfect length of the wall. However, this causes a multitude of problems
				//Such as:
				//Gaps appearing in large wall numbers
				//Desealing some stuff. so, bad.
				var length = 2 * DS.GetData().Arenas[previousTrial.Value.Scene - 1].Radius * GenerateWall.Tan(180f / DS.GetData().Arenas[previousTrial.Value.Scene - 1].Sides);
			
				//Here we create the wall
				var obj = Instantiate(Wall,
					new Vector3(x, 0.001F, y),
					Quaternion.identity
				);

				//So we add 10 because the end user won't be able to notice it anyways
				obj.transform.localScale = new Vector3(length, 4F, 0.5f);

				//This rotates the walls by the current angle + 90
				obj.transform.Rotate(Quaternion.Euler(0, - currentAngle - 90, 0).eulerAngles);

                //And we add the wall to the created list as to remove it later
                _created.Add(obj);

				//And of course we increment the interior angle.
				currentAngle += interiorAngle;
			}
		}

        //Generates the landmarks, pretty similar to the data in pickup.
        private void Generate2dLandmarks()
        {
            foreach (var p in E.Get().CurrTrial.TrialProgress.PreviousTrial.Value.LandMarks)
            {
                var d = DS.GetData().Landmarks[p - 1];

                var landmark = (GameObject)Instantiate(Resources.Load("Prefabs/" + d.Type));

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

    }
}
