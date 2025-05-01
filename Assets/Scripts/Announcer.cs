using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    [Header("Task Settings")]
    public string currentTargetWallColor;
    public int currentTargetTile;
    public bool taskActive = false;

    private string[] colors = { "Red", "Blue", "Green", "Yellow" };
    private int totalTiles = 9;
    private float taskDuration = 12.5f;

    [Header("Generator Failure Link")]
    public GeneratorController generatorController;
    public float failureChance = 0.5f;

    [Header("Task Time Running Out Sound")]
    public AudioSource taskAudioSource;

    public string CurrentTargetWallColor => currentTargetWallColor;
    public int CurrentTargetTile => currentTargetTile;
    public bool TaskActive => taskActive;

    public bool isFirstTimeCalled = false;
    [Header("Central Game State Variables")]
    public int taskNum = -1;
    public int failedTasks = 0;

    //Temporarily disabling this being called on start, replaced with a callable function to begin the announcer
    /**
    void Start() {
        announcerText.transform.parent.gameObject.SetActive(true);
        WelcomePlayer();
    }*/
    private void Start()
    {
        announcerText.transform.parent.gameObject.SetActive(false);
    }
    public void beginAnnouncer()
    {
        announcerText.transform.parent.gameObject.SetActive(true);
        isFirstTimeCalled = true;
        WelcomePlayer();
    }

    void WelcomePlayer() {
        announcerText.text = "Welcome, Test Subject.";
        announcerText.transform.parent.gameObject.SetActive(true);
        Invoke(nameof(GiveNewTask), displayTime);
    }

    void GiveNewTask() {
        currentTargetWallColor = colors[Random.Range(0, colors.Length)];
        currentTargetTile = Random.Range(1, totalTiles + 1);
        //taskNum++;
        //taskDuration = -0.5f*(taskNum) + 30f;
        //Debug.Log("Current time for Task "+taskNum+": "+taskDuration);
        taskAudioSource.Play();

        announcerText.text = $"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor.ToLower()} wall.";
        announcerText.transform.parent.gameObject.SetActive(true);

        taskActive = true;
        Invoke(nameof(CheckIfTaskFailed), taskDuration);
    }

    public void CheckTaskCompletion(string wallColor, int tileNumber) {
        if (!taskActive) return;

        if (wallColor == currentTargetWallColor && tileNumber == currentTargetTile) {
            TaskSucceeded();
        } else {
            TaskFailed();
        }
    }

    void TaskSucceeded() {
        taskActive = false;
        CancelInvoke(nameof(CheckIfTaskFailed));

        taskAudioSource.Stop();

        announcerText.text = "Subject has completed this task.";
        announcerText.transform.parent.gameObject.SetActive(true);

        Invoke(nameof(GiveNewTask), displayTime);
    }

    void CheckIfTaskFailed() {
        if (!taskActive) return;
        TaskFailed();
    }

    void TaskFailed() {
        taskActive = false;
        CancelInvoke(nameof(CheckIfTaskFailed));

        taskAudioSource.Stop();

        announcerText.text = "Player has failed this task.";
        announcerText.transform.parent.gameObject.SetActive(true);

        //Add to the failed tasks counter and adjust state accordingly
        failedTasks++;
        AdjustState();

        Debug.Log("Task failed.");

        //Below is randomly generating odds to break the generator
        if (generatorController != null) {
            if (generatorController.IsGeneratorOn()) {
                float roll = Random.value;
                Debug.Log($"Failure chance roll: {roll} (threshold: {failureChance})");

                if (roll < failureChance) {
                    Debug.Log("Announcer decided to break the generator.");
                    generatorController.BreakGenerator();
                }
            } else {
                Debug.Log("Generator is already off. Skipping break attempt.");
            }
        } else {
            Debug.LogWarning("generatorController is NULL in Announcer!");
        }

        Invoke(nameof(GiveNewTask), displayTime);
    }

    void AdjustState()
    {
        if(failedTasks == 1)
        {
            //open door, activate glass as the replacement for "yellow" wall
        }
        else
        {
            //get monster closer, if failedTasks
        }
    }
}
