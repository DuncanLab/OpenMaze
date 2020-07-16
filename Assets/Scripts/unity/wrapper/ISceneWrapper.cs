
using System.Collections;
using main;
using UnityEngine;

namespace unity.wrapper
{
    // Wrap all the APIs that we want to use in the wrapper service for testability.
    public interface ISceneWrapper
    {
        void LoadScene(string name);

        void LoadScene(int sceneNumber);

        AsyncOperation LoadAsyncScene(int environmentNumber);
        void SwitchScene(IEnumerator loadAction);

        void FreezeTime();

        void RestartTime();
    }
}
