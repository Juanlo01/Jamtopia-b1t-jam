using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    // wired directly as the Play button's OnClick() target in the Inspector, so this IS the click
    // handler already -- it doesn't need to go find and subscribe to another button's click event
    void ChangeScene()
    {
        Debug.Log("Button clicked!");
        SceneManager.TransitionTo("Motel");
    }
}