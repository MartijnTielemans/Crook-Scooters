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

    [Space]
    [SerializeField] int gameTimerSize;
    [SerializeField] int timerDecimalSize;

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
        joinTimerText.text = MakeDecimalSmaller(value.ToString("F1"), 2, (int)(timerDecimalSize * 1.5f));
    }

    public void DisplayPlayersJoinedText(string text)
    {
        playersJoinedText.text = text;
    }

    public void DisplayGameTimer(float value)
    {
        gameTimerText.text = MakeDecimalSmaller(value.ToString("F2"), 3, timerDecimalSize);
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

    string MakeDecimalSmaller(string value, int decimalCharacters, int fontSizeDecrease)
    {
        string text = value;
        string finalText = "";
        bool doOnce = false;

        // Displays decimals as a smaller font size
        for (int i = 0; i < text.Length; i++)
        {
            if (i < text.Length - decimalCharacters)
            {
                finalText = finalText + text[i];
            }
            else
            {
                // Only add this once
                if (!doOnce)
                    finalText = finalText + $"<size=-{fontSizeDecrease}>";

                doOnce = true;
                finalText = finalText + text[i];
            }
        }

        return finalText;
    }
}
