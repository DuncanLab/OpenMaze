using UnityEngine;
using E = main.Loader;

namespace UI.Buttons
{
    public class ExitButton : MonoBehaviour
    {
        public void Update()
        {
            if (E.Get().CurrTrial.trialData != null)
            {
                gameObject.SetActive(E.Get().CurrTrial.trialData.ExitButton);
            }
        }
        
        public void Click()
        {
            Application.Quit();
        }
    }
}
