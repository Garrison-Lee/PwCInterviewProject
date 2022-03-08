using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This class just contains functionality for the little menu in the game. Toggle menu, reload scene, quit game, etc.
/// </summary>
public class MenuManager : MonoBehaviour
{
    GameObject mainCanvas;
    GameObject helpPage;

    private void Awake()
    {
        mainCanvas = GetComponent<Canvas>().gameObject;
        helpPage = transform.Find("Background_Help").gameObject;
    }

    /// <summary>
    /// Opens the menu in above the target in the scene
    /// </summary>
    public void OpenMenu()
    {
        mainCanvas.SetActive(true);
    }
    public void CloseMenu()
    {
        mainCanvas.SetActive(false);
    }

    /// <summary>
    /// Reloads the current scene. In case the bow goes out of range etc.
    /// </summary>
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the game depending on what mode we're in. Editor vs standalone player etc.
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /// <summary>
    /// Resumes the game. In the context of this mini project that means closing the menu
    /// </summary>
    public void ResumeGame()
    {
        CloseMenu();
    }

    /// <summary>
    /// Left hand and right hand devices are different types of interactors. This button allows you to flip it.
    /// </summary>
    public void ChangeHands()
    {
        int rightHanded = PlayerPrefs.GetInt("RightHanded", 1);
        if (rightHanded > 0)
            PlayerPrefs.SetInt("RightHanded", 0);
        else
            PlayerPrefs.SetInt("RightHanded", 1);

        ResetScene();
    }

    public void ToggleHelpDisplay()
    {
        helpPage.SetActive(!helpPage.activeInHierarchy);
    }
}
