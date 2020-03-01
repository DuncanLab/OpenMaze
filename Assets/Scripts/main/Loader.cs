using System;
using System.IO;
using data;
using trial;
using UnityEngine;
using UnityEngine.UI;
using C = data.Constants;
using DS = data.DataSingleton;



namespace main
{
    /// <inheritdoc />
    /// <summary>
    /// Main entry point of the app as well as the game object that stays alive for all scenes.
    /// </summary>
    public class Loader : MonoBehaviour
    {
        //Singleton function
        public static Loader Get()
        {
            return GameObject.Find("Loader").GetComponent<Loader>();
        }
        public InputField[] Fields; //These are an array of the fields given from the field trials

        private static float _timer = 0;

        //An "abstract trial". Classic polymorphism.
        public AbstractTrial CurrTrial;

        //Unity method
        private void Start()
        {
            DontDestroyOnLoad(this);
            CurrTrial = new FieldTrial(Fields);
            //Initialize the default field trial, see this later.
        }

        //This function initializes the Data.singleton files
        public static bool ExternalActivation(string inputFile)
        {
            if (!inputFile.Contains(".json")) return false;
            DS.Load(inputFile);
            Directory.CreateDirectory(C.OutputDirectory);
            return true;
        }

        private void Update()
        {
            CurrTrial.Update(Time.deltaTime);
        }

        public static void LogHeaders()
        {
            using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, false))
            {
                writer.Write(
                    "StartField1, StartField2, StartField3, StartField4, TimeStamp, BlockIndex, TrialIndex, TrialInBlock, Instructional, 2D, Scene, Enclosure, PositionX, PositionY, PositionZ, RotationY, " +
                    "Collision, GoalX, GoalZ, LastX, LastZ, UpArrow, DownArrow," +
                    " LeftArrow, RightArrow, Space"
                + "\n");
                writer.Flush();
                writer.Close();
            }
        }

        public static void LogData(TrialProgress s, long trialStartTime, Transform t, int targetFound = 0)
        {
            // Don't output anything if the Y position is at default (avoids incorrect output data)
            if (t.position.y != -1000 && (targetFound == 1 || _timer > 1f / (DS.GetData().OutputTimesPerSecond == 0 ? 1000 : DS.GetData().OutputTimesPerSecond)))
            {
                using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, true))
                {
                    var PositionX = t.position.x.ToString();
                    var PositionZ = t.position.z.ToString();
                    var PositionY = t.position.y.ToString();
                    var RotationY = t.eulerAngles.y.ToString();
                    
                    var timeSinceExperimentStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - DataSingleton.GetData().ExperimentStartTime;
                    var timeSinceTrialStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - trialStartTime;

                    if(s.Instructional == 1)
                    {
                        PositionX = "NA";
                        PositionZ = "NA";
                        PositionY = "NA";
                        RotationY = "NA";
                    }

                    var str = string.Format(
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
                        "{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}",
                        s.Field1, s.Field2, s.Field3, s.Field4, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), s.BlockID + 1, s.TrialID + 1, s.TrialNumber + 1, s.Instructional, s.TwoDim, s.EnvironmentType, s.CurrentEnclosureIndex + 1, PositionX, PositionY, PositionZ, RotationY,
                        targetFound, s.TargetX, s.TargetY, s.LastX, s.LastY,
                        Input.GetKey(KeyCode.UpArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.DownArrow) ? 1 : 0, Input.GetKey(KeyCode.LeftArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.RightArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.Space) ? 1 : 0);
                    writer.Write(str + "\n");
                    writer.Flush();
                    writer.Close();
                }
                _timer = 0;
            }
            _timer += Time.deltaTime;
        }
    }
}
