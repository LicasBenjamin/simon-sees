using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    [Header("Task Settings")]
    private string currentTargetWallColor;
    private int currentTargetTile;
    private bool taskActive = false;

    private string[] colors = { "Red", "Blue", "Green", "Yellow" };
    private int totalTiles = 9;
    private float taskDuration = 10f;

    [Header("Generator Failure Link")]
    public GeneratorController generatorController;
    public float failureChance = 0.5f;

    public string CurrentTargetWallColor => currentTargetWallColor;
    public int CurrentTargetTile => currentTargetTile;
    public bool TaskActive => taskActive;

    void Start() {
        announcerText.transform.parent.gameObject.SetActive(true);
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

        announcerText.text = "Player has failed this task.";
        announcerText.transform.parent.gameObject.SetActive(true);

        Debug.Log("Task failed.");

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
}
