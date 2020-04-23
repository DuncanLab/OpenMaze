namespace value
{
    public class TrialId : Id
    {
        public static readonly TrialId EMPTY = new TrialId(-1);

        public TrialId(int value) : base(value)
        {
        }
    }
}
