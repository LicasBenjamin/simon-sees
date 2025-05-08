using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MoveAndTransition : MonoBehaviour
{
    public Vector3 targetPosition = new Vector3(0, 0, 0);
    public float speed = 2.0f;
    public AudioSource audioSource;

    void Start()
    {
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        audioSource.Play();
        // Move towards the target position
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPosition;

        SceneManager.LoadScene("Menu");
    }
}
