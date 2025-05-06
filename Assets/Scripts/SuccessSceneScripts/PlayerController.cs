using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 100f;

    private CharacterController controller;
    private float xRotation = 0f;
    private bool canMove = false;
    private HeadBobScene headBob; // Reference to HeadBobScene

    public Camera mainCamera;
    public Camera cutsceneCamera;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        headBob = GetComponentInChildren<HeadBobScene>(); // Finds HeadBobScene script

        cutsceneCamera.transform.SetPositionAndRotation(mainCamera.transform.position, mainCamera.transform.rotation);

        Cursor.lockState = CursorLockMode.Locked;
        canMove = false; // Player starts with control disabled
    }

    void Update()
    {
        if (!canMove) return;

        MovePlayer();
        RotatePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(Vector3.up * mouseX);
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void EnablePlayerControl()
    {
        Debug.Log("Player control enabled!");
        canMove = true;

        if (headBob != null)
        {
            headBob.enabled = true; // Enable head bobbing
        }
    }

    public void DisablePlayerControl()
    {
        Debug.Log("Player control disabled!");
        canMove = false;

        // Ensure movement input is completely ignored
        controller.enabled = false;

        if (headBob != null)
        {
            headBob.enabled = false;
        }
    }

}
