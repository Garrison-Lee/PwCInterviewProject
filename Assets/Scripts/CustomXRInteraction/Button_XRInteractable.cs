using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Script on the button that is behind the player and allows them to start a time trial
/// </summary>
public class Button_XRInteractable : XRBaseInteractable
{
    [Header("Time Trial Button Settings")]
    [Tooltip("How many seconds should the time trial last?")]
    public float timeTrialDuration = 60f;
    [Tooltip("How many seconds should the button take to press/depress? This is just an animation")]
    public float pressDuration = 0.15f;

    //These two together ensure the animation of the button moving can finish first and then it can
    // fire again if needed depending on the hand's state/position. So pressing the button again during
    // an animation won't hurt anything.
    private bool buttonPressed = false;
    private bool buttonHovered = false; //not quite the same as isHovered bc we check which controller

    private Transform parentTransform;
    private Vector3 positionNormal = new Vector3(0, 1f, 0);
    private Vector3 positionPressed = new Vector3(0, 0.85f, 0);

    private Scoreboard scoreboard;

    private void Start()
    {
        parentTransform = transform.parent.parent;
        int rightHanded = PlayerPrefs.GetInt("RightHanded", 1);

        Transform targetTransform;
        if (rightHanded > 0)
            targetTransform = parentTransform.Find("RightHanded");
        else
            targetTransform = parentTransform.Find("LeftHanded");

        parentTransform.Find("Column").position = targetTransform.position + Vector3.up * 0.7f;
        parentTransform.Find("Column").rotation = targetTransform.rotation;

        //There's only one scoreboard in scene
        scoreboard = FindObjectOfType<Scoreboard>();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        
        //Have to physically 'press' this button with the directInteractor hand
        XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;
        if (directInteractor != null) //Hovered by direct hand
        {
            //This flag is independent from button state so we can trigger coroutines as needed
            buttonHovered = true;
            PressButton();
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        //Have to physically 'press' this button with the directInteractor hand
        XRDirectInteractor directInteractor = args.interactorObject as XRDirectInteractor;
        if (directInteractor != null) //Hovered by direct hand
        {
            buttonHovered = false;
            UnpressButton();
        }
    }

    private void PressButton()
    {
        //A hover has been received but only do something if the button is currently not pressed. Otherwise
        // our button must be animating
        if (!buttonPressed)
        {
            StartCoroutine(PressRoutine());
        }
    }

    private void UnpressButton()
    {
        if (buttonPressed)
        {
            StartCoroutine(UnpressRoutine());
        }
    }

    //Routine allows movement over time. ie "animate" the button
    private IEnumerator PressRoutine()
    {
        float duration = 0f;

        //Take pressDuration seconds to move down into a depressed state
        while (duration < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(positionNormal, positionPressed, duration / pressDuration);

            duration += Time.deltaTime;
            yield return null;
        }

        //Once animation is over, we set this flag to true
        buttonPressed = true;

        //Start the actual time trial
        StartTimeTrial();

        //If the hand has been removed during the animation, we'll immediately start the UnPress sequence
        if (!buttonHovered)
        {
            UnpressButton();
        }
    }

    //See above. The same, but in reverse.
    private IEnumerator UnpressRoutine()
    {
        float duration = 0f;

        while (duration < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(positionNormal, positionPressed, (pressDuration - duration) / pressDuration);

            duration += Time.deltaTime;
            yield return null;
        }

        buttonPressed = false;

        if (buttonHovered)
        {
            PressButton();
        }
    }

    /// <summary>
    /// Method that actually starts the time trial
    /// </summary>
    private void StartTimeTrial()
    {
        scoreboard.ResetScore();
        scoreboard.StartTimer(timeTrialDuration);
    }
}
