using UnityEngine.SceneManagement;

namespace trial
{
    public class ThreeDTrial : TimeoutableTrial
    {
        public ThreeDTrial(int blockId, int trialId) : base(blockId, trialId)
        {
        }

        public override void PreEntry(TrialProgress t, bool first = true)
        {
            //Sets the field of the preentry
            base.PreEntry(t, first);
            t.TimingVerification = data.DataSingleton.GetData().TimingVerification; // timing diagnostics
            t.EnvironmentType = Value.EnvironmentType;
            t.Sides = Value.Sides;
            t.BlockID = BlockID;
            t.TrialID = TrialID;
            t.TwoDim = Value.TwoDimensional;
            t.Instructional = 0;
            t.LastX = t.TargetX;
            t.LastY = t.TargetY;
            t.TargetX = 0;
            t.TargetY = 0;
            _runningTime -= Value.TimeToRotate;

            SceneManager.LoadScene(Value.EnvironmentType);
        }

        public override void Progress()
        {
            TrialProgress.Num3D++;

            // If we are progressing without a success, record the failure
            // as a zero, otherwise record a 1.
            if (isSuccessful)
            {
                TrialProgress.successes.Add(1);
            }
            else
            {
                TrialProgress.successes.Add(0);
            }
            isSuccessful = false;
            base.Progress();
        }

        public override void Notify()
        {
            // Record that this particular trial was a success
            TrialProgress.NumSuccess++;
            isSuccessful = true;

        }
    }
}