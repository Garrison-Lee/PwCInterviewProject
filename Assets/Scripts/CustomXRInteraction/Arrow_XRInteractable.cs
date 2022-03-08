using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// This or the string is the most complicated script. This script is attached to each of the arrows in the scene.
///  Arrows are interactables. They can be grabbed by a hand or 'grabbed' by the nock/socketInteractor. They contain
///  their own logic for how to fly and how to stick into things.
/// </summary>
public class Arrow_XRInteractable : XRGrabInteractable
{
    [Header("Arrow Flight Settings")]
    [Tooltip("How much force should be applied to the arrow on a full draw? Lower force when bow is only partially drawn.")]
    public float arrowMaxVelocity = 1f;
    [Tooltip("Controls how fast the arrow spins, would normally be caused by the fletching dragging through air.")]
    public float arrowSpin_MIN = 100f; //Controls how fast the arrow spins
    public float arrowSpin_MAX = 2000f;

    //RigidBody to add force and torque to fly/rotate
    private Rigidbody rb;
    //Tracking the tip's transform because collision detection is spotty at best and doing a line cast from last position to current
    // position seems to be more reliable
    private Transform tip;
    private Vector3 lastPosition;

    //Need to do things in Update() but only if the arrow is currently in flight
    private bool inFlight = false;
    //This shouldn't even really be necessary because I have physics layer matrix set up in Editor but call me thorough
    private int collisionLayerMask;

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        tip = transform.Find("CollisionTip");
        collisionLayerMask = LayerMask.GetMask("Target"); //So we can only "stick into" things on the Target layer
    }

    /// <summary>
    /// Called by the string when it is released. Sets the arrow into flight
    /// </summary>
    /// <param name="drawForce">Normalized value of how far the back the string is drawn</param>
    public void Launch(float drawForce)
    {
        //Only continue launching if the arrow is currently nocked, otherwise, it can't launch. It's called throwing. You're just throwing an arrow at this point...
        XRBaseInteractor interactor = firstInteractorSelecting as XRBaseInteractor;
        if (interactor != null && interactor.TryGetComponent<Nock_XRInteractor>(out _))
        {
            //First force-deselect the arrow from the nock interactor so that it is free to fly!
            interactionManager.SelectExit(interactor as IXRSelectInteractor, this);
            //Changes some physics settings and sets bools accordingly
            ReadyArrowForFlight();

            //Just drop the arrow if the bow is hardly drawn
            if (drawForce < 0.03f)
                return;

            //Now let's launch it in the direction it's facing, super simple. Force will be a function of pull/draw distance
            rb.AddForce(-transform.right * arrowMaxVelocity * drawForce, ForceMode.Impulse);
            //Arrows spin in flight due to the curved fletching and air resistance. I'll just add some torque so it doesn't look too weird.
            rb.AddTorque(transform.right * Random.Range(arrowSpin_MIN, arrowSpin_MAX), ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Basically the Update() function that ensures the XRInteraction stuff has been recently updated
    /// </summary>
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (inFlight) //Only need to do all this calculation when the arrow is in flight
        {
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic) //Update()
            {
                //Manual collision checking
                if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hit, collisionLayerMask, QueryTriggerInteraction.Ignore))
                {
                    if (hit.transform.TryGetComponent(out IShootable shootable))
                    {
                        StickArrow(hit.transform);
                        shootable.GetShot();
                    }
                }

                lastPosition = tip.position;
            }

            //This simulates feather drag. Makes the tip "heavier" by pointing the arrow in the direction of flight
            //NOTE: this area is slightly janky at the edge cases. Could use refinement in bigger project. Adds a lot of juice to most shots though.
            if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed) //FixedUpdate()
            {
                if (rb.velocity.magnitude >= 10f) //If we're moving sufficiently fast
                {
                    transform.right = -rb.velocity;
                }

                if (rb.velocity.magnitude == 0) //arrow has come to a stop and hasn't triggered the sticking logic above. hit the ground/missed
                {
                    ResetArrow();
                }
            }
        }
        
    }

    //NOTE: Targets were too thin, or possibly the non-convex issue. Regardless, manually doing a linecast to detect sticking collisions. Letting physics handle normal bouncing
    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Check and see if whatever we've just hit is an intended target. Normal physics will apply otherwise but if it's a shootable target
    //    // then we'll stick our arrow into it and tell the target it just got shot.
    //    if (collision.gameObject.TryGetComponent(out IShootable shootable))
    //    {
    //        //Stick the arrow into the target and make it stop moving
    //        StickArrow(collision.transform);

    //        shootable.GetShot(); //Shootables will add some score to the scoreboard or something later once I work on that
    //    }
    //}

    //The method that will "stick" the arrow into whatever it hit by using parenting and turning the rb to kinematic that doesn't use gravity
    private void StickArrow(Transform newParent)
    {
        SetPhysics(false);
        transform.parent = newParent;
        inFlight = false;
    }

    //Resets physics to be ready for a rb.AddForce. ie it is no longer kinematic and does use gravity
    private void ReadyArrowForFlight()
    {
        SetPhysics(true);

        inFlight = true;
        transform.parent = null;

        lastPosition = tip.position;
    }

    //Resets arrow to initial state
    private void ResetArrow()
    {
        SetPhysics(true);

        inFlight = false;
        transform.parent = null;
    }

    //Turn "physics" off when we stick it into the target. Re-enable if we use an object pool or something to re-use arrows.
    private void SetPhysics(bool val)
    {
        rb.isKinematic = !val;
        rb.useGravity = val;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        ResetArrow();
    }
}
