using System;
using System.IO;
using SFB;
using trial;
using UnityEngine;
using UnityEngine.UI;
using C = data.Constants;
using DS = data.DataSingleton;


namespace main
{
    /// <inheritdoc />
    /// <summary>
    ///     Main entry point of the app as well as the game object that stays alive for all scenes.
    /// </summary>
    public class Loader : MonoBehaviour
    {
        private static float _timer;

        // The current trial running in the game. Loader stores this as an instance field. 
        public AbstractTrial CurrTrial;

        public InputField[] Fields; //These are an array of the fields given from the field trials

        //Singleton function
        public static Loader Get()
        {
            return GameObject.Find("Loader").GetComponent<Loader>();
        }

        //Unity method
        private void Start()
        {
            DontDestroyOnLoad(this);
            LoadConfigFile();
            CurrTrial = new FieldTrial(Fields);
            //Initialize the default field trial, see this later.
        }
        
        
        private static void LoadConfigFile()
        {
            // This block of code will use the default configuration file if
            // using webGL or Android (can add others in if statement) 
            // Otherwise calls the directory picker to select the configuration file.

            var defaultConfigDir = Application.streamingAssetsPath + "/Default_Config/";
            var files = new string[0];
            if (Directory.Exists(defaultConfigDir)) files = Directory.GetFiles(defaultConfigDir);

            if (files.Length > 0)
            {
                var defaultConfig = defaultConfigDir + Path.GetFileName(files[0]);
                Loader.ExternalActivation(defaultConfig);
            }
            else
            {
                while (true)
                {
                    // Here we're using: https://github.com/gkngkc/UnityStandaloneFileBrowser because it was easier than rolling our own
                    var paths = StandaloneFileBrowser.OpenFilePanel("Choose configuration file", "Configuration_Files",
                        "", false);
                    if (paths.Length == 0)
                    {
                        Debug.Log("Empty config given, exiting application");
                        #if UNITY_EDITOR
                            UnityEditor.EditorApplication.isPlaying = false;
                        #endif
                        Application.Quit();
                        return;
                    }
                    var path = paths[0];
                    if (ExternalActivation(path)) break;
                }
            }
        }

        //This function initializes the Data.singleton files
        public static bool ExternalActivation(string inputFile)
        {
            if (!inputFile.Contains(".json"))
            {
                Debug.LogError("Invalid Json File");
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #endif
                return false;
            }

            DS.Load(inputFile);
            Directory.CreateDirectory(C.OutputDirectory);
            return true;
        }

        private void Update()
        {
            CurrTrial?.Update(Time.deltaTime);
        }

        public static void LogHeaders()
        {
            using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, false))
            {
                writer.Write(
                    "Participant, Session, Condition, Version, TimeStamp, BlockIndex, TrialIndex, TrialInBlock, Instructional, 2D, Scene, Enclosure, PositionX, PositionY, PositionZ, RotationY, " +
                    "Collision, GoalX, GoalZ, LastX, LastZ, UpArrow, DownArrow," +
                    " LeftArrow, RightArrow, Space"
                    + "\n");
                writer.Flush();
                writer.Close();
            }
        }

        public static void LogData(TrialProgress s, long trialStartTime, Transform t, int targetFound = 0)
        {
            if (Get().CurrTrial.IsLoading())
            {
                return;
            }
            
            // Don't output anything if the Y position is at default (avoids incorrect output data)
            if (t.position.y != -1000 && (targetFound == 1 || _timer >
                1f / (DS.GetData().OutputTimesPerSecond == 0
                    ? 1000
                    : DS.GetData().OutputTimesPerSecond)))
            {
                using (var writer = new StreamWriter("Assets/OutputFiles~/" + DS.GetData().OutputFile, true))
                {
                    var trialIdStr = "InTrial";
                    var PositionX = t.position.x.ToString();
                    var PositionZ = t.position.z.ToString();
                    var PositionY = t.position.y.ToString();
                    var RotationY = t.eulerAngles.y.ToString();
                    var LoadingTime = 0.0;
                    
                    if (s.Instructional == 1)
                    {
                        PositionX = "NA";
                        PositionZ = "NA";
                        PositionY = "NA";
                        RotationY = "NA";
                    }

                    var str = string.Format(
                        "{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, " +
                        "{12}, {13}, {14}, {15}, {16}, {17}, {18}, {19}, {20}, {21}, {22}, {23}, {24}, {25}",
                        s.Subject, s.Condition, s.SessionID, s.Note,
                        DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"), s.BlockId, s.TrialId, s.TrialNumber + 1,
                        s.Instructional, s.TwoDim, s.EnvironmentType, s.CurrentEnclosureIndex + 1, PositionX, PositionY,
                        PositionZ, RotationY,
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
