using main;
using SFB;
using System;
using UnityEngine;
using UnityEngine.UI;

using Debug = UnityEngine.Debug;
using DS = data.DataSingleton;

namespace trial
{
    // This is the only trial in which the current data is null. This is so that we have uniform setup
    public class FieldTrial : AbstractTrial
    {
        private readonly InputField[] _fields;

        // Here we construct the entire linked list structure.
        public FieldTrial(InputField[] fields) : base(-1, -1)
        {
            _fields = fields;

            // This block of code basically calls the directory picker to select the configuration file.
            while (true)
            {
                // Here we're using: https://github.com/gkngkc/UnityStandaloneFileBrowser because it was easier than rolling our own
                string[] paths = StandaloneFileBrowser.OpenFilePanel("Choose configuration file", "Configuration_Files", "", false);
                string path = paths[0];
                if (Loader.ExternalActivation(path)) break;
            }

            TrialProgress = new TrialProgress();
            _fields = fields;

        }


        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            AbstractTrial currentTrial = this;
            foreach (var i in DS.GetData().BlockOrder)
            {
                var l = i - 1;
                var block = DS.GetData().Blocks[l];
                var newBlock = true;
                AbstractTrial currHead = null;

                var tCnt = 0;
                foreach (var j in block.TrialOrder)
                {
                    var k = j - 1;
                    AbstractTrial t;

                    // Here we decide what each trial is, I guess we could do this with a function map, but later. 
                    // here we have a picture as a trial.
                    if (k < 0)
                    {
                        t = new RandomTrial(l, k);
                    }
                    else
                    {
                        var trialData = DS.GetData().Trials[k];

                        // Control flow here is for deciding what Trial gets spat out from the config
                        if (trialData.FileLocation != null)
                        {
                            Debug.Log("Creating new Instructional Trial");
                            t = new InstructionalTrial(l, k);
                        }
                        else if (trialData.TwoDimensional == 1)
                        {
                            Debug.Log("Creating new 2D Screen Trial");
                            t = new TwoDTrial(l, k);
                        }
                        else
                        {
                            Debug.Log("Creating new 3D Screen Trial");
                            t = new ThreeDTrial(l, k);
                        }
                    }
                    if (newBlock) currHead = t;

                    t.isTail = tCnt == block.TrialOrder.Count - 1;
                    t.head = currHead;

                    currentTrial.next = t;

                    currentTrial = currentTrial.next;

                    newBlock = false;
                    tCnt++;
                }

                currentTrial.next = new CloseTrial(-1, -1);
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0))
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