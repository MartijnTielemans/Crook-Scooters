using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIcons : MonoBehaviour
{
    [SerializeField] GameObject[] playerIcons;

    public void UpdatePlayerIcons(int activePlayers)
    {
        for (int i = 0; i < playerIcons.Length; i++)
        {
            playerIcons[i].SetActive(false);
        }

        for (int i = 0; i < activePlayers; i++)
        {
            playerIcons[i].SetActive(true);
        }
    }
}
