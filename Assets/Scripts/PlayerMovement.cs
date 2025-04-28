using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Note: A portion of this code is referenced from the following YouTube video by Brackeys:
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

    public float stamina = 100f;
    public float maxStamina = 100f;
    public float staminaDrainRate = 25f;  // How much stamina drains per second when running
    public float staminaRegenRate = 15f;  // How much stamina regenerates per second when not running
    private bool isRunning;

    // Reference to the UI Slider that represents stamina
    public Slider staminaSlider;
    public GameObject staminaBarObject;

    // Update is called once per frame
    void FixedUpdate()
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

        /**
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        if (isRunning)
        {
            controller.Move(move * speed * 1.5f * Time.deltaTime);
        }
        else
        {
            //Move by vector generated adjusting to speed and framerate
            controller.Move(move * speed * Time.deltaTime);
        }*/

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && stamina > 0f;

        isRunning = wantsToRun;

        if (stamina >= 20f && staminaRegenRate == 5f)
        {
            staminaRegenRate = 15f; //restore back stamina regen amt
        }

        if (isRunning && staminaRegenRate != 5f)
        {
            //Debug.Log("Running");
            controller.Move(move * speed * 1.5f * Time.deltaTime);
            stamina -= staminaDrainRate * Time.deltaTime;
            if (stamina < 0f)
            {
                stamina = 0;
                staminaRegenRate = 5f; //temporarily punish player for using up all stamina
            }
        }
        else
        {
            //Debug.Log("Not Running");
            controller.Move(move * speed * Time.deltaTime);
            if (stamina < maxStamina)
                stamina += staminaRegenRate * Time.deltaTime;
            if (stamina > maxStamina)
                stamina = maxStamina;
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // Update the slider's value to reflect the current stamina
        }
        if (staminaBarObject != null)
        {
            bool shouldBeVisible = stamina < maxStamina;
            if (staminaBarObject.activeSelf != shouldBeVisible)
            {
                staminaBarObject.SetActive(shouldBeVisible);
            }
        }


        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        /**
        if (Input.GetKeyDown(KeyCode.E))
        {
            print(transform.position);
        }*/
    }
}
