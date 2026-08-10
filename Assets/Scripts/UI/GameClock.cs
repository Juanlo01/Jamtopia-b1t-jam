using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameClock : MonoBehaviour
{
    [SerializeField] Image clockHand;
    [SerializeField] TextMeshProUGUI daysLeft;

    [SerializeField] float dayLength = 60f;
    [SerializeField] int totalDays = 10;

    // static so day progress survives GameClock being recreated by a fresh
    // GeneralUI prefab instance every time SceneManager loads a new scene
    private static int currentDay = 1;
    private static float dayTimer = 0f;

    private void Start()
    {
        UpdateClockHandRotation();
        UpdateDaysLeftText();
    }

    private void Update()
    {
        if (clockHand != null)
        {
            float degreesPerSecond = 180f / dayLength;
            clockHand.rectTransform.Rotate(0f, 0f, degreesPerSecond * Time.deltaTime);
        }

        dayTimer += Time.deltaTime;
        if (dayTimer >= dayLength)
        {
            dayTimer -= dayLength;
            currentDay++;
            ResetClockHand();
            UpdateDaysLeftText();

            if (currentDay > totalDays)
            {
                SceneManager.TransitionTo("LossScreen");
            }
        }
    }

    private void ResetClockHand()
    {
        if (clockHand != null)
        {
            clockHand.rectTransform.localEulerAngles = new Vector3(0f, 0f, -90f);
        }
    }

    // used on Start() so a scene reloaded mid-day resumes the hand at its
    // persisted position instead of snapping back to the start-of-day pose
    private void UpdateClockHandRotation()
    {
        if (clockHand != null)
        {
            float progress = Mathf.Clamp01(dayTimer / dayLength);
            float angle = Mathf.Lerp(-90f, 90f, progress);
            clockHand.rectTransform.localEulerAngles = new Vector3(0f, 0f, angle);
        }
    }

    private void UpdateDaysLeftText()
    {
        if (daysLeft != null)
        {
            daysLeft.text = $"{currentDay} / {totalDays}";
        }
    }
}
