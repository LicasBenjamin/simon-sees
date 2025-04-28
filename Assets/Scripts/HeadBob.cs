using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobFrequency = 4.5f;   // How fast the bob happens
    public float bobHorizontalAmount = 0.2f; // Side to side movement amount
    public float bobVerticalAmount = 0.15f;   // Up and down movement amount
    public float speedThreshold = 0.1f;        // Minimum speed to start bobbing

    private float bobTimer = 0f;
    private Vector3 initialPosition;

    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private AudioSource soundFXObject;
    private float footstepTimer = 0.69f;
    public float footstepInterval = 0.69f;

    PlayerMovement playerMovementScript;
    void Start()
    {
        initialPosition = transform.localPosition;
        playerMovementScript = GetComponentInParent<PlayerMovement>();
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMoving = input.magnitude > 0;
        
        //Below is essentially a double nested if else loop for running and the stamina bar
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool canRun = playerMovementScript.staminaRegenRate == 15f;


        if (canRun)
        {
            bobFrequency = isRunning ? 7f : 4.5f;
            footstepInterval = isRunning ? 0.46f : 0.69f;
        }
        else
        {
            bobFrequency = 4.5f;
            footstepInterval = 0.69f;
        }

        if (isMoving)
        {
            
            //Debug.Log("Bob Frequency: " + bobFrequency + " | Footstep Interval: " + footstepInterval);
            bobTimer += Time.deltaTime * bobFrequency ;

            float horizontalBob = (0.5f) * Mathf.Sin(bobTimer) * bobHorizontalAmount;
            float verticalBob = -Mathf.Abs(Mathf.Sin(bobTimer)) * bobVerticalAmount;
            // Update footstep timer
            footstepTimer -= Time.deltaTime;

            if (footstepTimer <= 0f)
            {
                PlayFootstepSound();
                footstepTimer = footstepInterval; // Reset timer
                //Debug.Log("Footstep Timer: " + footstepInterval);
            }

            transform.localPosition = initialPosition + new Vector3(horizontalBob, verticalBob, 0);
        }
        else
        {
            // Smoothly reset to start position when not moving
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 1f);

            // If we're very close, snap to exact position
            if ((transform.localPosition - initialPosition).sqrMagnitude < 0.000001f)
            {
                transform.localPosition = initialPosition;
            }

            bobTimer = 0f;
            footstepTimer = 0.305f; // Reset footstep timer when not moving
        }
    }
    void PlayFootstepSound()
    {
        int rand = Random.Range(0, footstepSounds.Length);
        AudioSource footstep = Instantiate(soundFXObject, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        footstep.spatialBlend = 1f;
        footstep.clip = footstepSounds[rand];
        footstep.volume = 0.8f;
        footstep.pitch = Random.Range(0.8f, 1.2f);
        footstep.Play();
        float clipLength = footstep.clip.length;
        Destroy(footstep.gameObject, clipLength);
    }
}
