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
            
            _fields = fields;
        }


        //We are gonna generate all the trials here.
        private void GenerateTrials()
        {
            AbstractTrial currentTrial = this;
            
            foreach (int i in DS.GetData().BlockOrder)
            {
                var block = DS.GetData().BlockList[i];
                foreach (int j in block.TrialOrder)
                {
                    var trial = DS.GetData().TrialData[j];
                    //Here we decide what each trial is, I guess we could do this with a function map, but later. 
                    //here we have a picture as a trial.
                    if (trial.FileLocation != null)
                    {
                        new LoadingScreenTrial(i, j);
                    }
                    
                    
                }
            }
        }
        

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Input.GetKey(KeyCode.Space))
            {
                Loader.LogData("", false);

                foreach (var textBox in _fields)
                {
                    var arr = textBox.transform.GetComponentsInChildren<Text>();
                    Loader.LogData(arr[0].text + ": " + arr[1].text);
                    DS.GetData().CharacterData.OutputFile = arr[1].text + "_" + DS.GetData().CharacterData.OutputFile;
                }
                Progress();
            }
            
        }

        //We ignore the LogData in this case since it is not called in this trial
        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }
        
        
        public override void Progress()
        {
            throw new System.NotImplementedException();
        }
    }
}