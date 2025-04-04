using UnityEngine;
using TMPro;

public class WallDetection : MonoBehaviour {
    public float maxRayDistance = 100f;
    public LayerMask wallLayer;
    public TextMeshProUGUI colorText;

    private Camera mainCamera;
    private int score = 0;

    void Start() {
        mainCamera = Camera.main;
    }

    void Update() {
        UpdateReticleUI();

        if (Input.GetMouseButtonDown(0)) {
            HandleWallClick();
        }
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
        }
        else {
            colorText.text = "";
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
            }
        }
        else {
            Debug.Log("Click missed -- No wall detected.");
        }
    }

    int GetCurrentTile() {
        for (int i = 0; i < TileController.tilePlayerIsOn.Length; i++) {
            if (TileController.tilePlayerIsOn[i])
                return i + 1;
        }

        return 0; // Default if no tile detected
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