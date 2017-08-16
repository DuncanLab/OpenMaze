/// <summary>
/// This class is the central experiment 
/// </summary>
public class ExperimentManager
{
    
    //Singleton instance for Experiment Manager
    private static ExperimentManager _myInstance;

    /// <summary>
    /// Singleton Get function
    /// </summary>
    /// <returns>Singleton instance</returns> 
    public static ExperimentManager Get()
    {
        return _myInstance ?? (_myInstance = new ExperimentManager());
    }

    private ExperimentManager()
    {
        
    }


}
