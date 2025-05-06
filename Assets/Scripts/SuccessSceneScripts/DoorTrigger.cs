using UnityEngine;
using UnityEngine.Playables; // Required for Timeline control

public class DoorTrigger : MonoBehaviour
{
    public PlayableDirector timelineDirector; // Reference to Timeline
    public Camera mainCamera;
    public Camera cutsceneCamera;
    public PlayerController playerController;

    private bool hasActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;

            cutsceneCamera.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);

            mainCamera.gameObject.SetActive(false); // Hide gameplay camera
            cutsceneCamera.gameObject.SetActive(true); // Activate cutscene camera

            playerController.DisablePlayerControl();
            timelineDirector.Play(); // Start cutscene
        }
    }
}
