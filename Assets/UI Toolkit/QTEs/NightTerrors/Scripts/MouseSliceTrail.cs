using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MouseSliceTrail : MonoBehaviour
{
    private LineRenderer line;
    private Camera mainCam;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        mainCam = Camera.main;
        line.positionCount = 0;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            line.positionCount = 0;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 5f; // Distance in front of camera
            Vector3 worldPos = mainCam.ScreenToWorldPoint(mousePos);

            line.positionCount++;
            line.SetPosition(line.positionCount - 1, worldPos);
        }
        if (Input.GetMouseButtonUp(0))
        {
            line.positionCount = 0;
        }
    }
}