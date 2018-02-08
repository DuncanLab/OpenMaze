using System;
using data;
using main;
using UnityEngine;
using UnityEngine.UI;
using DS = data.DataSingleton;

namespace trial
{
    //This is the only trial in which the current data is null. This is so that we have uniform setup
    public class FieldTrial : AbstractTrial
    {
        private readonly InputField[] _fields;
        
        //Here we construct the entire linked list structure.
        public FieldTrial(InputField [] fields) : base(-1, -1)
        {
            TrialProgress = new TrialProgress();
            _fields = fields;
            GenerateTrials();
        }

        
        
        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            AbstractTrial currentTrial = this;
            foreach (var i in DS.GetData().BlockOrder)
            {
                var block = DS.GetData().BlockList[i];
                var newBlock = true;
                AbstractTrial currHead = null;
                
                var tCnt = 0;
                foreach (var j in block.TrialOrder)
                {
                    AbstractTrial t;

                    //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                    //here we have a picture as a trial.
                    if (j < 0)
                    {
                        t = new RandomTrial(i, j);
                    }
                    else
                    {
                        var trialData = DS.GetData().TrialData[j];

                        //Control flow here is for deciding what Trial gets spat out from the config

                        if (trialData.FileLocation != null)
                        {
                            Debug.Log("Creating new Loading Screen Trial");
                            t = new LoadingScreenTrial(i, j);
                        }
                        else if (trialData.TwoDimensional == 1)
                        {
                            Debug.Log("Creating new 2D Screen Trial");

                            t = new TwoDTrial(i, j);
                        }
                        else
                        {
                            Debug.Log("Creating new 3D Screen Trial");

                            t = new ThreeDTrial(i, j);
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
                
                //Sets the output file name as the desired one.
                foreach (var textBox in _fields)
                {
                    var arr = textBox.transform.GetComponentsInChildren<Text>();
                    DS.GetData().CharacterData.OutputFile = arr[1].text + "_" + DS.GetData().CharacterData.OutputFile;
                }
                var str  = _fields[0].transform.GetComponentsInChildren<Text>();
                TrialProgress.Subject = str[1].text;
                
                str = _fields[2].transform.GetComponentsInChildren<Text>();
                TrialProgress.Delay = str[1].text;
                
                
                
                str = _fields[1].transform.GetComponentsInChildren<Text>();
                TrialProgress.SessionID = str[1].text;
                
                
                str = _fields[3].transform.GetComponentsInChildren<Text>();
                TrialProgress.Note = str[1].text;
                
                
                
                Loader.LogFirst();

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