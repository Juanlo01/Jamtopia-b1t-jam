using UnityEngine;

public class EvidenceViewCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Camera>().enabled = true;
        GetComponent<Camera>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
