using UnityEngine;


public class NightTerrorSpawnController : MonoBehaviour
{
    [SerializeField] private GameObject spiky, teeny, smoky;

    [SerializeField] private bool evidenceView, sleeping = true;

    // Update is called once per frame
    void Update()
    {
        // Night Terrors can only spawn if Player is asleep and in Evidence View
        if (sleeping && evidenceView)
        {
            
        }
    }
}
