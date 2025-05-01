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

    [Header("Generator Failure Link")]
    public GeneratorController generatorController;
    public float failureChance = 0.5f;

    [Header("Task Time Running Out Sound")]
    public AudioSource taskAudioSource;
    public bool isFirstTimeCalled = false;

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

        // Pick a new random tile number (1–9) and wall color
        currentTargetWallColor = wallColors[Random.Range(0, wallColors.Length)];
        currentTargetTile = Random.Range(1, 10);

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

            // Random chance to break the generator
            if (generatorController != null && Random.value < failureChance) {
                generatorController.BreakGenerator();
            }

            yield return new WaitForSeconds(5f);
            GiveNextTask();
        }
    }

    IEnumerator NextTaskAfterDelay() {
        yield return new WaitForSeconds(5f);
        GiveNextTask();
    }
}
