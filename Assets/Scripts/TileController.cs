using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour {
    public static int playerIsOnTile = 0;
    public TileLabel[] tileLabels; // Assign all 9 TileLabel components in Inspector

    public void UpdateTileStandingOn(int visibleTileNumber) {
        playerIsOnTile = visibleTileNumber;
    }

    public void ShuffleTileNumbers() {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= tileLabels.Length; i++) numbers.Add(i);

        for (int i = 0; i < tileLabels.Length; i++) {
            int randIndex = Random.Range(0, numbers.Count);
            tileLabels[i].SetNumber(numbers[randIndex]);
            numbers.RemoveAt(randIndex);
        }
    }
}
