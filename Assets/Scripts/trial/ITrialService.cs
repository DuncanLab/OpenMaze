using data;
using value;

namespace trial
{
    public interface ITrialService
    {
        void AddContingencyServiceToTrial(AbstractTrial trial);
        void GenerateAllStartingTrials(AbstractTrial root);
        AbstractTrial GenerateBasicTrialFromConfig(BlockId blockId, TrialId trialId, Data.Trial targetTrialData);
    }
}
