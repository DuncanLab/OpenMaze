using System.Collections.Generic;
using data;
using UnityEngine;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {
        private bool replacement;


        private List<AbstractTrial> pool;
        
        public RandomTrial(Data.Trial value) : base(value)
        {
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }

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