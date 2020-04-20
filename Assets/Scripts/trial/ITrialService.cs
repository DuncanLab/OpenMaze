using System.Collections.Generic;
using data;
using value;

namespace trial
{
    public interface ITrialService
    {
        AbstractTrial GenerateBasicTrialFromConfig(BlockId blockId, TrialId trialId, Data.Trial trialDataFromIndex);

        void GenerateAllStartingTrials(AbstractTrial root);
    }
}