using UnityEngine;
using System.Collections;
using TMPro;

public class GeneratorController : MonoBehaviour {
    [Header("Lighting")]
    public Light[] ceilingSpotlights;
    public GameObject[] ceilingLightCylinders;
    public Light generatorSpotlight;
    public Light[] generatorPointLights;
    public Material lightMaterial;

    [Header("Audio")]
    public AudioSource startSound;
    public AudioSource idleSound;
    public AudioSource stopSound;
    public AudioSource ceilingAudio;
    public AudioSource errorSound;

    [Header("UI")]
    public TextMeshProUGUI interactPrompt;
    public GameObject miniGameUI;

    [Header("Settings")]
    public float generatorDuration = 30f;

    private bool isNearGenerator = false;
    private bool isGeneratorOn = false;
    private bool generatorBroken = false;
    private bool miniGameActive = false;
    private bool hasBeenTurnedOnOnce = false;

    void Start() {
        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights) {
            pointLight.enabled = true;
        }

        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights) {
            spotlight.enabled = false;
        }

        interactPrompt.gameObject.SetActive(false);
        miniGameUI.SetActive(false);
    }

    void Update() {
        if (isNearGenerator && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("E pressed near generator");

            if (generatorBroken && !miniGameActive) {
                Debug.Log("Mini-game should start now");
                StartMiniGame();
            } else if (!isGeneratorOn && !generatorBroken) {
                StartCoroutine(TurnOnGenerator());
            }
        }
    }

    public void BreakGenerator() {
        Debug.Log($"Break Attempt: On={isGeneratorOn}, Broken={generatorBroken}, HasRunOnce={hasBeenTurnedOnOnce}");

        if (generatorBroken || !isGeneratorOn) {
            Debug.Log("Skipping break: already broken or generator is off.");
            return;
        }

        generatorBroken = true;
        isGeneratorOn = false;
        hasBeenTurnedOnOnce = true;

        Debug.Log("Generator has been broken.");

        idleSound.Stop();
        ceilingAudio.Stop();
        stopSound.Play();

        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights) {
            spotlight.enabled = false;
        }

        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights) {
            pointLight.enabled = true;
        }

        interactPrompt.text = "[E] Fix Generator";
        interactPrompt.gameObject.SetActive(isNearGenerator);
    }

    public void StartMiniGame() {
        Debug.Log("StartMiniGame() called");

        if (miniGameUI != null) {
            miniGameUI.SetActive(true);
            Debug.Log("Mini-game UI activated");
        } else {
            Debug.LogWarning("MiniGame UI is NOT assigned!");
        }

        miniGameActive = true;
        interactPrompt.gameObject.SetActive(false);
    }

    public void CompleteMiniGame() {
        miniGameUI.SetActive(false);
        miniGameActive = false;
        generatorBroken = false;

        if (interactPrompt != null) {
            interactPrompt.text = "Generator Fixed!";
            interactPrompt.gameObject.SetActive(true);
            StartCoroutine(HidePromptAfterDelay(2f));
        }

        StartCoroutine(TurnOnGenerator());
    }

    private IEnumerator HidePromptAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        interactPrompt.gameObject.SetActive(false);
    }

    public void PlayErrorSound() {
        if (errorSound != null) errorSound.Play();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isNearGenerator = true;
            if (!isGeneratorOn || generatorBroken) {
                interactPrompt.gameObject.SetActive(true);
                interactPrompt.text = generatorBroken ? "[E] Fix Generator" : "[E] Start Generator";
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isNearGenerator = false;
            interactPrompt.gameObject.SetActive(false);
        }
    }

    private IEnumerator TurnOnGenerator() {
        startSound.Play();
        yield return new WaitForSeconds(startSound.clip.length);

        isGeneratorOn = true;
        hasBeenTurnedOnOnce = true;

        generatorSpotlight.enabled = false;
        foreach (Light pointLight in generatorPointLights) {
            pointLight.enabled = false;
        }

        lightMaterial.EnableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights) {
            spotlight.enabled = true;
        }

        ceilingAudio.Play();
        idleSound.Play();

        yield return new WaitForSeconds(generatorDuration - stopSound.clip.length);

        idleSound.Stop();
        stopSound.Play();
        yield return new WaitForSeconds(stopSound.clip.length);

        ceilingAudio.Stop();

        // New logic: treat shutdown as a breakdown
        isGeneratorOn = false;
        generatorBroken = true;
        miniGameActive = false;

        Debug.Log("Generator shut down automatically — now considered broken.");

        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights) {
            spotlight.enabled = false;
        }

        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights) {
            pointLight.enabled = true;
        }

        if (isNearGenerator && interactPrompt != null) {
            interactPrompt.text = "[E] Fix Generator";
            interactPrompt.gameObject.SetActive(true);
        }
    }

    public bool IsGeneratorOn() {
        return isGeneratorOn;
    }
}
