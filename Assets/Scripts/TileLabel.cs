using UnityEngine;
using TMPro;

public class TileLabel : MonoBehaviour {
    public TextMeshProUGUI labelText;  // Use UGUI for UI text

    public void SetNumber(int number) {
        if (labelText != null) {
            labelText.text = number.ToString();
        }
    }

    public int GetCurrentNumber() {
        if (labelText != null && int.TryParse(labelText.text, out int result))
            return result;
        return -1;
    }
}
