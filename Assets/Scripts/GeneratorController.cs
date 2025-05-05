using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GeneratorController : MonoBehaviour
{
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
    public AudioSource powerDownSound;

    [Header("UI")]
    public TextMeshProUGUI interactPrompt;
    public GameObject miniGameUI;
    public GameObject colorUI;

    [Header("Settings")]
    public float generatorMaxDuration = 100f;
    public float generatorDurationLeft = 0;
    public float generatorDrainRate = 1;

    private bool isNearGenerator = false;
    private bool generatorBroken = true;
    private bool miniGameActive = false;
    private bool hasBeenTurnedOnOnce = false;

    [Header("Local References")]
    [SerializeField] private Announcer announcer;
    [SerializeField] public TextMeshProUGUI testText;
    private bool soundEffectPlayed = true;
    [SerializeField] private Image powerBar;
    public bool generatorIsPaused = false;

    void Start()
    {
        generatorSpotlight.enabled = true;
        foreach (Light pointLight in generatorPointLights)
        {
            pointLight.enabled = true;
        }

        lightMaterial.DisableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights)
        {
            spotlight.enabled = false;
        }

        interactPrompt.gameObject.SetActive(false);
        miniGameUI.SetActive(false);
        if (colorUI != null) colorUI.SetActive(false);
    }

    void Update()
    {
        if (!generatorIsPaused)
        {
            generatorDurationLeft = Mathf.Max(generatorDurationLeft - Time.deltaTime * generatorDrainRate, 0);
            powerBar.fillAmount = generatorDurationLeft / generatorMaxDuration;
        }

        if (isNearGenerator && Input.GetKeyDown(KeyCode.E))
        {
            if (!miniGameActive)
            {
                StartMiniGame();
            }
        }

        if (generatorDurationLeft <= stopSound.clip.length && !soundEffectPlayed)
        {
            StartCoroutine("ShutDownPower");
        }
    }

    public void StartMiniGame()
    {
        if (miniGameUI != null)
        {
            miniGameUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("MiniGame UI is NOT assigned!");
        }

        miniGameActive = true;
        interactPrompt.gameObject.SetActive(false);
    }

    public void CompleteMiniGame()
    {
        miniGameUI.SetActive(false);
        miniGameActive = false;
        generatorBroken = false;

        if (!hasBeenTurnedOnOnce)
        {
            announcer.beginAnnouncer();
        }

        if (interactPrompt != null)
        {
            interactPrompt.text = "Generator Fixed!";
            interactPrompt.gameObject.SetActive(true);
            StartCoroutine(HidePromptAfterDelay(2f));
        }

        if (colorUI != null) colorUI.SetActive(true);

        StartCoroutine(RepairGenerator());
    }

    /// ✅ New method for clean early exit without completing the mini-game
    public void ExitMiniGameEarly()
    {
        miniGameUI.SetActive(false);
        miniGameActive = false;

        if (interactPrompt != null && isNearGenerator)
        {
            interactPrompt.text = "[E] Fix Generator";
            interactPrompt.gameObject.SetActive(true);
        }

        if (colorUI != null) colorUI.SetActive(false); // Optional: hide color reticle
    }

    private IEnumerator HidePromptAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        interactPrompt.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearGenerator = true;
            if (generatorBroken)
            {
                interactPrompt.gameObject.SetActive(true);
                interactPrompt.text = generatorBroken ? "[E] Fix Generator" : "[E] Start Generator";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isNearGenerator = false;
            interactPrompt.gameObject.SetActive(false);
        }
    }

    private IEnumerator RepairGenerator()
    {
        startSound.Play();
        yield return new WaitForSeconds(startSound.clip.length);

        soundEffectPlayed = false;
        hasBeenTurnedOnOnce = true;

        generatorSpotlight.enabled = false;
        foreach (Light pointLight in generatorPointLights)
        {
            pointLight.enabled = false;
        }

        lightMaterial.EnableKeyword("_EMISSION");
        foreach (Light spotlight in ceilingSpotlights)
        {
            spotlight.enabled = true;
        }

        ceilingAudio.Play();
        idleSound.Play();

        if (colorUI != null) colorUI.SetActive(true);

        float temp = Mathf.Min(generatorMaxDuration, generatorDurationLeft + generatorMaxDuration / 2);
        generatorDurationLeft = temp;

        miniGameActive = false;

        if (isNearGenerator && interactPrompt != null)
        {
            interactPrompt.text = "[E] Fix Generator";
            interactPrompt.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShutDownPower()
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

        if (colorUI != null) colorUI.SetActive(false);
    }
}
