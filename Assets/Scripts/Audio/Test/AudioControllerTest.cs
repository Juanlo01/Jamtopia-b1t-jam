using SimpleAudioSystem;
using UnityEngine;

public sealed class SceneMusicController : MonoBehaviour
{

    [SerializeField] private string sleepwalkMusicId = "mus.sleepwalk";

    private string currentSceneMusicId;

    private void Start()
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