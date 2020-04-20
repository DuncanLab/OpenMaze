using data;

namespace contingency
{
    public interface IContingencyBehaviourValidator
    {
        bool ValidateContingencyBehaviour(Data.ContingencyBehaviour contingencyBehaviour);
    }
}