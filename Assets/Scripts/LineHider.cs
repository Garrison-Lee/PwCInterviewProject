using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// This script is super simple. It just hides the raycast lines for the hands when something is actually selected. Methods are called
///  using the events that are part of the XRRayInteractor component on either hand controller
/// </summary>
public class LineHider : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.XRInteractorLineVisual lineVisual;

    private void Awake()
    {
        lineVisual = GetComponent<UnityEngine.XR.Interaction.Toolkit.XRInteractorLineVisual>();
    }

    public void HideLine()
    {
        lineVisual.enabled = false;
    }

    public void ShowLine()
    {
        lineVisual.enabled = true;
    }
}
