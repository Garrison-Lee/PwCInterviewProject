using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A *very* simple mover script to add moving targets.
/// </summary>
public class SimpleMoverScript : MonoBehaviour
{
    [Header("Moving Target Settings")]
    [Tooltip("Each Vector3 in this list is a waypoint on the path the target will trace.")]
    public List<Vector3> pathNodes = new List<Vector3>();
    [Tooltip("How many Unity units per second should this target travel?")]
    public float moveSpeed = 1f;

    private float tolerance = 0.05f; //5cm tolerance
    private int numNodes;
    private int curIDX = 0;
    private int nextIDX = 1;

    private Vector3 nextGoal;

    private void Awake()
    {
        numNodes = pathNodes.Count;
    }

    private void Start()
    {
        transform.position = pathNodes[curIDX];
        nextGoal = pathNodes[nextIDX];
    }

    private void Update()
    {
        CheckProximity();

        //Head towards whatever the nextGoal is at the speed that is set in editor
        Vector3 velocity = (nextGoal - transform.position).normalized;
        transform.position += velocity * moveSpeed * Time.deltaTime;
    }

    //When we're within [tolerance] units of the nextGoal, we'll count ourselves
    // as being there and start heading for the next one
    private void CheckProximity()
    {
        if (Vector3.Distance(pathNodes[nextIDX], transform.position) <= tolerance)
        {
            IncrementIndices();
            nextGoal = pathNodes[nextIDX];
        }
    }

    //Indices++ but don't go out of bounds!
    private void IncrementIndices()
    {
        curIDX = (curIDX + 1) % numNodes;
        nextIDX = (nextIDX + 1) % numNodes;
    }
}
