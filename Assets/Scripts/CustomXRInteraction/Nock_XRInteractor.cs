using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is attached to the middle of the string on a child transform called the nock. It represents the part of the bow
///  where an archer would actually fit an arrow before drawing and firing. It is a SocketInteractor. It primariy just looks for
///  an arrow hovering itself and steals it from the hand if it finds one. It also sends an event when an arrow is nocked/dropped
///  so the string can behave correctly
/// </summary>
public class Nock_XRInteractor : XRSocketInteractor
{
    //Boolean event so the string can know if an arrow is currently nocked or not
    [System.Serializable]
    public class ArrowNockedEvent : UnityEvent<bool> { }
    public ArrowNockedEvent NockEvent;

    [Header("References")]
    [Tooltip("Drag a reference to the bow base gameobject here.")]
    public Bow_XRInteractable myBow; //Only nock-able if bow is held. Will kick an arrow out of socket if bow is dropped.

    private bool bowHeld = false;

    //A reference to the arrow that (may) be socketed in this interactor. So the string can launch it when ready.
    private Arrow_XRInteractable nockedArrow = null;
    public Arrow_XRInteractable CurrentArrow { get { return nockedArrow; } }

    //Added some simple audio for extra pazazz! On nock and on launch
    private SimpleAudioEmitter audioEmitter;

    protected override void Awake()
    {
        base.Awake();

        if (NockEvent == null)
            NockEvent = new ArrowNockedEvent();

        audioEmitter = GetComponent<SimpleAudioEmitter>();
    }

    protected override void Start()
    {
        base.Start();
        myBow.GrabEvent.AddListener(SetBowHeld);
    }

    /// <summary>
    /// Overriding the normal method for when an interactable hovers over the socket. Will force the arrow out of the hand and onto the nock.
    ///  The logic for ensuring the interactable is an arrow is handled in CanHover() and CanSelect().
    /// </summary>
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);

        //Only care if the arrow is being held in a hand currently.
        XRBaseInteractable interactable = args.interactableObject as XRBaseInteractable;
        XRBaseInteractor interactor_other = interactable.firstInteractorSelecting as XRBaseInteractor;

        if (interactor_other != null && interactor_other.TryGetComponent<XRBaseController>(out _))
        {
            //Force the hand to deselect the arrow and force the nock to select the arrow. Arrow is now nocked
            interactor_other.interactionManager.SelectExit(interactor_other, interactable as IXRSelectInteractable);
            interactionManager.SelectEnter(this, interactable as IXRSelectInteractable);
            //Play a nock-click audioclip
            audioEmitter.PlayNoise("Click");
        }
    }

    /// <summary>
    /// Overriding the CanHover method to only allow arrows and also to ensure the bow is held.
    /// </summary>
    public override bool CanHover(IXRHoverInteractable interactable)
    {
        Arrow_XRInteractable arrow = interactable as Arrow_XRInteractable;

        return base.CanHover(interactable) && bowHeld && (arrow != null);
    }

    /// <summary>
    /// Overriding the definition of CanSelect for the nock to be both CanSelect && CanHover. This is because we can supply a value to the SocketInteractor
    ///  that prevents an item from hovering on the socket for a set time after an object is removed. This will allow the arrow to clear out and launch
    ///  without being "captured" by the nock/socket immediately.
    /// </summary>
    public override bool CanSelect(IXRSelectInteractable interactable)
    {
        return base.CanSelect(interactable) && CanHover(interactable as IXRHoverInteractable);
    }

    /// <summary>
    /// Because we've overridden the logic in CanHover and CanSelect, the only way an interactable can be selected with this socket
    ///  is if the interactable is a Arrow_XRInteractable. So we don't need to do any checking here!
    /// </summary>
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        nockedArrow = args.interactableObject as Arrow_XRInteractable;
        NockEvent.Invoke(true);
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        nockedArrow = null;
        NockEvent.Invoke(false);
    }

    //This method is simply the callback for the Event the Bow invokes when it is picked up or dropped
    private void SetBowHeld(bool val)
    {
        bowHeld = val;
    }

    public void LaunchArrow(float drawValue)
    {
        nockedArrow.Launch(drawValue);
        audioEmitter.PlayNoise("Release");
    }
}
