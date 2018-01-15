using System.Collections.Generic;
using data;
using JetBrains.Annotations;
using UnityEngine;

namespace trial
{
    public class RandomTrial : AbstractTrial
    {
        private bool replacement;


        private List<AbstractTrial> pool;

        public RandomTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void LogData(Transform t, bool collided = false)
        {
            throw new System.NotImplementedException();
        }

        public override void PreEntry(TrialProgress t)
        {
            throw new System.NotImplementedException();
        }

        

    }
}