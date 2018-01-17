using System.Reflection;
using data;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;

namespace trial
{
    public class LoadingScreenTrial : TimeoutableTrial

    {

        public LoadingScreenTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }



        public override void PreEntry(TrialProgress t, bool first = true)
        {
            base.PreEntry(t, first);
            SceneManager.LoadScene(Constants.LoadingScreen);
            
        }

        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Input.GetKey(KeyCode.Space))
            {
                Progress();
            }
        }

    }
}