using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour {
    public TextMeshProUGUI tileLabel; // Drag the TileLabel script manually or auto-link it

    private void Start() {
        if (tileLabel == null) {
            tileLabel = GetComponent<TextMeshProUGUI>();
        }
    }

    private int RomanToInt(string roman)
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
        for (int i = 0; i < romanNumerals.Length; i++)
        {
            if (roman == romanNumerals[i])
            {
                return i + 1;
            }
        }
        return -1;
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("Tile Trigger is Activated");
        if (other.CompareTag("Player") && tileLabel != null) {
            //int visibleTileNumber = tileLabel.GetCurrentNumber(); int.TryParse(tileTMP.text
            int visibleTileNumber;
            if (tileLabel.text.StartsWith("I") || tileLabel.text.StartsWith("V"))
            {
                visibleTileNumber = RomanToInt(tileLabel.text);
            }
            else
            {
                visibleTileNumber = int.Parse(tileLabel.text);
            }
            this.transform.parent.GetComponent<TileController>().UpdateTileStandingOn(visibleTileNumber);
            //Debug.Log("Tile updated to: " + TileController.playerIsOnTile);
        }
    }
}
