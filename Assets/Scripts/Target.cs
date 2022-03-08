using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class controlling the targets in this project. Adds to score, could be set up to do sfx, vfx, whatever. Other things can also
///  implement the IShootable interface and arrows will stick into them etc.
/// </summary>
public class Target : MonoBehaviour, IShootable
{
    [Header("Target Settings")]
    [Tooltip("How many points is this target worth when hit?")]
    public int pointValue = 1;
    [Tooltip("What color should the target appear as when hit?")]
    public Color hitColor;
    [Tooltip("How many seconds should a target take to fade after being hit?")]
    public float hitColorDuration = 1f;

    protected static Scoreboard scoreboard;
    protected const string floatingScorePath = "Canvas/FloatingScoreText";
    protected static Color badColor = new Color(1, 0, 0, 0.5f);

    private Text floatingText;
    private Material mat;
    private Color defaultColor;

    //Added audio for extra pazazz. On Good hit and Bad hit
    private SimpleAudioEmitter audioEmitter;

    private void Awake()
    {
        //There's only one of these so this will work fine for the scope of this project
        scoreboard = FindObjectOfType<Scoreboard>();

        floatingText = transform.Find(floatingScorePath).GetComponent<Text>();
        floatingText.text = "+" + pointValue.ToString();
        mat = GetComponent<MeshRenderer>().material;
        defaultColor = mat.color;

        audioEmitter = GetComponent<SimpleAudioEmitter>();
    }

    //The arrow will tell the target when it needs to "GetShot"
    public void GetShot()
    {
        //Player out of bounds, invalid shot
        if (!PlayerInBounds.InBounds)
        {
            StartCoroutine(ShowBadCouroutine());
            audioEmitter.PlayNoise("Bad");
        }
        //Player in bounds, valid shot
        else
        {
            scoreboard.AddScore(pointValue);
            StartCoroutine(ShowHitCoroutine());
            audioEmitter.PlayNoise("Good");
        }
    }

    //A little coroutine to indicate the target has been struck. Shows a color and how many points were received.
    private IEnumerator ShowHitCoroutine()
    {
        float duration = 0;
        floatingText.text = "+" + pointValue.ToString();
        mat.color = hitColor;
        floatingText.color = hitColor;

        while (duration < hitColorDuration)
        {
            mat.color = Color.Lerp(hitColor, defaultColor, duration / hitColorDuration);
            floatingText.color = Color.Lerp(hitColor, Color.clear, duration / hitColorDuration);

            duration += Time.deltaTime;
            yield return null;
        }
    }

    //A similar coroutine except this one shows that the shot was invalid due to being out of bounds
    private IEnumerator ShowBadCouroutine()
    {
        float duration = 0;
        floatingText.text = "+0";
        mat.color = badColor;
        floatingText.color = badColor;

        while (duration < hitColorDuration)
        {
            mat.color = Color.Lerp(badColor, defaultColor, duration / hitColorDuration);
            floatingText.color = Color.Lerp(badColor, Color.clear, duration / hitColorDuration);

            duration += Time.deltaTime;
            yield return null;
        }
    }
}
