using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    public float bobFrequency = 4.5f;   // How fast the bob happens
    public float bobHorizontalAmount = 0.2f; // Side to side movement amount
    public float bobVerticalAmount = 0.15f;   // Up and down movement amount
    public float speedThreshold = 0.1f;        // Minimum speed to start bobbing

    public float maxShakeIntensity = 0.1f; // How strong the shake can get at maximum
    public float shakeGrowthRate = 0.005f;  // How fast the shake intensity grows
    //private Vector3 originalCameraPosition;
    public Announcer announcer; //Reference to see what

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

        Vector3 totalOffset = Vector3.zero;

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
        //Perform the actual head bob movement
        if (announcer.taskActive)
        {
            //Debug.Log("Camera should be shaking");
            float shakeAmount = Mathf.Min(maxShakeIntensity, shakeGrowthRate * announcer.taskAudioSource.time);
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * shakeAmount;

            //Debug.Log(shakeAmount);
            totalOffset += shakeOffset;
        }
        if (isMoving)
        {

            //Debug.Log("Bob Frequency: " + bobFrequency + " | Footstep Interval: " + footstepInterval);
            bobTimer += Time.deltaTime * bobFrequency;

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
            Vector3 bobOffset = new Vector3(horizontalBob, verticalBob, 0);

            totalOffset += bobOffset;
        }
        else
        {
            // Smoothly reset to start position when not moving
            if (!announcer.taskActive)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition, Time.deltaTime * 1f);
            }

            // If we're very close, snap to exact position
            if ((transform.localPosition - initialPosition).sqrMagnitude < 0.000001f)
            {
                transform.localPosition = initialPosition;
            }

            bobTimer = 0f;
            footstepTimer = 0.305f; // Reset footstep timer when not moving
        }
        // FINAL POSITION (combined shake + bob)
        transform.localPosition = initialPosition + totalOffset;
    }
    /**
    if (doingCorrectTask && taskAudioSource != null && taskAudioSource.isPlaying)
        {
            if (originalCameraPosition == Vector3.zero)
            {
                originalCameraPosition = mainCamera.transform.localPosition;
            }

            float shakeAmount = Mathf.Min(maxShakeIntensity, taskAudioSource.time * shakeGrowthRate);
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * shakeAmount;

            //Debug.Log(shakeOffset);
            mainCamera.transform.localPosition = originalCameraPosition + shakeOffset;
        }
        else
        {
            // Reset camera position when not doing task
            if (mainCamera != null)
            {
                mainCamera.transform.localPosition = originalCameraPosition;
            }
        }*/

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
