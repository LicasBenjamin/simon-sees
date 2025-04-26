using UnityEngine;
using System.Collections;

public class LightsFlickering : MonoBehaviour
{
    public Light[] spotlights; // Assign spotlights in the Inspector
    public SpriteRenderer monsterSprite; // Assign the monster's SpriteRenderer in the Inspector
    public AudioSource buzzingSound; // Assign the AudioSource for buzzing sound
    public float minFlickerTime = 2f; // Minimum time between flickers
    public float maxFlickerTime = 5f; // Maximum time between flickers

    private void Start()
    {
        // Ensure lights start as ON and monster is NOT visible
        foreach (Light spotlight in spotlights)
        {
            spotlight.enabled = true;
        }
        monsterSprite.enabled = false;

        // Start the flickering routine
        StartCoroutine(FlickerRoutine());
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // Random time before the next flicker
            yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));

            // Turn lights OFF briefly
            foreach (Light spotlight in spotlights)
            {
                spotlight.enabled = false;
            }

            // Play buzzing sound (if assigned) and show the monster for 0.1 seconds
            if (buzzingSound != null)
            {
                buzzingSound.Pause(); // Stop buzzing sound when lights go off
            }

            monsterSprite.enabled = true; // Show the monster
            yield return new WaitForSeconds(0.2f); // Monster visible for varied seconds

            // Turn lights back ON
            foreach (Light spotlight in spotlights)
            {
                spotlight.enabled = true;
            }

            monsterSprite.enabled = false; // Hide the monster

            // Resume buzzing sound (if assigned)
            if (buzzingSound != null)
            {
                buzzingSound.Play();
            }
        }
    }
}
