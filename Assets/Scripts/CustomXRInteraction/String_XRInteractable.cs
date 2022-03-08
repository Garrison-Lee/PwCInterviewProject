using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is attached to the bow's string. If it is selected while the bow is not held, it passes the bowShaft
///  to the interactor instead of itself. If it is selected while the bow *is* held, then it begins tracking the 
///  pullingInteractor's position to determine draw amount. It updates the string's LineRenderer according to the draw amount
///  to "animate" the string. When released it checks for a nocked arrow and passes the draw strength to the arrow.Launch() method
///  if an arrow is nocked.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class String_XRInteractable : XRBaseInteractable
{
    [Header("References")]
    [Tooltip("Please drag a reference to the bow parent object here")]
    public Bow_XRInteractable myBow; //This reference is less obvious, it's so we can redirect the selection if the bow is not held and the string is grabbed.
    [Tooltip("Please drag a reference to the nock here")]
    public Nock_XRInteractor myNock; //This is so we can check for a nocked arrow and launch it when the string is released. Also used to "animate" the arrow with the string
    [Tooltip("The transform representing the middle of the string when the bow is relaxed")]
    public Transform start = null; //These are so I could fiddle with the max draw distances and such in the editor since I wasn't sure how long my arms would be.
    [Tooltip("The transform represnting the middle of the string when the bow is drawn")]
    public Transform drawn = null;

    //These bools are set by methods listening to events in the Bow_XR and Nock_XR scripts and change how the string handles input.
    private bool bowHeld = false;
    private bool arrowNocked = false;
    //Since we'll manually do calculations using the position of this hand, we'll store a reference
    private XRBaseInteractor myInteractor;

    //The "string" is now a line renderer so that I can more easily "animate" the bow
    private LineRenderer line;
    //Since the line renderer is using local space, this float is for converting between local and world space only once.
    private float maxLineDrawDistance; 
    //This is the normalized value of how far the string is currently pulled back
    private float curDrawValue;

    private void Start()
    {
        //Both of these are boolean events that simply alert us to when the bow is grabbed/dropped and when an arrow is nocked/dropped
        myBow.GrabEvent.AddListener(SetBowHeld);
        myNock.NockEvent.AddListener(SetArrowNocked);

        //LineRenderer is using local space, we'll want to use global space for most calculations so we set this one aside and store it
        line = GetComponent<LineRenderer>();
        maxLineDrawDistance = (drawn.localPosition - start.localPosition).magnitude;
    }

    /// <summary>
    /// If the bow is held in one hand, then begin drawing the string according to other hand's position. If the bow
    ///  is unheld, redirect this selection to the bow.
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        //Bow not held and player grabbed string, redirect the action so they grab the bow instead.
        if (!bowHeld)
        {
            interactionManager.SelectExit(args.interactorObject, this);
            interactionManager.SelectEnter(args.interactorObject, myBow);
        }
        else
        {
            //If we made it here, we're holding the bow in one hand and the string in the other! Exciting stuff, let's draw the bow now!
            myInteractor = args.interactorObject as XRBaseInteractor;
            if (myInteractor == null)
                Debug.LogError("Was unable to cast the interactor that just selected the string to a XRBaseInteractor...");
        }
    }

    //The string has been released. If the conditions are right, this launches an arrow. Otherwise the bow just loses tension.
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        //We can stop tracking the pullingInteractor now
        myInteractor = null;

        //You can always draw and release the string, but is there actually an arrow to let fly?
        if (arrowNocked)
        {
            myNock.LaunchArrow(curDrawValue);
        }

        //Resets the drawValue and also resets LineRenderer to show straight line
        UndoPull();
    }

    /// <summary>
    /// Called every frame to process interactables associated with whatever InteractionManager this interactable is using. Basically an Update()
    ///  that is tied together with the XRInteraction Toolkit so we know the values have been updated very recently
    /// </summary>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (isSelected && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic) //Do this as often as possible
        {
            CalculatePull();
        }
    }

    /// <summary>
    /// Uses the position of the hand holding the string relative to the starting position of the string to determine pull
    /// </summary>
    private void CalculatePull()
    {
        if (myInteractor == null)
        {
            Debug.LogError("Tried to call CalculatePull() while the string had no Interactor.");
            return;
        }

        //We're going to use a dot product to "cast a shadow" from the actual vector the player has drawn the string to the ideal vector of straight back.
        Vector3 actualPull = myInteractor.transform.position - start.position;
        Vector3 idealPull = drawn.position - start.position;

        //"Cast a shadow" from physics 1. Basically, how much of our pull is in the right direction? We'll use the component that's in the right direction.
        float effectivePull = Vector3.Dot(actualPull, idealPull.normalized) / idealPull.magnitude;
        effectivePull = Mathf.Clamp(effectivePull, 0f, 1f); //Normalize the draw power
        curDrawValue = effectivePull;

        //Where should we move the string/nock/arrow to? And update LineRenderer too!
        line.SetPosition(1, new Vector3(0, 0, -maxLineDrawDistance * curDrawValue));
        myNock.transform.position = Vector3.Lerp(start.position, drawn.position, curDrawValue);
    }

    /// <summary>
    /// Moves things back to how they were before the bow was drawn.
    /// </summary>
    private void UndoPull()
    {
        curDrawValue = 0f;
        line.SetPosition(1, Vector3.zero);
        myNock.transform.position = Vector3.Lerp(start.position, drawn.position, curDrawValue);
    }

    //The lil callbacks for the bow and nocks' events
    private void SetArrowNocked(bool val)
    {
        arrowNocked = val;
    }
    private void SetBowHeld(bool val)
    {
        bowHeld = val;
    }
}
