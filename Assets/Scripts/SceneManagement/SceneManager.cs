using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class SceneManager : MonoBehaviour
{
    [SerializeField] Image fadeImage;
    [SerializeField] Image spinnerImage;

    [SerializeField] float fadeDuration = 0.5f;
    [SerializeField] float spinDegreesPerSecond = 180f;

    private bool isLoading = false;

    // [YarnCommand] on an instance method makes Yarn Spinner treat the command's first
    // argument as the name of a GameObject to find the component on (eg. <<jump MyCharacter>>) --
    // that's why a plain instance method here broke <<load_scene Station>> ("Station" was being
    // looked up as a GameObject, not passed as sceneName). Static commands skip that target
    // lookup entirely, so LoadScene forwards to the one live instance instead.
    private static SceneManager instance;

    private void Awake()
    {
        instance = this;

        SetImageAlpha(fadeImage, 0f);
        SetImageAlpha(spinnerImage, 0f);
        SetSpinnerVisible(false);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    [YarnCommand("load_scene")]
    public static void LoadScene(string sceneName)
    {
        if (instance == null)
        {
            Debug.LogWarning($"{nameof(SceneManager)}: no instance in the scene to load \"{sceneName}\" with.");
            return;
        }

        instance.LoadSceneInternal(sceneName);
    }

    private void LoadSceneInternal(string sceneName)
    {
        if (isLoading)
        {
            return;
        }

        if (!TryGetBuildIndex(sceneName, out int buildIndex))
        {
            Debug.LogWarning($"{nameof(SceneManager)}: no scene named \"{sceneName}\" found in Build Settings.", this);
            return;
        }

        StartCoroutine(LoadSceneRoutine(buildIndex));
    }

    private static bool TryGetBuildIndex(string sceneName, out int buildIndex)
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                buildIndex = i;
                return true;
            }
        }

        buildIndex = -1;
        return false;
    }

    private IEnumerator LoadSceneRoutine(int buildIndex)
    {
        isLoading = true;

        yield return StartCoroutine(FadeRoutine(0f, 1f));
        SetSpinnerVisible(true);

        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(buildIndex);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            Spin();
            yield return null;
        }

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            Spin();
            yield return null;
        }

        SetSpinnerVisible(false);
        yield return StartCoroutine(FadeRoutine(1f, 0f));

        isLoading = false;
    }

    private void Spin()
    {
        if (spinnerImage != null)
        {
            spinnerImage.rectTransform.Rotate(0f, 0f, -spinDegreesPerSecond * Time.deltaTime);
        }
    }

    private IEnumerator FadeRoutine(float fromAlpha, float toAlpha)
    {
        if (fadeImage == null)
        {
            yield break;
        }

        float accumulator = 0f;
        while (accumulator < fadeDuration)
        {
            accumulator += Time.deltaTime;
            SetImageAlpha(fadeImage, Mathf.Lerp(fromAlpha, toAlpha, accumulator / fadeDuration));
            yield return null;
        }

        SetImageAlpha(fadeImage, toAlpha);
    }

    private static void SetImageAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            return;
        }

        Color colour = image.color;
        colour.a = alpha;
        image.color = colour;
    }

    private void SetSpinnerVisible(bool visible)
    {
        if (spinnerImage != null)
        {
            spinnerImage.gameObject.SetActive(visible);
        }
    }
}
