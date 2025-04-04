using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    private string currentTargetWallColor;
    private int currentTargetTile;
    private bool taskActive = false;

    private string[] colors = { "Red", "Blue", "Green", "Yellow" };
    private int totalTiles = 9;  // Adjust based on actual tile count

    private float taskDuration = 10f;

    void Start() {
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
        if (!taskActive)
            return;

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
        if (!taskActive)
            return;

        TaskFailed();
    }

    void TaskFailed() {
        taskActive = false;
        CancelInvoke(nameof(CheckIfTaskFailed));

        announcerText.text = "Subject has failed this task.";
        announcerText.transform.parent.gameObject.SetActive(true);

        Invoke(nameof(GiveNewTask), displayTime);
    }
}