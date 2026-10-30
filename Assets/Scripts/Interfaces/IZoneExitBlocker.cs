namespace Interfaces
{
    // Something that can forbid leaving its zone right now, e.g. the player's hand while it holds an object
    public interface IZoneExitBlocker
    {
        bool CanLeaveZone { get; }
    }
}
