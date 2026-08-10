using System.Collections;
using System.Collections.Generic;
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
    // that's why a plain instance method here broke <<transitionTo("Station")>> ("Station" was
    // being looked up as a GameObject, not passed as sceneName). Static commands skip that target
    // lookup entirely, so TransitionTo forwards to the one live instance instead.
    private static SceneManager instance;

    // The "Dialogue System" prefab (and its VariableStorage) is recreated fresh on every scene
    // load, same as GeneralUI/GameClock -- so, like GameClock's day fields, variables have to be
    // bridged across the destroy/recreate boundary via a static, not left to the storage itself.
    private static (Dictionary<string, float> floats, Dictionary<string, string> strings, Dictionary<string, bool> bools)? variableSnapshot;

    private void Awake()
    {
        instance = this;

        SetImageAlpha(fadeImage, 0f);
        SetImageAlpha(spinnerImage, 0f);
        SetSpinnerVisible(false);

        RestoreVariableSnapshot();
        PushSettingForCurrentScene();
    }

    private void Start()
    {
        // variableSnapshot is only ever populated by a prior scene load via this manager, so its
        // absence means this is the very first scene of a fresh run -- run Initialize's <<declare>>
        // defaults and its setup commands (eg. changeEvidenceStatus) exactly once, here. Later scene
        // loads restore the carried-forward snapshot instead and must NOT re-run this, or it would
        // stomp all progress back to defaults.
        if (variableSnapshot == null)
        {
            RunInitializeDialogue();
        }
    }

    private static void RunInitializeDialogue()
    {
        DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
        if (runner == null)
        {
            return;
        }

        // <<declare>> alone never touches VariableStorage's live dictionary -- it only registers a
        // fallback default that TryGetValue reads on demand, so declared variables never actually get
        // *stored* (and never show up in InMemoryVariableStorage's debug view) until something calls
        // SetValue on them. Push every declared default in explicitly, then run Initialize for its
        // actual runtime commands (eg. changeEvidenceStatus), which declares can't cover.
        PushDeclaredDefaults(runner);
        runner.StartDialogue("Initialize").Forget();
    }

    private static void PushDeclaredDefaults(DialogueRunner runner)
    {
        if (runner.YarnProject == null)
        {
            return;
        }

        foreach (var pair in runner.YarnProject.InitialValues)
        {
            switch (pair.Value)
            {
                case bool boolValue:
                    runner.VariableStorage.SetValue(pair.Key, boolValue);
                    break;
                case string stringValue:
                    runner.VariableStorage.SetValue(pair.Key, stringValue);
                    break;
                case float floatValue:
                    runner.VariableStorage.SetValue(pair.Key, floatValue);
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    [YarnCommand("transitionTo")]
    public static void TransitionTo(string sceneName)
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

    private static void CaptureVariableSnapshot()
    {
        DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
        if (runner == null)
        {
            return;
        }

        variableSnapshot = runner.VariableStorage.GetAllVariables();
    }

    private static void RestoreVariableSnapshot()
    {
        if (variableSnapshot == null)
        {
            return;
        }

        DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
        if (runner == null)
        {
            return;
        }

        var (floats, strings, bools) = variableSnapshot.Value;
        runner.VariableStorage.SetAllVariables(floats, strings, bools, clear: false);
    }

    // $setting has two owners depending on scene: inside policeDepartment, PoliceDepartmentRoomTracker
    // owns it (there are several trackable rooms within that one scene); everywhere else, the scene
    // itself IS the setting, so SceneManager just mirrors the active scene's name straight in. Runs
    // after RestoreVariableSnapshot so it correctly overwrites whatever stale $setting carried over
    // from the scene just left.
    private const string PoliceDepartmentSceneName = "policeDepartment";

    private static void PushSettingForCurrentScene()
    {
        string activeSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (activeSceneName == PoliceDepartmentSceneName)
        {
            return;
        }

        DialogueRunner runner = FindFirstObjectByType<DialogueRunner>();
        if (runner == null)
        {
            return;
        }

        runner.VariableStorage.SetValue("$setting", activeSceneName);
    }

    private IEnumerator LoadSceneRoutine(int buildIndex)
    {
        isLoading = true;

        yield return StartCoroutine(FadeRoutine(0f, 1f));
        SetSpinnerVisible(true);

        CaptureVariableSnapshot();

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
