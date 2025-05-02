using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WallDetection : MonoBehaviour {
    public float maxRayDistance = 100f;
    public LayerMask wallLayer;
    public TextMeshProUGUI colorText;
    public Announcer announcer;

    public Image UICompletionCursor;
    public float taskIncreaseRate = 0.15f;
    public AudioSource taskAudioSource;

    //public float maxShakeIntensity = 0.1f;
    //public float shakeGrowthRate = 0.05f;
    //private Vector3 originalCameraPosition;

    private Camera mainCamera;

    void Start() {
        mainCamera = Camera.main;
        //originalCameraPosition = mainCamera.transform.localPosition;
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
        //bool doingCorrectTask = false;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer) && hit.collider.CompareTag("Wall")) {
            Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null) {
                string colorName = GetColorName(wallRenderer.material.color);
                //checking if wall is "Glass"
                if(colorName == "White")
                {
                    colorName = "Glass";
                }
                //Debug.Log("Player is looking at " + colorName + " Wall");
                int tileStandingOn = TileController.playerIsOnTile;
                //Debug.Log("Player is standing on tile #" + tileStandingOn);
                //If hovering and standing on correct task, increment bar
                if (colorName == announcer.currentTargetWallColor && tileStandingOn == announcer.currentTargetTile && announcer.taskActive)
                {
                    //doingCorrectTask = true;
                    //If bar is fully completed, trigger completion in announcer
                    if (UICompletionCursor.fillAmount >= 1f)
                    {
                        UICompletionCursor.fillAmount = 0;
                        announcer.CheckTaskCompletion(colorName, tileStandingOn);

                        if (taskAudioSource != null && taskAudioSource.isPlaying)
                            taskAudioSource.Stop();

                        //mainCamera.transform.localPosition = originalCameraPosition;
                    } else {
                        UICompletionCursor.fillAmount += taskIncreaseRate * Time.deltaTime;
                    }
                }
            }
        }


        if (!announcer.taskActive)
        {
            UICompletionCursor.fillAmount = 0;
        }

        // Audio handling
        /**
        if (taskAudioSource != null) {
            if (doingCorrectTask) {
                if (!taskAudioSource.isPlaying)
                    taskAudioSource.Play();
            } else {
                if (taskAudioSource.isPlaying)
                    taskAudioSource.Pause();
            }
        }

        if (!announcer.taskActive) {
            if (taskAudioSource != null)
                taskAudioSource.Stop();
            UICompletionCursor.fillAmount = 0;
        }*/

        /**
        if (doingCorrectTask && taskAudioSource != null && taskAudioSource.isPlaying) {
            float shakeAmount = Mathf.Min(maxShakeIntensity, taskAudioSource.time * shakeGrowthRate);
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * shakeAmount;

            mainCamera.transform.localPosition = originalCameraPosition + shakeOffset;
        } else {
            mainCamera.transform.localPosition = originalCameraPosition;
        }*/
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

                int tileStandingOn = TileController.playerIsOnTile;

                Debug.Log($"Clicked on: {wallName} | Color: {colorName} | Tile: {tileStandingOn}");
                announcer.CheckTaskCompletion(colorName, tileStandingOn);
            }
        } else {
            Debug.Log("Click missed -- No wall detected.");
        }
    }

    string GetColorName(Color color) {
        if (color == Color.red) return "Red";
        if (color == Color.blue) return "Blue";
        if (color == Color.green) return "Green";
        if (color == Color.yellow) return "Yellow";
        if (color == Color.white) return "Glass";

        string hexColor = ColorUtility.ToHtmlStringRGB(color);
        switch (hexColor) {
            case "001992": return "Blue";
            case "840000": return "Red";
            case "005005": return "Green";
            case "847E00": return "Yellow";
            case "D2FFFF": return "Glass";
            default: return "Unknown";
        }
    }
}
