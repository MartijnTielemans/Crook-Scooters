using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] AudioSource menuSelect;

    public void OnSumbit()
    {
        StartCoroutine(LoadScene());
    }

    public void OnCancel()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        menuSelect.Play();
        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);

        SceneManager.LoadScene("MainMenu");
    }
}
