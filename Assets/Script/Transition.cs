using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public float waitTime;
    public Transform center;
    public Transform left;
    public Transform right;

    public DoorManager doorManager;
    void Start()
    {
        StartCoroutine(In());
    }

    public IEnumerator In()
    {
        Vector3 Gotoposition = left.position;
        float elapsedTime = 0;
        Vector3 currentPos = center.position;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(SceneManager.GetActiveScene().buildIndex != 0)doorManager.transiOver = true;
        yield return null;
    }
    public void Out(int sceneToLoad)
    {
        StartCoroutine(OutCoro(sceneToLoad));
    }
    IEnumerator OutCoro(int sceneToLoad)
    {
        Vector3 Gotoposition = center.position;
        float elapsedTime = 0;
        Vector3 currentPos = right.position;

        while (elapsedTime < waitTime)
        {
            transform.position = Vector3.Lerp(currentPos, Gotoposition, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
        yield return null;
    }
}
