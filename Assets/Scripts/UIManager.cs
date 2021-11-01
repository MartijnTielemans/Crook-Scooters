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
    [SerializeField] TextMeshProUGUI gameTimerText;

    public void JoinCanvasVisibility(bool visible)
    {
        joinCanvas.GetComponent<Animator>().Play("CanvasLeave");
    }

    public void GameCanvasVisibility(bool visible)
    {
        gameCanvas.GetComponent<Animator>().Play("CanvasLeave");
    }

    public void DisplayJoinTimer(float value)
    {
        joinTimerText.text = value.ToString();
    }

    public void DisplayGameTimer(float value)
    {
        gameTimerText.text = value.ToString();
    }
}
