using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Yarn.Unity;

public class EvidenceInventory : MonoBehaviour
{
    // Evidence name, Description, id, and colection status
    LinkedList<(int id, String evidenceName, String evidenceDescription, String collectionStatus)> evidenceList = new LinkedList<(int, String, String, String)>();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        evidenceList.AddLast((1, "???", "???", "Undiscovered"));
        evidenceList.AddLast((2, "???", "???", "Undiscovered"));
        evidenceList.AddLast((3, "???", "???", "Undiscovered"));
        evidenceList.AddLast((4, "???", "???", "Undiscovered"));
        evidenceList.AddLast((5, "???", "???", "Undiscovered"));
    }

    [YarnCommand("writeToNotePad")]
    public void writeToNotepad(int id, String title, String description)
    {
        LinkedListNode<(int id, String evidenceName, String evidenceDescription, String collectionStatus)> current = evidenceList.First;

        while (current != null)
        {
            if (current.Value.id == id)
            {
                var item = current.Value;
                item.evidenceName = title;
                item.evidenceDescription = description;
                current.Value = item;
                Debug.Log($"[Notepad] Recorded Evidence ID {id}: '{title}' - {description}");
                return;
            }
            current = current.Next;
        }
    }


    [YarnCommand("changeEvidenceStatus")]
    public void changeEvidenceStatus(int id, String collected)
    {
        LinkedListNode<(int id, String evidenceName, String evidenceDescription, String collectionStatus)> current = evidenceList.First;

        while (current != null)
        {
            if (current.Value.id == id)
            {
                var item = current.Value;
                item.collectionStatus = collected;
                current.Value = item;
                Debug.Log($"[Notepad] Status for Evidence ID {id} changed to collected = {collected}");
                return;
            }
            current = current.Next;
        }
    }

}
