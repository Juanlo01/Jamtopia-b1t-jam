using System;

// Decouples policeDepartment.unity's TeleportLinks (publishers, via PoliceDepartmentRoomPublisher)
// from PoliceDepartmentRoomTracker (the one subscriber, on the scene's Camera) -- neither needs a
// direct reference to the other.
public static class PoliceDepartmentRoomBus
{
    public static event Action<PoliceDepartmentRoom> RoomEntered;

    public static void Publish(PoliceDepartmentRoom room)
    {
        RoomEntered?.Invoke(room);
    }
}
