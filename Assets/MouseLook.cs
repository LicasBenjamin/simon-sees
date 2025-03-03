using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Note: Much of this code is referenced from the following YouTube video by Brackeys:
//https://www.youtube.com/watch?v=_QajrabyTJc
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;   //Value to scale mouse sensitivity
    public Transform playerBody;            //Value to manipulate player's body transform
    
    float xRotation = 0f;                   //Value to keep track of player's xRotation
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Assign mouseX and mouseY values each frame
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Set the rotation of the up and down movement to a clamped value, then apply it to this transform
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Rotate player's body transform for moving mouseX (mouse left and right)
        playerBody.Rotate(Vector3.up * mouseX);

    }
}
