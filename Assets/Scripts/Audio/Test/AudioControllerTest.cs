using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneMusicController : MonoBehaviour
{

    [SerializeField] private string sleepwalkMusicId = "mus.sleepwalk";

    private string currentSceneMusicId;

    // this object persists across scene loads (DontDestroyOnLoad on the AudioManager root), so
    // Start() only ever fires once for the very first scene -- listen for every later scene load too
    private void OnEnable()
    {
        // fully-qualified: this project also has its own global-namespace SceneManager (TransitionTo),
        // so a bare "SceneManager" here would be ambiguous with UnityEngine.SceneManagement.SceneManager
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForCurrentScene();
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;


        switch (sceneName)
        {
            case "MainMenu":
                currentSceneMusicId = "mus.mainmenu";
                break;

            case "Greenroom":
                currentSceneMusicId = "mus.jazzbar";
                break;

            case "Motel":
                currentSceneMusicId = "mus.motel";
                break;

            case "Station":
                currentSceneMusicId = "mus.breakroom";
                break;

            case "NightTerrors":
                currentSceneMusicId = "mus.minigame";
                break;

            default:
                currentSceneMusicId = null;
                Debug.Log($"No music assigned for scene: {sceneName}");
                return;
        }

        AudioManager.Instance.PlayMusic(currentSceneMusicId);
    }

    public void StartSleepwalkMusic()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlayMusic(sleepwalkMusicId);
    }

    public void StopSleepwalkMusic()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(currentSceneMusicId))
        {
            AudioManager.Instance.PlayMusic(currentSceneMusicId);
        }
    }
}