using UnityEngine;
using TMPro; // Required for TextMeshPro

public class WallDetection : MonoBehaviour {
    public float maxRayDistance = 100f; // Raycast distance
    public LayerMask wallLayer; // Assign "Wall" layer in the Inspector
    public TextMeshProUGUI colorText; // Assign this in the Inspector (UI next to the reticle)

    private Camera mainCamera;
    private string lastClickedColor = "None"; // Stores last clicked color

    void Start() {
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update() {
        UpdateColorText(); // Always update reticle UI
        if (Input.GetMouseButtonDown(0)) { // Detect wall only on mouse click
            DetectWallOnClick();
        }
    }

    // Always updates ColorText UI based on what the reticle is looking at
    void UpdateColorText() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer)) {
            if (hit.collider.CompareTag("Wall")) {
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
                if (wallRenderer != null) {
                    Color wallColor = wallRenderer.material.color;
                    string hexColor = GetHexColor(wallColor);
                    string colorName = GetColorName(hexColor);

                    // Update UI with detected color
                    colorText.text = colorName;
                    colorText.color = wallColor; // Change text color to match wall
                }
            }
        } else {
            colorText.text = ""; // Hide text if no wall detected
        }
    }

    // Detects wall color only when user clicks
    void DetectWallOnClick() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer)) {
            if (hit.collider.CompareTag("Wall")) {
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
                if (wallRenderer != null) {
                    Color wallColor = wallRenderer.material.color;
                    string hexColor = GetHexColor(wallColor);
                    string colorName = GetColorName(hexColor);

                    // Store clicked color
                    lastClickedColor = colorName;
                    Debug.Log($"Wall Clicked -- Color: {colorName}");
                }
            }
        } else {
            Debug.Log("Click missed -- No wall detected.");
        }
    }

    // Converts Color to HEX
    string GetHexColor(Color color) {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        return $"{r:X2}{g:X2}{b:X2}"; // Converts to uppercase hex
    }

    // Matches HEX to Color Name
    string GetColorName(string hexColor) {
        switch (hexColor) {
            case "001992": return "Blue";
            case "840000": return "Red";
            case "005005": return "Green";
            case "847E00": return "Yellow";
            default: return "Unknown";
        }
    }
}
