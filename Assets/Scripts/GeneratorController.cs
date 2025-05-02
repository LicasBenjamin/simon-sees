using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

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
    public AudioSource powerDownSound;

    [Header("UI")]
    public TextMeshProUGUI interactPrompt;
    public GameObject miniGameUI;
    public GameObject colorUI; // ✅ NEW: assign color reticle UI

    [Header("Settings")]
    public float generatorMaxDuration = 100f;
    public float generatorDurationLeft = 0;

    private bool isNearGenerator = false;
    private bool generatorBroken = true;
    private bool miniGameActive = false;
    private bool hasBeenTurnedOnOnce = false;
    [Header("Local References")]
    [SerializeField] private Announcer announcer;
    [SerializeField] public TextMeshProUGUI testText;
    private bool soundEffectPlayed = true;
    [SerializeField] private Image powerBar;


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
        if (colorUI != null) colorUI.SetActive(false); // ✅ OFF at start

        //testText = GetComponent<TextMeshProUGUI>();
        //generatorDurationLeft = 0f;
        //generatorMaxDuration = 45f;

}

    void Update() {
        generatorDurationLeft = Mathf.Max(generatorDurationLeft - Time.deltaTime, 0);
        //Debug.Log("Generator Time Left: " + generatorDurationLeft);
        //testText.text = generatorDurationLeft.ToString();
        powerBar.fillAmount = generatorDurationLeft/ generatorMaxDuration;

        if (isNearGenerator && Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("E pressed near generator");

            if (!miniGameActive) {
                Debug.Log("Mini-game should start now");
                StartMiniGame();
            }
            else
            {
                StartCoroutine(RepairGenerator());
            }
        }
        StartCoroutine("ShutDownPower");
    }
    /*
    public void BreakGenerator() {
        //Debug.Log($"Break Attempt: On={isGeneratorOn}, Broken={generatorBroken}, HasRunOnce={hasBeenTurnedOnOnce}");

        if (generatorBroken) {
            Debug.Log("Skipping break: already broken or generator is off.");
            return;
        }

        generatorBroken = true;
        hasBeenTurnedOnOnce = true;

        Debug.Log("Generator has been broken.");

        idleSound.Stop();
        ceilingAudio.Stop();
        stopSound.Play();
        powerDownSound.Play();

        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights) {
            spotlight.enabled = false;
        }

        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights) {
            pointLight.enabled = true;
        }

        if (colorUI != null) colorUI.SetActive(false); // ✅ Hide color UI

        interactPrompt.text = "[E] Fix Generator";
        interactPrompt.gameObject.SetActive(isNearGenerator);
    }*/

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

        if (!hasBeenTurnedOnOnce)
        {
            announcer.beginAnnouncer();
        }

        if (interactPrompt != null) {
            interactPrompt.text = "Generator Fixed!";
            interactPrompt.gameObject.SetActive(true);
            StartCoroutine(HidePromptAfterDelay(2f));
        }

        if (colorUI != null) colorUI.SetActive(true); // ✅ Show color UI again

        StartCoroutine(RepairGenerator());
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
            if (generatorBroken) {
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

    private IEnumerator RepairGenerator() {
        startSound.Play();
        yield return new WaitForSeconds(startSound.clip.length);

        soundEffectPlayed = false;
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

        if (colorUI != null) colorUI.SetActive(true); // ✅ Show when generator turns on

        float temp = Mathf.Min(generatorMaxDuration, generatorDurationLeft + generatorMaxDuration / 2);
        generatorDurationLeft = temp;
        //yield return new WaitForSeconds(temp - stopSound.clip.length);

        
        
        miniGameActive = false;

        //Debug.Log("Generator shut down automatically — now considered broken.");

        if (isNearGenerator && interactPrompt != null) {
            interactPrompt.text = "[E] Fix Generator";
            interactPrompt.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShutDownPower()
    {
        if (generatorDurationLeft <= stopSound.clip.length && !soundEffectPlayed)
        {
            soundEffectPlayed = true;
            idleSound.Stop();
            stopSound.Play();
            yield return new WaitForSeconds(stopSound.clip.length);
            ceilingAudio.Stop();
            powerDownSound.Play();

            generatorBroken = true;
            lightMaterial.DisableKeyword("_EMISSION");
            foreach (Light spotlight in ceilingSpotlights)
            {
                spotlight.enabled = false;
            }

            generatorSpotlight.enabled = true;
            foreach (Light pointLight in generatorPointLights)
            {
                pointLight.enabled = true;
            }

            if (colorUI != null) colorUI.SetActive(false); // ✅ Hide when generator shuts down
        }
    }
}
