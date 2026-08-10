using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    private VisualElement uiStartMenu;

        // private void OnEnable()
        // {
        //     // One-line query & click listener setup
        //     GetComponent<UIDocument>().rootVisualElement.Q<Button>("PlayButton").clicked += () => 
        //     {
        //         Debug.Log("Button clicked!");
        //         SceneManager.LoadScene("Motel");
        //     };
        // }

        void ChangeScene()
        {
            Debug.Log("Button clicked!");
            SceneManager.LoadScene("Motel");
        }
}