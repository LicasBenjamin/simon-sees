using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Announcer : MonoBehaviour {
    public TextMeshProUGUI announcerText;
    public float displayTime = 5f; // How long to show the message

    // Start is called before the first frame update
    void Start() {
        // Setting the "welcome" message
        announcerText.text = "What up lil bitch";
        Invoke("HideAnnouncer", displayTime);
    }

    void HideAnnouncer() {
        // Deactivate game object to hide
        announcerText.gameObject.transform.parent.gameObject.SetActive(false);
    }
}
