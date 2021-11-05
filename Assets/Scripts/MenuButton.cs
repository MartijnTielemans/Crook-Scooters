using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] float timer = .5f;
    bool canPress = true;

    public void StartButton()
    {
        if (canPress)
            StartCoroutine(StartSequence());
    }

    public void HelpButton()
    {
        if (canPress)
            StartCoroutine(HelpSequence());
    }

    public void ExitButton()
    {
        if (canPress)
            StartCoroutine(ExitSequence());
    }

    IEnumerator StartSequence()
    {
        canPress = false;
        yield return new WaitForSeconds(timer);

        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);

        SceneManager.LoadScene("LoadingScreen");
    }

    IEnumerator HelpSequence()
    {
        canPress = false;
        yield return new WaitForSeconds(timer);

        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);

        SceneManager.LoadScene("HelpScene");
    }

    IEnumerator ExitSequence()
    {
        canPress = false;
        yield return new WaitForSeconds(timer);

        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);

        Application.Quit();
    }
}
