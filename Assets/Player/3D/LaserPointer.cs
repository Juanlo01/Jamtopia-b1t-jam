//Attach this script to your Camera
//This draws a line in the Scene view going through a point 200 pixels from the lower-left corner of the screen
//To see this, enter Play Mode and switch to the Scene tab. Zoom into your Camera's position.
//Referenced from Unity 6.3LTS Documentation @ https://docs.unity3d.com/6000.3/Documentation/ScriptReference/Camera.ScreenPointToRay.html

using UnityEngine;
using System.Collections;

public class LaserPointer : MonoBehaviour
{
    Camera cam;
    Vector3 pos = new Vector3(200, 200, 0);


    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(pos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
    }
}
