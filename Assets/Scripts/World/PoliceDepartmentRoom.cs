// Granular rooms tracked within policeDepartment.unity. "Office" is the default/generic state --
// arriving via a link that isn't tagged into one of the specific rooms below (or before the player
// has moved at all) leaves $setting as "office" rather than a stale room from before.
public enum PoliceDepartmentRoom
{
    Office,
    BoltonsOffice,
    Breakroom,
    InterrogationRoom,
    Lab,
}
