using System.IO;
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

        //The top of the csv
        public static void LogFirst()
        {
            using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, false))
            {
                writer.Write(
                    "Trial Increment, Trial Number, Time Since Experiement Start (Seconds), Time Since Trial Start (seconds), X, Y, Angle, Environment Type, Sides, TargetFound, PickupType, " +
                    "TargetX, TargetY, LastX, LastY, BlockID, TrialID, Subject, Delay, 2D, Instructional, Visible, UpArrow, DownArrow," +
                    " LeftArrow, RightArrow, Space, Session, Note"
                + "\n");
                writer.Flush();
                writer.Close();
            }
        }

        //Logs the data out to the csv.
        public static void LogData(TrialProgress s, float timestamp, Transform t, int targetFound = 0)
        {
            if (_timer > 1f / (DS.GetData().OutputTimesPerSecond == 0 ? 1000 : DS.GetData().OutputTimesPerSecond) || targetFound == 1)
            {
                using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, true))
                {
                    var trialIdStr = s.TrialID.ToString();

                    // If we're in a loading delay period we want to output that
                    // instead of TrialID;
                    if (s.isLoaded == false)
                    {
                        trialIdStr = "Loading Delay";
                    }

                    var str = string.Format(
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
                        "{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}, {26}",
                        data.DataSingleton.GetData().TrialInitialValue, s.TrialNumber, s.TimeSinceExperimentStart, timestamp, t.position.x, t.position.z, t.eulerAngles.y, s.EnvironmentType, s.CurrentMazeName,
                        targetFound, s.PickupType, s.TargetX, s.TargetY, s.LastX, s.LastY, s.BlockID, trialIdStr,
                        s.Subject, s.Delay, s.TwoDim, s.Instructional, s.Visible, Input.GetKey(KeyCode.UpArrow) ? 1 : 0,
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
