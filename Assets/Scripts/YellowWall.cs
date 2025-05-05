using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowWall : MonoBehaviour
{
    [Header("Local References")]
    [SerializeField] private AudioSource doorSound;
    [SerializeField] private ReflectionProbe probe;
    [SerializeField] private GameObject glassWall;

    private void Start()
    {
        glassWall.SetActive(false);

        // Log current color to help debug if needed
        var wallColor = GetComponent<Renderer>()?.material?.color;
        if (wallColor != null)
        {
            Debug.Log($"[YellowWall] Current wall color is: {wallColor}");
        }
    }

    private void Update()
    {
        // Manual test input
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(OpenDoor());
        }*/
    }

    public IEnumerator OpenDoor()
    {
        Debug.Log("[YellowWall] Sliding wall opening...");

        // Show the glass wall
        glassWall.SetActive(true);

        // Play door sound
        if (doorSound != null)
            doorSound.Play();

        // Wait before moving (to sync with sound)
        yield return new WaitForSeconds(7);

        // Slide the wall upward
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, 15, 0);
        float duration = 13f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;

            if (probe != null)
                probe.RenderProbe();
        }

        // Ensure final position is exact
        transform.position = endPos;

        if (probe != null)
            probe.RenderProbe();

        // Disable the wall so it can't be interacted with again
        gameObject.SetActive(false);

        Debug.Log("[YellowWall] Wall finished sliding and is now inactive.");
    }
}
