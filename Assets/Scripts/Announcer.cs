using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    [Header("Task Settings")]
    public string[] wallColors = { "Red", "Blue", "Green", "Yellow" };
    public int currentTargetTile = 0;
    public string currentTargetWallColor = "";
    public bool taskActive = false;

    private string[] colors = { "Red", "Blue", "Green", "Yellow" };
    private int totalTiles = 9;
    private float taskDuration = 20f;

    [Header("Generator Failure Link")]
    public GeneratorController generatorController;
    public float failureChance = 0.5f;

    [Header("Task Time Running Out Sound")]
    public AudioSource taskAudioSource;
    public bool isFirstTimeCalled = false;
    [Header("Central Game State Variables")]
    public int taskNum = -1;
    public int failedTasks = 0;

    [Header("Tile Logic")]
    public TileController tileController; // ✅ Drag your TileController here in Inspector

    private Coroutine taskTimerCoroutine;

    void Start() {
        announcerText.text = "Welcome, test subject.";
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        Invoke(nameof(GiveNextTask), displayTime);
    }

    void GiveNextTask() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        currentTargetWallColor = colors[Random.Range(0, colors.Length)];
        currentTargetTile = Random.Range(1, totalTiles + 1);
        //taskNum++;
        //taskDuration = -0.5f*(taskNum) + 30f;
        //Debug.Log("Current time for Task "+taskNum+": "+taskDuration);
        taskAudioSource.Play();

        //if failedTasks >= 1, change "Yellow" to "Glass"
        if (failedTasks >= 1)
        {
            if(currentTargetWallColor == "Yellow")
            {
                currentTargetWallColor = "Glass";
            }
        }
        announcerText.text = $"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor.ToLower()} wall.";
        announcerText.transform.parent.gameObject.SetActive(true);
        // Pick a new random tile number (1–9) and wall color

        // Shuffle tile number labels on the ground
        if (tileController != null) {
            tileController.ShuffleTileNumbers();
        }

        // Display new task
        announcerText.text = $"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor} wall.";
        taskActive = true;

        // Start timer for failure
        if (taskTimerCoroutine != null) StopCoroutine(taskTimerCoroutine);
        taskTimerCoroutine = StartCoroutine(TaskTimer());
    }

    public void CheckTaskCompletion(string wallColor, int tileNumber) {
        if (!taskActive) return;

        if (wallColor == currentTargetWallColor && tileNumber == currentTargetTile) {
            announcerText.text = "Subject has completed this task.";
            taskActive = false;

            if (taskTimerCoroutine != null)
                StopCoroutine(taskTimerCoroutine);

            StartCoroutine(NextTaskAfterDelay());
        } else {
            announcerText.text = $"Subject failed — wrong wall or tile.";
        }
    }

    IEnumerator TaskTimer() {
        yield return new WaitForSeconds(10f);

        if (taskActive) {
            taskActive = false;
            announcerText.text = $"Subject has failed this task.";

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
                    //generatorController.BreakGenerator();
                }
            } else {
                Debug.Log("Generator is already off. Skipping break attempt.");
                // Random chance to break the generator
                //if (generatorController != null && Random.value < failureChance) {
                //    generatorController.BreakGenerator();
            }

            yield return new WaitForSeconds(5f);
            GiveNextTask();
        }
    }

    IEnumerator NextTaskAfterDelay() {
        yield return new WaitForSeconds(5f);
        GiveNextTask();
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
