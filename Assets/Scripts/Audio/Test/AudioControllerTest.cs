using SimpleAudioSystem;
using UnityEngine;

public sealed class AudioManagerTest : MonoBehaviour
{
    public void PlayTestMusic()
    {
        AudioManager.Instance.PlayMusic("music.test");
    }

    public void StopTestMusic()
    {
        AudioManager.Instance.StopMusic();
    }

    public void PlayTestSFX()
    {
        AudioManager.Instance.PlayOneShot("sfx.test");
    }
}