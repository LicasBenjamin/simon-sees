using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowWall : MonoBehaviour
{
    [SerializeField] private AudioSource doorSound;
    [SerializeField] private ReflectionProbe probe;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(OpenDoor());
        }
    }
    IEnumerator OpenDoor()
    {
        doorSound.Play();
        yield return new WaitForSeconds(7);

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 15, 0);
        float duration = 13f; // how long the door should take to open
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
            probe.RenderProbe();
        }
        probe.RenderProbe();
        transform.position = endPos; // Ensure it ends at the exact final position
    }
}
