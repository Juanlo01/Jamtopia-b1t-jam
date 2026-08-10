using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;

[Serializable]
public class EvidenceData
{
    public string id;
    public string evidenceName = "???";
    public string evidenceDescription = "???";
    public string collectionStatus = "Undiscovered";
    public int entry; // 1 to 5
}

public class EvidenceInventory : MonoBehaviour
{
    [Header("UI Text Slots (Slots 0 to 4 = Entries 1 to 5)")]
    [SerializeField] private TMP_Text[] motelEntryTextSlots = new TMP_Text[5];
    [SerializeField] private TMP_Text[] motelEvidenceList = new TMP_Text[5];

    [Header("Evidence Data")]
    [SerializeField] private List<EvidenceData> evidenceList = new List<EvidenceData>();

    // [YarnCommand] on an *instance* method makes Yarn Spinner treat the command's first argument as
    // the name of a GameObject to find this component on (which is why calls used to need a stray
    // "UIDocument" token up front). Static commands skip that target lookup entirely, so the commands
    // below are static and forward to the one live instance -- same approach as SceneManager.TransitionTo.
    private static EvidenceInventory instance;

    private void Awake()
    {
        instance = this;

        // populated here rather than in Start() because SceneManager.Start() kicks off the Initialize
        // node (which calls changeEvidenceStatus); Start() ordering between the two isn't guaranteed,
        // but every Awake() runs before any Start(), so the list is always ready in time.
        EnsureEvidenceList();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void EnsureEvidenceList()
    {
        // Populate default motel evidence if the list is empty in Inspector
        if (evidenceList.Count == 0)
        {
            evidenceList.Add(new EvidenceData { id = "motelFrontDoor", entry = 1 });
            evidenceList.Add(new EvidenceData { id = "motelDeadWoman", entry = 2 });
            evidenceList.Add(new EvidenceData { id = "motelDroppedNail", entry = 3 });
            evidenceList.Add(new EvidenceData { id = "motelHairbrush", entry = 4 });
            evidenceList.Add(new EvidenceData { id = "motelMakeupWipes", entry = 5 });
            evidenceList.Add(new EvidenceData { id = "greenroomSaxophone", entry = 1 });
            evidenceList.Add(new EvidenceData { id = "greenroomSaxophonePlayer", entry = 2 });
            evidenceList.Add(new EvidenceData { id = "greenroomIceBucket", entry = 3 });
            evidenceList.Add(new EvidenceData { id = "greenroomVent", entry = 4 });
            evidenceList.Add(new EvidenceData { id = "greenroomInstruments", entry = 5 });
        }
    }

    void Start()
    {
        // Display initial statuses (e.g., "Undiscovered") on game start
        InitializeUI();
    }

    [YarnCommand("writeToNotepad")]
    public static void writeToNotepad(string id, string title, string description)
    {
        if (instance == null)
        {
            Debug.LogWarning($"[EvidenceInventory] no instance in the scene to write '{id}' to the notepad with.");
            return;
        }

        instance.WriteToNotepadInternal(id, title, description);
    }

    [YarnCommand("changeEvidenceStatus")]
    public static void changeEvidenceStatus(string id, string collected)
    {
        if (instance == null)
        {
            Debug.LogWarning($"[EvidenceInventory] no instance in the scene to change status of '{id}' with.");
            return;
        }

        instance.ChangeEvidenceStatusInternal(id, collected);
    }

    private void WriteToNotepadInternal(string id, string title, string description)
    {
        EvidenceData item = evidenceList.Find(x => x.id == id);

        if (item != null)
        {
            item.evidenceName = title;
            item.evidenceDescription = description;

            UpdateNotepadUI(item.entry, title, description);
        }
        else
        {
            Debug.LogWarning($"[EvidenceInventory] ID '{id}' not found!");
        }
    }

    private void ChangeEvidenceStatusInternal(string id, string collected)
    {
        EvidenceData item = evidenceList.Find(x => x.id == id);

        if (item != null)
        {
            item.collectionStatus = collected;

            // Updates ONLY the collection status text slot for this entry number
            UpdateEvidenceListUI(item.entry, collected);

            Debug.Log($"[EvidenceInventory] Entry {item.entry} status changed to: {collected}");
        }
        else
        {
            Debug.LogWarning($"[EvidenceInventory] ID '{id}' not found!");
        }
    }

    private void InitializeUI()
    {
        foreach (var item in evidenceList)
        {
            UpdateNotepadUI(item.entry, item.evidenceName, item.evidenceDescription);
            UpdateEvidenceListUI(item.entry, item.collectionStatus);
        }
    }

    // Updates the specific entry's collection status text
    private void UpdateEvidenceListUI(int entryNumber, string collectionStatus)
    {
        int index = entryNumber - 1;

        if (index >= 0 && index < motelEvidenceList.Length)
        {
            if (motelEvidenceList[index] != null)
            {
                motelEvidenceList[index].text = collectionStatus;
            }
            else
            {
                Debug.LogWarning($"[EvidenceInventory] motelEvidenceList slot at index {index} is not assigned!");
            }
        }
    }

    // Updates the specific entry's notepad description text
    private void UpdateNotepadUI(int entryNumber, string title, string description)
    {
        int index = entryNumber - 1;

        if (index >= 0 && index < motelEntryTextSlots.Length)
        {
            if (motelEntryTextSlots[index] != null)
            {
                motelEntryTextSlots[index].text = $"{title}: {description}";
            }
            else
            {
                Debug.LogWarning($"[EvidenceInventory] motelEntryTextSlots slot at index {index} is not assigned!");
            }
        }
    }
}