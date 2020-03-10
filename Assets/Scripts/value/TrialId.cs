namespace value
{
    public class TrialId : Id
    {
        public TrialId(int value) : base(value)
        {
        }
        
        public static readonly TrialId EMPTY = new TrialId(-1);

    }
}