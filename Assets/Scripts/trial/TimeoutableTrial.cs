using Gaia.FullSerializer;
using JetBrains.Annotations;
using UnityEngine;

namespace trial
{
    public abstract class TimeoutableTrial : AbstractTrial
    {
        
        private readonly float _threshHold;

        protected TimeoutableTrial(int blockId, int trialId) : base(blockId, trialId)
        {
            _threshHold = Value.TimeAllotted == -1 ? int.MaxValue : Value.TimeAllotted;

        }
        
        //Code for a trial to continue
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            
            if (_runningTime > _threshHold)
            {
                Debug.Log(_runningTime);
                Progress();
            }
        }
    }
}