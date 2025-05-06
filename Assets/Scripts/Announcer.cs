using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Announcer : MonoBehaviour {
    [Header("UI")]
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    [Header("Task Settings")]
    public int currentTargetTile = 0;
    public string currentTargetWallColor = "";
    public bool taskActive = false;

    private int totalTiles = 9;
    private float taskDuration = 20f;
    public int successfulTasks = 0;

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
    public TextMeshProUGUI testText;
    public Simon simonReference;
    public TextMeshProUGUI testTextTimer;
    private float timer;

    [Header("Wall Color Randomization")]
    public List<Renderer> wallRenderers;  // Drag the 4 wall Renderers here
    public Material redMat;
    public Material greenMat;
    public Material blueMat;
    public Material yellowMat;

    private Dictionary<string, Renderer> currentColorToWall = new Dictionary<string, Renderer>();
    private string invalidColor = "";

    void Start() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(false);
        // Manually assign wall colors based on their initial materials
        ShuffleWallColors();
    }
    private void Update()
    {
        testTextTimer.text = "Timer For Current Task: " + timer.ToString("F1");
        timer = Mathf.Max(timer -= Time.deltaTime, 0);
    }

    public void beginAnnouncer() {
        announcerText.text = "Welcome, test subject.";
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        //add tutorial information here
        Invoke(nameof(GiveNextTask), displayTime);
    }

    void GiveNextTask() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);
        currentTargetTile = Random.Range(1, totalTiles + 1);
        taskAudioSource.Play();
        timer = taskDuration;

        /*Adjustments to difficulty*/

        //Increase the generator's drain rate
        if (successfulTasks > 4)
        {
            generatorController.generatorDrainRate += 0.1f;
            Debug.Log("Generator drain rate: " + generatorController.generatorDrainRate);
        }
        // Shuffle tile number labels on the ground after round 5
        if (successfulTasks > 8) 
        {  
            tileController.ShuffleTileNumbers();
        }
        // Shuffle wall colors after round 10
        if (successfulTasks > 12)
        {
            ShuffleWallColors();
        }
        if (successfulTasks == 15)
        {
            Debug.Log("Switching to Success Cutscene...");
            SceneManager.LoadScene("SuccessCutscene");
            return;  // Prevents further logic from running after the scene change
        }


        //ShuffleWallColors();

        List<string> availableColors = new List<string>(currentColorToWall.Keys);

        if (failedTasks >= 1)
        {
            //swap raised wall with glass wall
            availableColors.Remove(invalidColor);
            availableColors.Add("Glass");
        }

        int tempRange = Random.Range(0, availableColors.Count);
        Debug.Log("temp range generated: " + tempRange);
        Debug.Log("My List: " + string.Join(", ", availableColors));
        currentTargetWallColor = availableColors[tempRange];

        testText.text = "Successful Tasks: " + successfulTasks;
        // Display new task
        announcerText.text = $"Stand on tile {currentTargetTile} and look at the {currentTargetWallColor.ToLower()} wall.";
        taskActive = true;

        if (taskTimerCoroutine != null) StopCoroutine(taskTimerCoroutine);
        taskTimerCoroutine = StartCoroutine(TaskTimer());
    }

    public void CheckTaskCompletion(string wallColor, int tileNumber) {
        if (!taskActive) return;

        if (wallColor == currentTargetWallColor && tileNumber == currentTargetTile) {
            announcerText.text = "Subject has completed this task.";
            successfulTasks++;
            generatorController.miniGameUI.GetComponent<MiniGameController>().lineSpeed += 20;
            timer = 0;
            //Debug.Log("Successful Task Count: " + successfulTasks);
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
            if(failedTasks == 1)
            {
                generatorController.generatorIsPaused = true;   //Pause the generator from going down
                yield return new WaitForSeconds(34f);           //Give enough time for wall opening sequence to play
                generatorController.generatorIsPaused = false;  //Allow the generator to be turned back on
            }
            else
            {
                //Nothing really added here unless timing needs to be added for a "cutscene" with dialogue for the other failedTask amounts
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
        else if (failedTasks == 2)
        {
            simonReference.triggerSecondErrorMovement();
        }
        else
        {
            SceneManager.LoadScene("FailScene");
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
            //if (!rend.gameObject.activeInHierarchy) continue; // ✅ skip hidden walls
            //if (colorPool.Count == 0) break;

            int index = Random.Range(0, colorPool.Count);
            //Debug.Log("ColorPool [" + index + "]: " + colorPool[index].name);

            var (colorName, mat) = colorPool[index];

            if(rend.gameObject.GetComponent<YellowWall>() != null)
            {
                invalidColor = colorPool[index].name;
            }

            rend.material = mat;
            currentColorToWall[colorName] = rend;

            colorPool.RemoveAt(index);

            Debug.Log($"[Wall Color] Assigned {colorName} to {rend.gameObject.name}");
        }
    }
}