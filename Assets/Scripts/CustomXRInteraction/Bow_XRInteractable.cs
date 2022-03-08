using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This script is on the bow parent transform. It represents the bow shaft. It is, of course,
///  a GrabInteractable and that contains most of the functionality the bowShaft needs. However,
///  we've added a custom boolean event to the script so that the string&nock can know when the bow
///  is held to change their behavior.
/// </summary>
public class Bow_XRInteractable : XRGrabInteractable
{
    //Boolean event that the string&nock will subscribe to
    [System.Serializable]
    public class BowGrabbedEvent : UnityEvent<bool> { }
    public BowGrabbedEvent GrabEvent;

    protected override void Awake()
    {
        base.Awake();

        if (GrabEvent == null)
            GrabEvent = new BowGrabbedEvent();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        //If the bow is selected by a controller, the bow is now held
        if (args.interactorObject.transform.TryGetComponent(out XRBaseController _))
        {
            GrabEvent.Invoke(true);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        //If the bow is no longer selected, it has been dropped
        GrabEvent.Invoke(false);
    }
}
