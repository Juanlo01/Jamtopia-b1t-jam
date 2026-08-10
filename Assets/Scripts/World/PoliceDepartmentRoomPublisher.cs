using UnityEngine;

// Sibling component for a TeleportLink within policeDepartment.unity -- tags one arrival side of
// that link with a room, and publishes to PoliceDepartmentRoomBus whenever the player teleports
// into it. Add a second instance (with the other arrivalSide) on the same link if both sides lead
// into tracked rooms; leave links that only lead to generic hallway/office space unpublished.
public class PoliceDepartmentRoomPublisher : MonoBehaviour
{
    [SerializeField] private TeleportLink teleportLink;
    [SerializeField] private TeleportSide arrivalSide;
    [SerializeField] private PoliceDepartmentRoom room;

    private void OnEnable()
    {
        if (teleportLink != null) teleportLink.OnTeleported += HandleTeleported;
    }

    private void OnDisable()
    {
        if (teleportLink != null) teleportLink.OnTeleported -= HandleTeleported;
    }

    private void HandleTeleported(TeleportSide side)
    {
        if (side == arrivalSide) PoliceDepartmentRoomBus.Publish(room);
    }
}
