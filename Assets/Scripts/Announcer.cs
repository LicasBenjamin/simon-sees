using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    public string currentTargetWallColor;
    public int currentTargetTile;
    public bool taskActive = false;

    private string[] colors = { "Red", "Blue", "Green", "Yellow" };
    private int totalTiles = 9;  // Adjust based on actual tile count

    private float taskDuration = 20f;

    void Start() {
        // Force activation at game start to ensure visibility
        announcerText.transform.parent.gameObject.SetActive(true);
        WelcomePlayer();
    }

    void WelcomePlayer() {
        SetAnnouncerText("Welcome, Test Subject.");
        Invoke(nameof(GiveNewTask), displayTime);
    }

    void GiveNewTask() {
        currentTargetWallColor = colors[Random.Range(0, colors.Length)];
        currentTargetTile = Random.Range(1, totalTiles + 1);

        SetAnnouncerText($"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor.ToLower()} wall.");

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

        SetAnnouncerText("Subject has completed this task.");
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

        SetAnnouncerText("Player has failed this task.");
        Invoke(nameof(GiveNewTask), displayTime);
    }

    void SetAnnouncerText(string message) {
        announcerText.text = message;

        // Always ensure parent UI is active before displaying text
        announcerText.transform.parent.gameObject.SetActive(true);

        CancelInvoke(nameof(HideAnnouncer));
        Invoke(nameof(HideAnnouncer), displayTime);
    }

    void HideAnnouncer() {
        announcerText.transform.parent.gameObject.SetActive(false);
    }
}
