using UnityEngine;

public class StartButton : MonoBehaviour
{
    public static bool clicked;

    private void LateUpdate()
    {
        clicked = false;
    }

    public void Click()
    {
        clicked = true;
    }
}