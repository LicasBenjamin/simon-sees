using UnityEngine;
using UnityEngine.UI;

public class WallDetection : MonoBehaviour
{
    public float maxRayDistance = 50f; // Maximum raycast distance
    public LayerMask wallLayer; // Assign the "Wall" layer in the Inspector
    public int score = 0; // Player's score
    public Text colorText; // Assign in the UI (Optional)

    private Camera mainCamera; // Reference to the main camera

    void Start() {
        mainCamera = Camera.main; // Get the main camera reference
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) // Left-click to detect the wall
        {
            DetectWall();
        }
    }

    void DetectWall() {
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2)); // Center of the screen (reticle)
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer)) {
            if (hit.collider.CompareTag("Wall")) {
                string wallName = hit.collider.name;

                // Get the Renderer to access the wall's color
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();
                if (wallRenderer != null) {
                    Color wallColor = wallRenderer.material.color;
                    string detectedColor = GetHexColor(wallColor); // Convert to hex

                    string colorName = GetColorName(detectedColor); // Get the corresponding color name

                    if (colorName != "Unknown") {
                        score += 10; // Increase score -- for testing
                        Debug.Log("Clicked on: " + wallName + " | Color: " + colorName + " | Score: " + score);

                        if (colorText != null) {
                            colorText.text = "Detected Color: " + colorName;
                        }
                    }
                    else {
                        Debug.Log("Unknown color detected: " + detectedColor);
                    }
                }
            }
        }
    }

    // Convert a Color to a hex string
    string GetHexColor(Color color) {
        int r = Mathf.RoundToInt(color.r * 255);
        int g = Mathf.RoundToInt(color.g * 255);
        int b = Mathf.RoundToInt(color.b * 255);
        return $"{r:X2}{g:X2}{b:X2}"; // Returns hex in uppercase
    }

    // Match the detected hex color to a known wall color
    string GetColorName(string hexColor) {
        switch (hexColor)
        {
            case "001992": return "Blue";
            case "840000": return "Red";
            case "005005": return "Green";
            case "847E00": return "Yellow";
            default: return "Unknown";
        }
    }
}