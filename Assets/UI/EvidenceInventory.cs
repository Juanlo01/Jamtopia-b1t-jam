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

    void Start()
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

        // Display initial statuses (e.g., "Undiscovered") on game start
        InitializeUI();
    }

    [YarnCommand("writeToNotepad")]
    public void writeToNotepad(string id, string title, string description)
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

    [YarnCommand("changeEvidenceStatus")]
    public void changeEvidenceStatus(string id, string collected)
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