using UnityEngine;

public sealed class LastPlayerSighting
{
    private static LastPlayerSighting _instance;

    public Vector3 Position { get; set; }			        // The last global sighting of the player.
    public Vector3 ResetPosition { get; private set; }      // The default position if the player is not in sight.

    public LastPlayerSighting()
    {
        Position = new Vector3(1000f, 1000f, 1000f);
        ResetPosition = new Vector3(1000f, 1000f, 1000f);
    }

    public void Reset()
    {
        Position = ResetPosition;
    }

    public static LastPlayerSighting Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LastPlayerSighting();
            }
            return _instance;
        }
    }
}