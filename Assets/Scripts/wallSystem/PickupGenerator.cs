using System;
using System.Collections.Generic;
using System.Diagnostics;
using data;
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
            var scriptPath = DataSingleton.GetData().PythonScriptsPath + inputFile;
            var p = new Process
            {
                StartInfo = new ProcessStartInfo("python", "\"" + scriptPath + "\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            p.Start();
            p.StandardInput.Write(JsonUtility.ToJson(E.Get().CurrTrial.trialData) + "\n");

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

            var activeGoals = E.Get().CurrTrial.trialData.ActiveGoals;
            var inactiveGoals = E.Get().CurrTrial.trialData.InactiveGoals;
            var invisibleGoals = E.Get().CurrTrial.trialData.InvisibleGoals;
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
                var goalItem = DS.GetData().Goals[Mathf.Abs(val) - 1];
                UnityEngine.Debug.Log(goalItem);

                // Position is not set in the config file
                if (goalItem.Position.Count == 0)
                {
                    p = ReadFromExternal(goalItem.PythonFile);
                }
                else
                {
                    try
                    {
                        p = new Data.Point { X = goalItem.PositionVector.x, Y = goalItem.PositionVector.y, Z = goalItem.PositionVector.z };
                    }
                    catch (Exception _)
                    {
                        p = new Data.Point { X = goalItem.PositionVector.x, Y = 0.5f, Z = goalItem.PositionVector.z };

                    }
                }

                GameObject prefab;
                GameObject obj;
                var spriteName = "";

                if (goalItem.Type.ToLower().Equals("3d"))
                {
                    prefab = (GameObject)Resources.Load("3D_Objects/" + goalItem.Object, typeof(GameObject));
                    obj = Instantiate(prefab);
                    obj.AddComponent<RotateBlock>();
                }
                else
                {
                    // Load the "2D" prefab here, so we have the required components
                    prefab = (GameObject)Resources.Load("3D_Objects/" + goalItem.Type.ToUpper(), typeof(GameObject));
                    obj = Instantiate(prefab);
                    spriteName = goalItem.Object;
                }

                obj.transform.Rotate(goalItem.RotationVector);
                obj.transform.localScale = goalItem.ScaleVector;
                obj.transform.position = new Vector3(p.X, p.Y, p.Z);

                obj.AddComponent<PickupSound>();
                obj.GetComponent<PickupSound>().Sound = Resources.Load<AudioClip>("Sounds/" + goalItem.Sound);

                if (!string.IsNullOrEmpty(spriteName))
                {
                    var pic = Img2Sprite.LoadNewSprite(DataSingleton.GetData().SpritesPath + spriteName);
                    obj.GetComponent<SpriteRenderer>().sprite = pic;
                }

                var color = Data.GetColour(goalItem.Color);

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

            GameObject.Find("Participant").GetComponent<PlayerController>().ExternalStart(p.X, p.Z);
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