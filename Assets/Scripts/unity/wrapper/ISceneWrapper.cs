
using UnityEngine;

namespace unity.wrapper
{
    // Wrap all the APIs that we want to use in the wrapper service for testability.
    public interface ISceneWrapper
    {
        AsyncOperation LoadAsyncScene(string name);
        AsyncOperation LoadAsyncScene(int environmentNumber);
    }
}
