using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// A script for capturing and handling any input from the VR devices that we want to handle manually. Such as opening the menu.
///  Ended up hardly using this script but I won't tear it out because it could be useful for extended functionality.
/// </summary>
public class InputManager : MonoBehaviour
{
    //Devices we're tracking input for
    InputDevice headset;
    InputDevice rightHand;
    InputDevice leftHand;

    //Things we want to send input to
    public MenuManager menu;

    void Start()
    {
        //Simple characteristics that will work with a basic headset + hand controllers set up. We will just use the first device found that matches criteria for each device.
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDeviceCharacteristics headsetCharacteristics = InputDeviceCharacteristics.HeadMounted;

        //Re-usable list used to find compatible controllers. We'll then store a reference to the first matching device for each of the three devices we want to track.
        List<InputDevice> matchingDevices = new List<InputDevice>();

        //Finding headset
        InputDevices.GetDevicesWithCharacteristics(headsetCharacteristics, matchingDevices);
        if (matchingDevices.Count > 0)
            headset = matchingDevices[0];
        else
            Debug.LogError("Did not find a device to use as the Headset");

        //Finding right handed controller
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, matchingDevices);
        if (matchingDevices.Count > 0)
            rightHand = matchingDevices[0];
        else
            Debug.LogError("Did not find a device to use as the RightHandController");

        //Finding left handed controller
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, matchingDevices);
        if (matchingDevices.Count > 0)
            leftHand = matchingDevices[0];
        else
            Debug.LogError("Did not find a device to use as the LeftHandController");
    }

    // Update is called once per frame
    void Update()
    {
        //If left hand controller presses menu button, toggle the menu in the game
        if (leftHand.TryGetFeatureValue(CommonUsages.menuButton, out bool menuButtonPressed) && menuButtonPressed)
        {
            menu.OpenMenu();   
        }
    }
}
