using UnityEngine;

public class Tile : MonoBehaviour {
    public TileLabel tileLabel; // Drag the TileLabel script manually or auto-link it

    private void Start() {
        if (tileLabel == null) {
            tileLabel = GetComponent<TileLabel>();
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.CompareTag("Player") && tileLabel != null) {
            int visibleTileNumber = tileLabel.GetCurrentNumber();
            this.transform.parent.GetComponent<TileController>().UpdateTileStandingOn(visibleTileNumber);
        }
    }
}
