using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// The quiver manages an ObjectPool of arrows and provides one to the hand that interacted with the quiver.
///  It is attached to a trigger volume behind the player's headset. So it's always invisible, but interactable nonetheless.
/// </summary>
public class Quiver_XRInteractable : XRBaseInteractable
{
    [Header("Quiver Settings")]
    [Tooltip("How many arrows would you like in the Object Pool? Will re-use arrows once the quiver is empty.")]
    public int poolSize = 13;

    private List<GameObject> arrowPool;
    private int poolIndex = -1; //We cycle which arrow is offered up so its the "oldest" arrow that's re-used first.

    //It looks like XRBaseInteractable never implements Start so there's nothing to override...
    private void Start()
    {
        //Reference to the one arrow that's actually in the scene way below the map
        GameObject templateArrow = GameObject.Find("Template_Arrow");

        //ObjectPool is a list of arrows
        arrowPool = new List<GameObject>();
        
        //Create as many arrows as is set in editor, name them with their number
        for (int i = 0; i < poolSize; i++)
        {
            GameObject newArrow = Instantiate(templateArrow);
            newArrow.name = "Arrow_" + i.ToString();
            arrowPool.Add(newArrow);
        }
    }

    /// <summary>
    /// When the quiver is selected, we draw out an arrow and give it to the hand
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        DrawArrow(args.interactorObject, args.interactableObject);
    }

    /// <summary>
    /// Returns the first unused arrow, or the oldest-used arrow if all have been shot
    /// </summary>
    private GameObject PickArrow()
    {
        poolIndex = (poolIndex + 1) % poolSize;
        return arrowPool[poolIndex];
    }

    /// <summary>
    /// Takes the chosen arrow, then puts it into the hand that interacted with the quiver.
    /// </summary>
    private void DrawArrow(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        IXRSelectInteractable arrow = PickArrow().GetComponent<Arrow_XRInteractable>();

        interactionManager.SelectExit(interactor, interactable);
        interactionManager.SelectEnter(interactor, arrow);
    }
}
