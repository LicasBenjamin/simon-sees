using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    [Header("Task Settings")]
    public int currentTargetTile = 0;
    public string currentTargetWallColor = "";
    public bool taskActive = false;

    private int totalTiles = 9;
    private float taskDuration = 25f;

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
    public TileController tileController;
    private Coroutine taskTimerCoroutine;

    [Header("Local References")]
    public YellowWall yellowWallReference;

    [Header("Wall Color Randomization")]
    public List<Renderer> wallRenderers;  // Drag the 4 wall Renderers here
    public Material redMat;
    public Material greenMat;
    public Material blueMat;
    public Material yellowMat;

    private Dictionary<string, Renderer> currentColorToWall = new Dictionary<string, Renderer>();

    void Start() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void beginAnnouncer() {
        announcerText.text = "Welcome, test subject.";
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        Invoke(nameof(GiveNextTask), displayTime);
    }

    void GiveNextTask() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        currentTargetTile = Random.Range(1, totalTiles + 1);
        taskAudioSource.Play();

        if (tileController != null) {
            tileController.ShuffleTileNumbers();
        }

        ShuffleWallColors();

        List<string> availableColors = new List<string>(currentColorToWall.Keys);

        if (failedTasks >= 1 && availableColors.Contains("Yellow")) {
            availableColors.Remove("Yellow");
            availableColors.Add("Glass");
        }

        currentTargetWallColor = availableColors[Random.Range(0, availableColors.Count)];

        announcerText.text = $"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor.ToLower()} wall.";
        taskActive = true;

        if (taskTimerCoroutine != null) StopCoroutine(taskTimerCoroutine);
        taskTimerCoroutine = StartCoroutine(TaskTimer());
    }

    public void CheckTaskCompletion(string wallColor, int tileNumber) {
        if (!taskActive) return;

        if (wallColor == currentTargetWallColor && tileNumber == currentTargetTile) {
            announcerText.text = "Subject has completed this task.";
            taskActive = false;
            taskAudioSource.Stop();
            if (taskTimerCoroutine != null)
                StopCoroutine(taskTimerCoroutine);

            StartCoroutine(NextTaskAfterDelay());
        } else {
            announcerText.text = $"Subject failed — wrong wall or tile.";
        }
    }

    IEnumerator TaskTimer() {
        yield return new WaitForSeconds(taskDuration);

        if (taskActive) {
            taskActive = false;
            announcerText.text = $"Subject has failed this task.";
            failedTasks++;
            AdjustState();
            taskAudioSource.Stop();

            if (failedTasks == 1) {
                yield return new WaitForSeconds(34f);
            } else {
                yield return new WaitForSeconds(5f);
            }
            GiveNextTask();
        }
    }

    IEnumerator NextTaskAfterDelay() {
        yield return new WaitForSeconds(5f);
        GiveNextTask();
    }

    void AdjustState() {
        if (failedTasks == 1) {
            StartCoroutine(HandleError1Dialogue());
        }
    }

    IEnumerator HandleError1Dialogue() {
        yield return Speak("", 0.5f);
        StartCoroutine(yellowWallReference.OpenDoor());
        yield return Speak("", 15f);
        yield return Speak("Looks like Simon is upset now", 2.5f);
        yield return Speak("to monitor him one of the walls is now going to be\n referred to as the “glass wall,”", 5f);
        yield return Speak(" it should be self-explanatory Subject 31D.", 5f);
        yield return Speak("As a reminder, performing the tasks incorrectly is\n not advised for your safety", 5f);
        yield return Speak("", 0.5f);
    }

    IEnumerator Speak(string speech, float seconds) {
        announcerText.text = speech;
        yield return new WaitForSeconds(seconds);
    }

    void ShuffleWallColors() {
        currentColorToWall.Clear();

        List<(string name, Material mat)> colorPool = new List<(string, Material)> {
            ("Red", redMat),
            ("Green", greenMat),
            ("Blue", blueMat),
            ("Yellow", yellowMat)
        };

        foreach (Renderer rend in wallRenderers) {
            if (!rend.gameObject.activeInHierarchy) continue; // ✅ skip hidden walls
            if (colorPool.Count == 0) break;

            int index = Random.Range(0, colorPool.Count);
            var (colorName, mat) = colorPool[index];

            rend.material = mat;
            currentColorToWall[colorName] = rend;

            colorPool.RemoveAt(index);

            Debug.Log($"[Wall Color] Assigned {colorName} to {rend.gameObject.name}");
        }
    }
}