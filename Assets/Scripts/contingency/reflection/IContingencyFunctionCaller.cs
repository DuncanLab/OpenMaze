using data;
using trial;

namespace contingency.reflection
{
    public interface IContingencyFunctionCaller
    {
        string InvokeContingencyFunction(TrialProgress tp, Data.Contingency contingency);
    }
}
