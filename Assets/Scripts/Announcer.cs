using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f;

    void Start() {
        announcerText.text = "Welcome, test subject.";
        Invoke("HideAnnouncer", displayTime);
    }

    public void AnnounceWallInteraction(string wallColor, int tileNumber) {
        announcerText.text = $"Test subject has viewed the {wallColor.ToLower()} wall and stood on tile {tileNumber}.";
        announcerText.gameObject.transform.parent.gameObject.SetActive(true);

        CancelInvoke("HideAnnouncer"); // Cancel previous hide if any
        Invoke("HideAnnouncer", displayTime);
    }

    void HideAnnouncer() {
        announcerText.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
