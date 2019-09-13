using data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using DS = data.DataSingleton;
using E = main.Loader;

namespace wallSystem
{
    public class PickupGenerator : MonoBehaviour
    {
        private List<GameObject> _destroy;


        private static Data.Point ReadFromExternal(string inputFile)
        {

            var p = new Process
            {
                StartInfo = new ProcessStartInfo("python",
                    "Assets/InputFiles~/" + inputFile)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            p.StandardInput.Write(JsonUtility.ToJson(E.Get().CurrTrial.Value) + "\n");

            p.WaitForExit();
            var line = p.StandardOutput.ReadLine();


            while (!p.StandardError.EndOfStream)
            {
                var outputLine = p.StandardError.ReadLine();
                UnityEngine.Debug.LogError(outputLine);

            }

            if (line == null)
            {
                UnityEngine.Debug.LogError("PYTHON FILE ERROR! (Most likely the wrong python version/environment)");
                return new Data.Point { X = 5, Y = 0, Z = 5 };
            }

            var arr = line.Split(',');
            return new Data.Point
            {
                X = float.Parse(arr[0]),
                Y = float.Parse(arr[2]),
                Z = float.Parse(arr[1])
            };

        }

        // Use this for initialization
        private void Start()
        {

            var gen = GameObject.Find("WallCreator").GetComponent<GenerateGenerateWall>();

            _destroy = new List<GameObject>(); //This initializes the food object destroy list


            var activeGoals = E.Get().CurrTrial.Value.ActiveGoals;
            var inactiveGoals = E.Get().CurrTrial.Value.InactiveGoals;
            var invisibleGoals = E.Get().CurrTrial.Value.InvisibleGoals;
            var inactiveSet = new HashSet<int>(inactiveGoals);
            var invisibleSet = new HashSet<int>(invisibleGoals);
            var activeSet = new HashSet<int>(activeGoals);

            var merged = new List<int>();
            merged.AddRange(activeGoals);
            merged.AddRange(inactiveGoals);
            merged.AddRange(invisibleGoals);

            Data.Point p = new Data.Point { X = 0, Y = 0, Z = 0 };
            foreach (var val in merged)
            {

                var item = DS.GetData().Goals[Mathf.Abs(val) - 1];
                UnityEngine.Debug.Log(item);

                //Here is the text to determine the type of food that exists here




                if (item.Position.Count == 0)
                {
                    p = ReadFromExternal(item.PythonFile);

                }
                else
                {
                    try
                    {
                        p = new Data.Point { X = item.Position[0], Y = item.Position[2], Z = item.Position[1] };
                    }
                    catch (Exception _)
                    {
                        p = new Data.Point { X = item.Position[0], Y = 0.5f, Z = item.Position[1] };

                    }
                }




                var prefab = (GameObject)Resources.Load("prefabs/" + item.Type, typeof(GameObject));

                var obj = Instantiate(prefab);
                if (!item.Type.Equals("2DImageDisplayer"))
                    obj.AddComponent<RotateBlock>();
                obj.transform.Rotate(item.Rotation[0], item.Rotation[1], item.Rotation[2]);

                obj.AddComponent<PickupSound>();

                obj.GetComponent<PickupSound>().Sound = Resources.Load<AudioClip>("Audio/" + item.SoundLocation);

                // length, height, width
                obj.transform.localScale = new Vector3(item.Scale[0], item.Scale[2], item.Scale[1]);
                obj.transform.position = new Vector3(p.X, p.Y, p.Z);
                var sprite = item.ImageLoc;
                if (sprite != null)
                {
                    var pic = Img2Sprite.LoadNewSprite(Constants.InputDirectory + sprite);
                    obj.GetComponent<SpriteRenderer>().sprite = pic;
                }

                var color = Data.GetColour(item.Color);
                try
                {
                    obj.GetComponent<Renderer>().material.color = color;
                    obj.GetComponent<Renderer>().enabled = !invisibleSet.Contains(val);
                    obj.GetComponent<Collider>().enabled = !inactiveSet.Contains(val);
                    if (activeSet.Contains(val) || invisibleSet.Contains(val))
                    {
                        obj.tag = "Pickup";
                        obj.GetComponent<Collider>().isTrigger = true;
                    }
                }
                catch (Exception _)
                {
                    print("Visibility not working");
                }

                _destroy.Add(obj);
            }
            GameObject.Find("FirstPerson").GetComponent<PlayerController>().ExternalStart(p.X, p.Z);

        }

        //And here we destroy all the food.
        private void OnDestroy()
        {
            foreach (var t in _destroy)
            {
                if (t != null) Destroy(t);
            }
        }

    }
}