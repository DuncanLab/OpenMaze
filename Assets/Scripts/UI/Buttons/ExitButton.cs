using UnityEngine;
using E = main.Loader;

public class ExitButton : MonoBehaviour
{
    public static bool clicked;


    private void LateUpdate()
    {
        clicked = false;
        gameObject.SetActive(E.Get().CurrTrial.trialData.ExitButton);
    }

    public void Click()
    {
        clicked = true;
    }
}
