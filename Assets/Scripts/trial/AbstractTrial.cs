using data;
using UnityEngine;

namespace trial
{
    //A central trial class 
    public abstract class AbstractTrial
    {
        public Data.Trial Value;
        
        //This points to the start of the block of trials (if present)
        private AbstractTrial head;
        
        //This points to the tail of the block of trials
        private AbstractTrial tail;

        //This points to the next trial
        private AbstractTrial next;


        protected AbstractTrial(Data.Trial value)
        {
            Value = value;
        }
        
        
        public abstract void LogData(Transform t, bool collided = false);

        public abstract void PreEntry();

        public abstract void Update(float deltaTime);
        
        
        //Standard progression
        public abstract void Progress();
    }
}