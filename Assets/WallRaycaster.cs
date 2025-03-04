using UnityEngine;

public class WallClickRaycaster : MonoBehaviour {
    public float maxRayDistance = 50f; // Raycast distance
    public LayerMask wallLayer; 
    public int score = 0; 

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            CastRayToWalls();
        }
    }

    void CastRayToWalls() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Create a ray from the camera
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance, wallLayer)) {
            if (hit.collider.CompareTag("Wall")) { // Ensure it's a wall 
                string wallName = hit.collider.name;

                // Get the material color
                Renderer wallRenderer = hit.collider.GetComponent<Renderer>();

                if (wallRenderer != null) {
                    Color wallColor = wallRenderer.material.color;
                    string colorName = GetColorName(wallColor); 

                    score += 10;
                    Debug.Log("Clicked on: " + wallName + " | Color: " + colorName + " | Score: " + score);
                }
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green, 1f);
    }

    string GetColorName(Color color)
    {
        if (color == Color.red) return "Red";
        if (color == Color.blue) return "Blue";
        if (color == Color.green) return "Green";
        if (color == Color.yellow) return "Yellow";

        return "Unknown";
    }
}
