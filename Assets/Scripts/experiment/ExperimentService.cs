namespace experiment
{
    public class ExperimentService : IExperimentService
    {
        
        
        private ExperimentService()
        {
            
        }
        
        // Singleton instance for the ExperimentService
        public class ExperimentServiceFactory : IExperimentServiceFactory
        {
            private static IExperimentService _service;
            
            private ExperimentServiceFactory(){}
            
            public IExperimentService GetExperimentService()
            {
                return _service ?? (_service = new ExperimentService());
            }

            public static IExperimentServiceFactory Create()
            {
                return new ExperimentServiceFactory();   
            }
        }
    }
}
