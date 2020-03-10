using main;
using SFB;
using System;
using data;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using value;
using System.IO;

using Debug = UnityEngine.Debug;
using DS = data.DataSingleton;

namespace trial
{
    // This is the only trial in which the current data is null. This is so that we have uniform setup
    public class FieldTrial : AbstractTrial
    {
        private readonly InputField[] _fields;

        // Here we construct the entire linked list structure.
        public FieldTrial(InputField[] fields) : base(BlockId.EMPTY, TrialId.EMPTY)
        {
            _fields = fields;

            // This block of code will use the default configuration file if
            // using webGL or Android (can add others in if statement) 
            // Otherwise calls the directory picker to select the configuration file.
    
            var defaultConfigDir = Application.streamingAssetsPath + "/Default_Config/";
            var files = new string[0];
            if (Directory.Exists(defaultConfigDir))
            {
                files = Directory.GetFiles(defaultConfigDir);
            }

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
                    string[] paths = StandaloneFileBrowser.OpenFilePanel("Choose configuration file", "Configuration_Files", "", false);
                    string path = paths[0];
                    if (Loader.ExternalActivation(path)) break;
                }
            }

            TrialProgress = new TrialProgress();
            _fields = fields;

        }


        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            AbstractTrial currentTrial = this;
            foreach (var blockDisplayIndex in DS.GetData().BlockOrder)
            {
                var blockId = new BlockId(blockDisplayIndex);
                var block = DS.GetData().Blocks[blockId.Value];
                var newBlock = true;
                AbstractTrial currHead = null;

                var trialCount = 0;
                foreach (var trialDisplayIndex in block.TrialOrder)
                {
                    AbstractTrial newTrial;
                        var trialId = new TrialId(trialDisplayIndex);
                        switch (trialId.Value)
                        {
                            // Here we decide what each trial is, I guess we could do this with a function map, but later. 
                            // here we have a picture as a trial.
                            case -1:
                                newTrial = new RandomTrial(blockId);
                                break;
                            case -2:
                                newTrial = null;
                                break;
                            default:
                                newTrial = TrialUtils.GenerateBasicTrialFromConfig(blockId, trialId, trialData);
                                break;
                        }
                    
                    

                    if (newBlock) currHead = newTrial;

                    newTrial.isTail = trialCount == block.TrialOrder.Count - 1;
                    newTrial.head = currHead;

                    currentTrial.next = newTrial;

                    currentTrial = currentTrial.next;

                    newBlock = false;
                    trialCount++;
                }

                currentTrial.next = new CloseTrial();
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (StartButton.clicked == true)
            {
                // Sets the output file name as the desired one.
                var Field1Text = _fields[0].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Field1 = Field1Text.text;

                var Field2Text = _fields[1].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Field2 = Field2Text.text;

                var Field3Text = _fields[2].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Field3 = Field3Text.text;

                var Field4Text = _fields[3].transform.GetComponentsInChildren<Text>()[1];
                TrialProgress.Field4 = Field4Text.text;

                DS.GetData().OutputFile = TrialProgress.Field1 + "_" +
                                          TrialProgress.Field2 + "_" +
                                          TrialProgress.Field3 + "_" +
                                          TrialProgress.Field4 + "_" + 
                                          DateTime.Now.ToString("yyyy-MM-dd-HH.mm.ss") + ".csv";

                GenerateTrials();

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