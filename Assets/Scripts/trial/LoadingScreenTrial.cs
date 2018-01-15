using System.Reflection;
using data;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;
using DS = data.DataSingleton;

namespace trial
{
    public class LoadingScreenTrial : AbstractTrial

    {
        private float _threshHold;
        
        public LoadingScreenTrial(int blockId, int trialId) : base(blockId, trialId)
        {
            _threshHold = Value.TimeAllotted == -1 ? int.MaxValue : Value.TimeAllotted;
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }
        
        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            if (Input.GetKey(KeyCode.Space) || deltaTime > _threshHold)
            {
                Progress();
            }
        }
        

    }
}