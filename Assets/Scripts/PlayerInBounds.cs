using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A static class that is updated by the BoundaryScript when the player exits/enters the bounds. 
///  The targets then check this class before awarding points to ensure the player is not cheating.
/// </summary>
public static class PlayerInBounds
{
    public static bool InBounds = false;
}
