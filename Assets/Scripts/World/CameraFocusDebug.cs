using UnityEngine;

// Debug helper for previewing candidate camera framings (eg. TeleportLink's focusA/focusB points)
// without wiring up the full teleport flow. Holds a persistent list of focus transforms; whichever
// one is selected gets copied onto this Camera's transform on Awake.
[RequireComponent(typeof(Camera))]
public class CameraFocusDebug : MonoBehaviour
{
    [Tooltip("Candidate camera transforms this debug camera can be set to.")]
    [SerializeField] private Transform[] focusPoints;

    [Tooltip("Index into focusPoints applied to this Camera's transform on Awake.")]
    [SerializeField] private int selectedFocusIndex;

    public Transform SelectedFocus =>
        (focusPoints != null && selectedFocusIndex >= 0 && selectedFocusIndex < focusPoints.Length)
            ? focusPoints[selectedFocusIndex]
            : null;

    private void OnValidate(){
        if(focusPoints == null || focusPoints.Length == 0){
            selectedFocusIndex = 0;
            return;
        }
        selectedFocusIndex = Mathf.Clamp(selectedFocusIndex, 0, focusPoints.Length - 1);
    }

    private void Awake(){
        ApplySelectedFocus();
    }

    // public so the custom inspector can re-apply the current selection on demand (eg. a preview button)
    public void ApplySelectedFocus(){
        Transform focus = SelectedFocus;
        if(focus == null) return;

        transform.SetPositionAndRotation(focus.position, focus.rotation);
    }
}
