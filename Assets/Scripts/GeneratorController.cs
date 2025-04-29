using UnityEngine;
using System.Collections;
using TMPro;

public class GeneratorController : MonoBehaviour
{
    public Light[] ceilingSpotlights; // Assign ceiling spotlights in the Inspector
    public GameObject[] ceilingLightCylinders; // Assign the ceiling light cylinder objects
    public Light generatorSpotlight; // Assign the generator's spotlight
    public Light[] generatorPointLights; // Assign the generator's point lights
    public float generatorDuration = 30f; // Duration generator stays on
    public AudioSource startSound; // Sound for starting the generator
    public AudioSource idleSound; // Looping sound when the generator is active
    public AudioSource stopSound; // Sound for turning off the generator
    public TextMeshProUGUI interactPrompt; // UI prompt for interaction
    public AudioSource ceilingAudio; // Assign the ceiling AudioSource in the Inspector
    public Material lightMaterial; //Added a light material to be toggled for faster execution


    private bool isNearGenerator = false; // Tracks if player is near the generator
    private bool isGeneratorOn = false; // Tracks if the generator is active

    private void Start()
    {
        // Ensure generator lights are ON initially
        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights)
        {
            pointLight.enabled = true;
        }

        // Ensure ceiling light cylinders and spotlights are OFF initially
        foreach (GameObject lightCylinder in ceilingLightCylinders)
        {
            //lightCylinder.SetActive(false);
            //lightCylinder.GetComponent<Material>().DisableKeyword("_EMISSION");
        }
        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights)
        {
            spotlight.enabled = false;
        }

        // Interaction prompt is hidden initially
        interactPrompt.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Check for interaction when the player is near and generator is off
        if (isNearGenerator && !isGeneratorOn && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(TurnOnGenerator());
        }
    }

    private IEnumerator TurnOnGenerator()
    {
        // Play start sound
        startSound.Play();
        yield return new WaitForSeconds(startSound.clip.length); // Wait for start sound to finish

        // Turn on generator
        isGeneratorOn = true;

        // Turn off generator-specific lights
        generatorSpotlight.enabled = false;
        foreach (Light pointLight in generatorPointLights)
        {
            pointLight.enabled = false;
        }

        // Enable ceiling audio and play idle sound
        ceilingAudio.Play(); // Start ceiling audio
        idleSound.Play();

        // Enable ceiling lights and their cylinders
        foreach (GameObject lightCylinder in ceilingLightCylinders)
        {
            //lightCylinder.SetActive(true);
            //lightCylinder.GetComponent<Material>().EnableKeyword("_EMISSION");
        }
        lightMaterial.EnableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights)
        {
            spotlight.enabled = true;
        }

        // Wait for most of the generator's duration
        yield return new WaitForSeconds(generatorDuration - stopSound.clip.length);

        // Play stop sound
        idleSound.Stop(); // Stop the idle sound first
        stopSound.Play();

        // Wait for stop sound to finish before stopping ceiling audio
        yield return new WaitForSeconds(stopSound.clip.length);

        // Stop ceiling audio
        ceilingAudio.Stop();

        // Turn off generator
        isGeneratorOn = false;

        // Turn back on generator-specific lights
        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights)
        {
            pointLight.enabled = true;
        }

        // Disable ceiling lights and their cylinders
        foreach (GameObject lightCylinder in ceilingLightCylinders)
        {
            //lightCylinder.SetActive(false);
            //lightCylinder.GetComponent<Material>().DisableKeyword("_EMISSION");
        }
        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights)
        {
            spotlight.enabled = false;
        }

        // Reactivation: Player needs to return to the generator to restart
        interactPrompt.gameObject.SetActive(true); // Show prompt again
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearGenerator = true;
            if (!isGeneratorOn)
            {
                interactPrompt.gameObject.SetActive(true); // Show the prompt
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearGenerator = false;
            interactPrompt.gameObject.SetActive(false); // Hide the prompt
        }
    }
}
