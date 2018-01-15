using data;
using UnityEngine;

namespace trial
{
    public class ThreeDTrial : AbstractTrial
    {
        public ThreeDTrial(Data.Trial value) : base(value)
        {
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }
        
        //We guarantee that PreEntry wi
        public override void PreEntry()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            throw new System.NotImplementedException();
        }

        public override void Progress()
        {
            throw new System.NotImplementedException();
        }
    }
}