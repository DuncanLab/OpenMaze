using System;
using System.Collections;
using data;
using unity.wrapper;
using UnityEngine;

namespace loading
{
    public class LoadingService : ILoadingService
    {
        private readonly Data _data;
        private static ILoadingService _loadingService;
        private readonly ISceneWrapper _sceneWrapper;
        
        // Singleton service
        public static ILoadingService Create()
        {
            return _loadingService ?? (_loadingService = new LoadingService(
                DataSingleton.GetData(),
                SceneWrapper.Create()));
        }

        private LoadingService(Data data, ISceneWrapper sceneWrapper)
        {
            _data = data;
            _sceneWrapper = sceneWrapper;
        }

        public void TransitionNextSceneWithDelay(int sceneNumber)
        {
            var entryTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _data.ExperimentStartTime;
            _sceneWrapper.LoadScene("StopGapLoad");
            
            var scenePromise = _sceneWrapper.LoadAsyncScene(sceneNumber);
            scenePromise.allowSceneActivation = false;
            _sceneWrapper.SwitchScene(BeginLoad(scenePromise, entryTime));
        }

        /**
         * This function will try to load a scene
         */
        private IEnumerator BeginLoad(AsyncOperation scenePromise, long entryTime)
        {
            
            var loadingDelta = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _data.ExperimentStartTime - entryTime;
            while (!scenePromise.isDone || loadingDelta < _data.MinLoadMsDelay)
            {
                loadingDelta = DateTimeOffset.Now.ToUnixTimeMilliseconds() - _data.ExperimentStartTime - entryTime;
                yield return null;
            }

            if (loadingDelta > _data.MinLoadMsDelay * 1.1)
            {
                if (_data.IgnoreMinLoadMsDelay)
                {
                    Debug.Log("Note, your MinLoadMsDelay has been reached and the flag " +
                              "IgnoreMinLoadMsDelay has been set. " +
                              "If this is unintended, please re-tune the value in your config file.");
                }
                else
                {

                    Debug.LogError("The minimum loading delay has been exceeded. The current delay is " +
                                   $"{_data.MinLoadMsDelay} but the loading took {loadingDelta}. Please re-tune " +
                                       "LoadMinMsDelay in the config file or set the flag IgnoreLoadMinMsDelay to true");
                    Application.Quit();
                }

            }
            
            scenePromise.allowSceneActivation = true;
        }
    }
}
