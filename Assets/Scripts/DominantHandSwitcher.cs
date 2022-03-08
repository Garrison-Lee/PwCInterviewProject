using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a small script that simply allows the user to choose between left handed and right handed dominant controls.
/// </summary>
[DefaultExecutionOrder(-500)]
public class DominantHandSwitcher : MonoBehaviour
{
    //The gameObjects we want to turn on for right handed players
    GameObject rightHand_R;
    GameObject leftHand_R;
    //The gameObjects we want to turn on for left handed players
    GameObject rightHand_L;
    GameObject leftHand_L;

    private const string path = "VR Origin/Camera Offset/";

    private void Awake()
    {
        rightHand_R = GameObject.Find(path + "RightHand_R");
        leftHand_R = GameObject.Find(path + "LeftHand_R");
        rightHand_L = GameObject.Find(path + "RightHand_L");
        leftHand_L = GameObject.Find(path + "LeftHand_L");

        int rightHanded = PlayerPrefs.GetInt("RightHanded", 1);
        if (rightHanded > 0)
        {
            rightHand_L.SetActive(false);
            leftHand_L.SetActive(false);
        }
        else
        {
            rightHand_R.SetActive(false);
            leftHand_R.SetActive(false);
        }
    }
}
