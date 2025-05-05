using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileController : MonoBehaviour {
    public static int playerIsOnTile = 0;
    public TextMeshProUGUI[] tileLabels; // Assign all 9 TileLabel components in Inspector

    public void UpdateTileStandingOn(int visibleTileNumber) {
        playerIsOnTile = visibleTileNumber;
    }

    public void ShuffleTileNumbers() {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= tileLabels.Length; i++) numbers.Add(i);

        for (int i = 0; i < tileLabels.Length; i++) {
            int randIndex = Random.Range(0, numbers.Count);
            int number = numbers[randIndex];
            tileLabels[i].text = (numbers[randIndex]).ToString();
            if (number == 6 || number == 9)
            {
                tileLabels[i].fontStyle = FontStyles.Underline;
            }
            else
            {
                tileLabels[i].fontStyle = FontStyles.Normal;
            }
            numbers.RemoveAt(randIndex);
        }
    }

    public void RomanizeNumbers()
    {
        string[] romanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
        for (int i = 0; i<tileLabels.Length; i++)
        {
            tileLabels[i].fontSize = 45;
            tileLabels[i].fontStyle = FontStyles.Normal;
            //assuming that text is an integer
            int indexTileShouldBe = int.Parse(tileLabels[i].text);

            tileLabels[i].text = romanNumerals[indexTileShouldBe - 1];
        }
    }
}
