using System.Linq;
using data;

namespace contingency
{
    // This class will validate the contingency section of a block.
    // A contingency is valid iff exactly 1 value is set to true
    // This will be coded with reflection.
    public class ContingencyBehaviourValidator : IContingencyBehaviourValidator
    {
        private ContingencyBehaviourValidator()
        {
        }

        public bool ValidateContingencyBehaviour(Data.ContingencyBehaviour contingencyBehaviour)
        {
            // Check if NextTrials is null first.
            var numTrueProps = contingencyBehaviour.NextTrials == null ? 0 : 1;

            numTrueProps += contingencyBehaviour.GetType()
                .GetFields()
                .Where(e => e.FieldType == false.GetType())
                .Select(contingencyFlag => (bool) contingencyFlag.GetValue(contingencyBehaviour))
                .Select(isFieldTrue => isFieldTrue ? 1 : 0)
                .Sum();

            return numTrueProps == 1;
        }

        public static IContingencyBehaviourValidator Create()
        {
            return new ContingencyBehaviourValidator();
        }
    }
}
