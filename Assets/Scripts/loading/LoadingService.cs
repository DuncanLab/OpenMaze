namespace loading
{
    public class LoadingService : ILoadingService
    {
        public static ILoadingService Create()
        {
            return new LoadingService();
        }
    }
}
