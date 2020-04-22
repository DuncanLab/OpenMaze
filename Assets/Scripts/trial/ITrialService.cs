using System.Collections.Generic;
using data;
using value;

namespace trial
{
    public interface ITrialService
    {
        void GenerateAllStartingTrials(AbstractTrial root);
        AbstractTrial GenerateBasicTrialFromConfig(BlockId blockId, TrialId trialId, Data.Trial targetTrialData);
    }
}