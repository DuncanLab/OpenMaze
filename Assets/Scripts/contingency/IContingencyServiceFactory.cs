using data;
using trial;

namespace contingency
{
    public interface IContingencyServiceFactory
    {
        IContingencyService CreateEmpty();

        IContingencyService Create(Data.Contingency contingency, AbstractTrial abstractTrial);
    }
}
