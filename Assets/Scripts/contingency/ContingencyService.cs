using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using contingency.reflection;
using data;
using main;
using trial;
using UnityEngine;
using value;

namespace contingency
{
    /**
     * This class is responsible for handling the new Contingency system.
     *
     * An unique instance of this service will be bound to every abstract trial.
     * If a trial does not have an associated ContingencyBehaviour, every operation within this service will be
     * a no-op.
     */
    public class ContingencyService : IContingencyService
    {
        private readonly AbstractTrial _abstractTrial;
        private readonly Data.Contingency _contingency;

        private readonly IContingencyBehaviourValidator _contingencyBehaviourValidator;
        private readonly IContingencyFunctionCaller _contingencyFunctionCaller;
        private readonly Data _data;
        private readonly ITrialService _trialService;

        private ContingencyService()
        {
        }

        private ContingencyService(
            IContingencyBehaviourValidator contingencyBehaviourValidator,
            IContingencyFunctionCaller contingencyFunctionCaller,
            ITrialService trialService,
            Data data,
            Data.Contingency contingency,
            AbstractTrial abstractTrial)
        {
            _contingencyBehaviourValidator = contingencyBehaviourValidator;
            _contingencyFunctionCaller = contingencyFunctionCaller;
            _trialService = trialService;
            _data = data;
            _contingency = contingency;
            _abstractTrial = abstractTrial;
        }

        public AbstractTrial ExecuteContingency(TrialProgress tp)
        {
            if (!HasContingency())
                // If there is no contingency, the trial should just progress to the next one.
                return _abstractTrial.next;

            var contingencyRes = _contingencyFunctionCaller.InvokeContingencyFunction(tp, _contingency);
            
            if (contingencyRes == null || !_contingency.BehaviourByResult.ContainsKey(contingencyRes))
            {
                Debug.LogError($"{contingencyRes} does not exist as a result for trial " +
                               $"{_abstractTrial.TrialId} in block {_abstractTrial.BlockId}");
                Application.Quit();
                return null;
            }

            var behaviour = _contingency.BehaviourByResult[contingencyRes];

            if (!_contingencyBehaviourValidator.ValidateContingencyBehaviour(behaviour))
            {
                Debug.LogError($"{contingencyRes} lead to an invalid contingencyBehaviour for " +
                               $"{_abstractTrial.TrialId} in block {_abstractTrial.BlockId}");
                Application.Quit();
                return null;
            }

            var curr = _abstractTrial;

            // Remove all generated trials before executing the behaviour
            while (curr.next.IsGenerated) curr = curr.next;
            _abstractTrial.next = curr.next;


            if (behaviour.NextTrials != null)
            {
                AddTrials(behaviour);
                return _abstractTrial.next;
            }

            if (behaviour.EndBlock)
            {
                curr = _abstractTrial;
                while (!curr.isTail) curr = curr.next;
                return curr.next;
            }

            if (behaviour.RepeatContingency) return _abstractTrial.SourceTrial;

            if (behaviour.RestartBlock) return _abstractTrial.head;

            // Other behaviours not implemented yet.
            throw new NotImplementedException();
        }


        private bool HasContingency()
        {
            return _contingency != null;
        }

        private void AddTrials(Data.ContingencyBehaviour behaviour)
        {
            var curr = _abstractTrial;
            var next = curr.next;


            foreach (var trialId in behaviour.NextTrials.Select(trialIdx => new TrialId(trialIdx)))
            {
                var trial = _trialService.GenerateBasicTrialFromConfig(_abstractTrial.BlockId, trialId,
                    _data.Trials[trialId.Value]);
                trial.IsGenerated = true;
                trial.SourceTrial = _abstractTrial.SourceTrial;
                trial.head = _abstractTrial.SourceTrial.head;
                curr.next = trial;
                curr = curr.next;
            }

            _trialService.AddContingencyServiceToTrial(curr);
            curr.next = next;
        }

        public class ContingencyServiceFactory : IContingencyServiceFactory
        {
            private ContingencyServiceFactory()
            {
            }

            // This generator function is called when a trial doesn't have an associated contingency
            public IContingencyService CreateEmpty()
            {
                return new ContingencyService();
            }

            public IContingencyService Create(Data.Contingency contingency, AbstractTrial abstractTrial)
            {
                return new ContingencyService(
                    ContingencyBehaviourValidator.Create(),
                    ContingencyFunctionCaller.Create(),
                    TrialService.Create(),
                    DataSingleton.GetData(),
                    contingency,
                    abstractTrial
                );
            }

            public static IContingencyServiceFactory Create()
            {
                return new ContingencyServiceFactory();
            }
        }
    }
}
