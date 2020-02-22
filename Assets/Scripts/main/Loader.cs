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
                    "StartField1, StartField2, StartField3, StartField4, TimeStamp, TrialState, TrialTime, LoadingTime, BlockIndex, TrialIndex, TrialInBlock, Instructional, 2D, Scene, Enclosure, PositionX, PositionZ, RotationY, " +
                    "Collision, GoalX, GoalZ, LastX, LastZ, UpArrow, DownArrow," +
                    " LeftArrow, RightArrow, Space"
                + "\n");
                writer.Flush();
                writer.Close();
            }
        }

        public static void LogData(TrialProgress s, long trialStartTime, Transform t, int targetFound = 0)
        {
            if (targetFound == 1 || _timer > 1f / (DS.GetData().OutputTimesPerSecond == 0 ? 1000 : DS.GetData().OutputTimesPerSecond))
            {
                using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, true))
                {
                    var trialIdStr = "InTrial";
                    var PositionX = t.position.x.ToString();
                    var PositionZ = t.position.z.ToString();
                    var RotationY = t.eulerAngles.y.ToString();
                    var LoadingTime = 0.0;
                    
                    var timeSinceExperimentStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - DataSingleton.GetData().ExperimentStartTime;
                    var timeSinceTrialStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - trialStartTime;

                    if(s.Instructional == 1)
                    {
                        PositionX = "NA";
                        PositionZ = "NA";
                        RotationY = "NA";
                    }

                    var str = string.Format(
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
                        "{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}, {27}",
                        s.Subject, s.Condition, s.SessionID, s.Note, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), trialIdStr, DateTimeOffset.Now.ToUnixTimeMilliseconds() - trialStartTime, LoadingTime, s.BlockID + 1, s.TrialID + 1, s.TrialNumber + 1, s.Instructional, s.TwoDim, s.EnvironmentType, s.CurrentEnclosureIndex + 1, PositionX, PositionZ, RotationY,
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
