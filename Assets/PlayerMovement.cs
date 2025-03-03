using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Note: Much of this code is referenced from the following YouTube video by Brackeys:
//https://www.youtube.com/watch?v=_QajrabyTJc
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;  //Reference to character controller to apply movement vector to
    public float speed = 12f;               //Value for speed of character's movement

    public Transform groundCheck;
    public float groundDistance = 0.4f;     //Used for the radius of a sphere to check for ground
    public LayerMask groundMask;
    public float gravity = -9.81f;

    Vector3 velocity;
    bool isGrounded;
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; //adds a little more force downward on the player
        }
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //x is negative 1 if left, positive 1 if right
        //z is negative 1 if backward, positive 1 if forward
        Vector3 move = (transform.right * x + transform.forward * z).normalized;

        //Move by vector generated adjusting to speed and framerate
        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
