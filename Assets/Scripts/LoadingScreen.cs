using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject transition;
    [SerializeField] string sceneToLoad;
    [SerializeField] float timer = 4;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene(timer));
    }

    IEnumerator LoadScene(float timer)
    {
        yield return new WaitForSeconds(timer);

        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);

        SceneManager.LoadScene(sceneToLoad);
    }
}
