using UnityEngine;
using UnityEngine.InputSystem;

public sealed class SettingsMenuToggle : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject settingsCanvas;

    [Header("Behavior")]
    [SerializeField] private bool startClosed = true;
    [SerializeField] private bool pauseGameWhileOpen = true;
    [SerializeField] private bool manageCursor = true;

    public bool IsOpen { get; private set; }

    private void Start()
    {
        SetMenuOpen(!startClosed);
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        SetMenuOpen(!IsOpen);
    }

    public void OpenMenu()
    {
        SetMenuOpen(true);
    }

    public void CloseMenu()
    {
        SetMenuOpen(false);
    }

    private void SetMenuOpen(bool open)
    {
        IsOpen = open;

        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(open);
        }

        if (pauseGameWhileOpen)
        {
            Time.timeScale = open ? 0f : 1f;
        }

        if (manageCursor)
        {
            Cursor.visible = open;
            Cursor.lockState = open
                ? CursorLockMode.None
                : CursorLockMode.Locked;
        }
    }

    private void OnDestroy()
    {
        if (pauseGameWhileOpen && Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}