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
                    "Timestamp, Trial Increment, Trial Number, X, Y, Angle, Environment Type, EnclosureIndex, TargetFound, PickupType, " +
                    "TargetX, TargetY, LastX, LastY, BlockID, TrialID, Participant, Condition, 2D, Instructional, Visible, UpArrow, DownArrow," +
                    " LeftArrow, RightArrow, Space, Session, Note"
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
                    var trialIdStr = (s.TrialID + 1).ToString();

                    var timeSinceExperimentStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - DataSingleton.GetData().ExperimentStartTime;
                    var timeSinceTrialStart = DateTimeOffset.Now.ToUnixTimeMilliseconds() - trialStartTime;

                    var str = string.Format(
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
                        "{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}",
                        DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), data.DataSingleton.GetData().TrialInitialValue, s.TrialNumber + 1, t.position.x, t.position.z, t.eulerAngles.y, s.EnvironmentType, s.CurrentEnclosureIndex,
                        targetFound, s.PickupType, s.TargetX, s.TargetY, s.LastX, s.LastY, s.BlockID + 1, trialIdStr,
                        s.Subject, s.Condition, s.TwoDim, s.Instructional, s.Visible, Input.GetKey(KeyCode.UpArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.DownArrow) ? 1 : 0, Input.GetKey(KeyCode.LeftArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.RightArrow) ? 1 : 0,
                        Input.GetKey(KeyCode.Space) ? 1 : 0, s.SessionID, s.Note);
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
