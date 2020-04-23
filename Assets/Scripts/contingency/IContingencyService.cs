using trial;

namespace contingency
{
    public interface IContingencyService
    {
        // Returns the trial that we should go to
        AbstractTrial ExecuteContingency(TrialProgress tp);
    }
}
