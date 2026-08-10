using UnityEngine;
using Yarn.Unity;

// Attach to the Camera in policeDepartment.unity. Subscribes to PoliceDepartmentRoomBus (published
// by PoliceDepartmentRoomPublisher siblings on the scene's TeleportLinks) and mirrors whichever
// room the player last arrived in into Yarn's $setting. SceneManager defers $setting entirely to
// this tracker while policeDepartment is the active scene (see SceneManager.PushSettingForCurrentScene).
public class PoliceDepartmentRoomTracker : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private PoliceDepartmentRoom startingRoom = PoliceDepartmentRoom.Office;

    public PoliceDepartmentRoom CurrentRoom { get; private set; }

    private void OnEnable()
    {
        PoliceDepartmentRoomBus.RoomEntered += HandleRoomEntered;
    }

    private void OnDisable()
    {
        PoliceDepartmentRoomBus.RoomEntered -= HandleRoomEntered;
    }

    private void Start()
    {
        HandleRoomEntered(startingRoom);
    }

    private void HandleRoomEntered(PoliceDepartmentRoom room)
    {
        CurrentRoom = room;

        if (dialogueRunner == null)
        {
            return;
        }

        dialogueRunner.VariableStorage.SetValue("$setting", RoomToSettingString(room));
    }

    private static string RoomToSettingString(PoliceDepartmentRoom room)
    {
        switch (room)
        {
            case PoliceDepartmentRoom.BoltonsOffice: return "boltons_office";
            case PoliceDepartmentRoom.Breakroom: return "breakroom";
            case PoliceDepartmentRoom.InterrogationRoom: return "interrogation_room";
            case PoliceDepartmentRoom.Lab: return "lab";
            default: return "office";
        }
    }
}
