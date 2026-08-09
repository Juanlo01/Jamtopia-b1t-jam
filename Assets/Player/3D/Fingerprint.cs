using System;
using Unity.VisualScripting;
using UnityEngine;

public class Fingerprint : MonoBehaviour
{
    public float a;
    [SerializeField] GameObject previousTool;
    [SerializeField] GameObject nextTool;

    private void Awake(){
        a = 0f;
         GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, a);
    }

    private void Update()
    {
        if (a >= 1f)
        {
            previousTool.SetActive(false);
            nextTool.SetActive(true);
        }
    }


    private void OnTriggerEnter(Collider Collision)
    {
        a += 0.2f;
        GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, a);
    }
}
