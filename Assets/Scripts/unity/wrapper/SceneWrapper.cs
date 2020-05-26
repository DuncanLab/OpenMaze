using System.Collections;
using main;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace unity.wrapper
{
    public class SceneWrapper : ISceneWrapper
    {
        private static ISceneWrapper _sceneWrapper;
        
        // Singleton service
        public static ISceneWrapper Create()
        {
            return _sceneWrapper ?? (_sceneWrapper = new SceneWrapper(Loader.Get()));
        }
        
        private readonly Loader _loader;

        private SceneWrapper(Loader loader)
        {
            _loader = loader;
        }

        public void FreezeTime()
        {
            Time.timeScale = 0;
        }

        public void RestartTime()
        {
            Time.timeScale = 1;
        }
        
        public void SwitchScene(IEnumerator loadAction)
        {
            _loader.StartCoroutine(loadAction);
        }
        
        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }
        
        public AsyncOperation LoadAsyncScene(int environmentNumber)
        {
            return SceneManager.LoadSceneAsync(environmentNumber);
        }
    }
}
