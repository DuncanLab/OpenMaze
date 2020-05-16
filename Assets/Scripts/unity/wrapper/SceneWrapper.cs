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
            return _sceneWrapper ?? (_sceneWrapper = new SceneWrapper());
        }
        
        
        public AsyncOperation LoadAsyncScene(string name)
        {
            return SceneManager.LoadSceneAsync(name);
        }
        
        public AsyncOperation LoadAsyncScene(int environmentNumber)
        {
            return SceneManager.LoadSceneAsync(environmentNumber);
        }
    }
}
