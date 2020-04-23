using data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Timing
{
    public class TimingSquare : MonoBehaviour
    {
        private void Start()
        {
            // testing for timing settings. 
            Debug.Log("Timing Status: " + DataSingleton.GetData().TimingVerification);

            // set properties of timing box
            var timingBox = GameObject.Find("TimingUnit").GetComponent<Graphic>();
            timingBox.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);

            // if timing diagnostics are turned on then timing square will initialize with each scene. 
            if (DataSingleton.GetData().TimingVerification)
            {
                timingBox.enabled = true;
                if (DataSingleton.GetData().TrialInitialValue % 2 == 0)
                {
                    Debug.Log("Changing square to black");
                    timingBox.color = Color.black;
                }
                else
                {
                    Debug.Log("Changing square to white");
                    timingBox.color = Color.white;
                }
            }
            else
            {
                timingBox.enabled = false;
            }
        }
    }
}
