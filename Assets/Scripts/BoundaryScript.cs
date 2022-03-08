using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This script tracks when the player is in or out of the established bounds. It updates a static class that the targets can then
///  check before allocating points for a shot.
/// </summary>
public class BoundaryScript : MonoBehaviour
{
    private Vector3 halfExtents;
    private int playerLayerMask;
    private bool playerInBounds = false;

    private void Awake()
    {
        halfExtents = new Vector3(5, 2, 5);
        playerLayerMask = LayerMask.GetMask("Player");
    }

    private void FixedUpdate()
    {
        if (Physics.CheckBox(transform.position, halfExtents, Quaternion.identity, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (!playerInBounds)
            {
                playerInBounds = true;
                PlayerInBounds.InBounds = playerInBounds;
            }
        }
        else
        {
            if (playerInBounds)
            {
                playerInBounds = false;
                PlayerInBounds.InBounds = playerInBounds;
            }
        }
    }
}
