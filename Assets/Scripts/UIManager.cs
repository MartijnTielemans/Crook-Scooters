using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject joinCanvas;
    [SerializeField] GameObject gameCanvas;

    [SerializeField] TextMeshProUGUI joinTimerText;
    [SerializeField] TextMeshProUGUI playersJoinedText;
    [SerializeField] TextMeshProUGUI gameTimerText;
    [SerializeField] TextMeshProUGUI gameEndText;

    [SerializeField] Animator quickRestartCanvas;
    [SerializeField] Animator speedLinesAnim;

    public void JoinCanvasAnimation(string anim)
    {
        joinCanvas.GetComponent<Animator>().Play(anim);
    }

    public void GameCanvasAnimation(string anim)
    {
        gameCanvas.GetComponent<Animator>().Play(anim);
    }

    public void DisplayJoinTimer(float value)
    {
        joinTimerText.text = value.ToString("F1");
    }

    public void DisplayPlayersJoinedText(string text)
    {
        playersJoinedText.text = text;
    }

    public void DisplayGameTimer(float value)
    {
        gameTimerText.text = value.ToString("F1");
    }

    public void ShowGameEndText(string text)
    {
        gameEndText.enabled = true;
        gameEndText.text = text;
    }

    public void ShowQuickRestartCanvas()
    {
        quickRestartCanvas.Play("QuickRestart_In");
    }

    public void PlaySpeedLines()
    {
        speedLinesAnim.Play("Speedlines_Flash");
    }
}
