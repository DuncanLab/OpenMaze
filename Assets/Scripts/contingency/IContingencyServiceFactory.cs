using data;
using trial;

namespace contingency
{
    public interface IContingencyServiceFactory
    {
        IContingencyService CreateEmpty(AbstractTrial abstractTrial);

        IContingencyService Create(Data.Contingency contingency, AbstractTrial abstractTrial);
    }
}
