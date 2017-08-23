using System.Collections;
using System.Runtime.InteropServices;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace states
{
    
    public class TransitionEvent : Event
    {
        private float _targetTime;
        private AsyncOperation _loadLevel;
        private float _runTime;
        public TransitionEvent(int stage, State st = State.None, float time = 0) : base(st)
        {
            _loadLevel = SceneManager.LoadSceneAsync(stage);           
            _loadLevel.allowSceneActivation = false;

            _runTime = L.RunningTime;
            _targetTime = time;

        }
        
        public override void Act()
        {
            if (L.RunningTime - _runTime < _targetTime || _loadLevel.progress < .9f)
                return;
            
            
            _loadLevel.allowSceneActivation = true;
            L.ResetEvent();
        }
    }
}