
using System.Collections;
using main;
using UnityEngine;

namespace states
{


    
    
    /// <summary>
    /// 
    /// We will transition from enums into a true State system
    /// 
    /// </summary>
    public class Event
    {
        //The state that we want the system to transition to (null if no transition)
        public readonly State State;
        protected Loader L;
        public Event(State st)
        {
            State = st;
            L = Loader.Get();
        }
        
        //Here we peform an action 
        public virtual void Act()
        {
            L.ResetEvent();
        }



    }
}
