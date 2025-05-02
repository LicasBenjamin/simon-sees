using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour {
    public TextMeshProUGUI tileLabel; // Drag the TileLabel script manually or auto-link it

    private void Start() {
        if (tileLabel == null) {
            tileLabel = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnTriggerStay(Collider other) {
        //Debug.Log("Tile Trigger is Activated");
        if (other.CompareTag("Player") && tileLabel != null) {
            //int visibleTileNumber = tileLabel.GetCurrentNumber(); int.TryParse(tileTMP.text
            int visibleTileNumber = int.Parse(tileLabel.text);
            this.transform.parent.GetComponent<TileController>().UpdateTileStandingOn(visibleTileNumber);
            //Debug.Log("Tile updated to: " + TileController.playerIsOnTile);
        }
    }
}
