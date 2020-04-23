using System;
using System.IO;
using main;
using SFB;
using UnityEngine;
using UnityEngine.UI;
using value;
using DS = data.DataSingleton;

namespace trial
{
    // This trial is loaded at the beginning. This is the Entry point of the application.
    public class FieldTrial : AbstractTrial
    {
        private readonly InputField[] _fields;
        private readonly ITrialService _trialService;

        // Here we construct the entire linked list structure.
        public FieldTrial(InputField[] fields) : base(null, BlockId.EMPTY, TrialId.EMPTY)
        {
            _fields = fields;

            LoadConfigFile();

            TrialProgress = new TrialProgress();
            _fields = fields;
            _trialService = TrialService.Create();
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
                    var path = paths[0];
                    if (Loader.ExternalActivation(path)) break;
                }
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (StartButton.clicked)
            {
                // Sets the output file name as the desired one.
                var subjectTextField = _fields[0].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Subject = subjectTextField.text;

                var sessionTextField = _fields[1].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.SessionID = sessionTextField.text;

                var conditionTextField = _fields[2].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Condition = conditionTextField.text;

                var noteTextField = _fields[3].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Note = noteTextField.text;

                DS.GetData().OutputFile = TrialProgress.Subject + "_" +
                                          TrialProgress.SessionID + "_" +
                                          TrialProgress.Condition + "_" +
                                          TrialProgress.Note + "_" +
                                          DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss") + ".csv";

                _trialService.GenerateAllStartingTrials(this);

                Loader.LogHeaders();

                Progress();
            }
        }

        public override void Progress()
        {
            Loader.Get().CurrTrial = next;
            next.PreEntry(TrialProgress);
        }
    }
}
