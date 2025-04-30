using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WallDetection : MonoBehaviour {
    public float maxRayDistance = 100f;
    public LayerMask wallLayer;
    public TextMeshProUGUI colorText;
    public Announcer announcer;

    private Camera mainCamera;
    private int score = 0;

    public Image UICompletionCursor;
    public float taskIncreaseRate = 0.15f;
    public AudioSource taskAudioSource;

    public float maxShakeIntensity = 0.1f;
    public float shakeGrowthRate = 0.05f;
    private Vector3 originalCameraPosition;

    void Start() {
        mainCamera = Camera.main;
        originalCameraPosition = mainCamera.transform.localPosition;
    }

    void Update() {
        UpdateReticleUI();
        HandleWallHover();
    }

    void UpdateReticleUI() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer) && hit.collider.CompareTag("Wall")) {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null) {
                Color wallColor = wallRenderer.material.color;
                string colorName = GetColorName(wallColor);

                colorText.text = colorName;
                colorText.color = wallColor;
            }
        } else {
            colorText.text = "";
        }
    }

    void HandleWallHover() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        bool doingCorrectTask = false;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer) && hit.collider.CompareTag("Wall")) {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null) {
                string colorName = GetColorName(wallRenderer.material.color);
                int tileStandingOn = GetCurrentTile();

                // ✅ Updated to use public getters
                if (colorName == announcer.CurrentTargetWallColor &&
                    tileStandingOn == announcer.CurrentTargetTile &&
                    announcer.TaskActive) {

                    doingCorrectTask = true;

                    if (UICompletionCursor.fillAmount >= 1f) {
                        UICompletionCursor.fillAmount = 0;
                        announcer.CheckTaskCompletion(colorName, tileStandingOn);

                        if (taskAudioSource != null && taskAudioSource.isPlaying) {
                            taskAudioSource.Stop();
                        }

                        mainCamera.transform.localPosition = originalCameraPosition;
                    } else {
                        UICompletionCursor.fillAmount += taskIncreaseRate * Time.deltaTime;
                    }
                }
            }
        }

        // Handle audio
        if (taskAudioSource != null) {
            if (doingCorrectTask) {
                if (!taskAudioSource.isPlaying) {
                    taskAudioSource.Play();
                }
            } else {
                if (taskAudioSource.isPlaying) {
                    taskAudioSource.Pause();
                }
            }
        }

        // Reset bar when not active
        if (!announcer.TaskActive) {
            taskAudioSource.Stop();
            UICompletionCursor.fillAmount = 0;
        }

        // Handle camera shake
        if (doingCorrectTask && taskAudioSource != null && taskAudioSource.isPlaying) {
            float shakeAmount = Mathf.Min(maxShakeIntensity, taskAudioSource.time * shakeGrowthRate);
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * shakeAmount;

            mainCamera.transform.localPosition = originalCameraPosition + shakeOffset;
        } else {
            if (mainCamera != null) {
                mainCamera.transform.localPosition = originalCameraPosition;
            }
        }
    }

    void HandleWallClick() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer) && hit.collider.CompareTag("Wall")) {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null) {
                string wallName = hit.collider.name;
                Color wallColor = wallRenderer.material.color;
                string colorName = GetColorName(wallColor);

                score += 10;
                int tileStandingOn = GetCurrentTile();

                Debug.Log($"Clicked on: {wallName} | Color: {colorName} | Score: {score} | Tile: {tileStandingOn}");

                announcer.CheckTaskCompletion(colorName, tileStandingOn);
            }
        } else {
            Debug.Log("Click missed -- No wall detected.");
        }
    }

    int GetCurrentTile() {
        for (int i = 0; i < TileController.tilePlayerIsOn.Length; i++) {
            if (TileController.tilePlayerIsOn[i])
                return i + 1;
        }
        return 0;
    }

    string GetColorName(Color color) {
        if (color == Color.red) return "Red";
        if (color == Color.blue) return "Blue";
        if (color == Color.green) return "Green";
        if (color == Color.yellow) return "Yellow";

        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        switch (hexColor) {
            case "001992": return "Blue";
            case "840000": return "Red";
            case "005005": return "Green";
            case "847E00": return "Yellow";
            default: return "Unknown";
        }
    }
}
