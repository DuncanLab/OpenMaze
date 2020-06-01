namespace loading
{
    public interface ILoadingService
    {
        bool IsLoading();
        void TransitionNextSceneWithDelay(int sceneNumber);
    }
}
