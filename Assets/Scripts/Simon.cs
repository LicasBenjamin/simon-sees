using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simon : MonoBehaviour
{
    [Header("Positions for errors")]
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 error2Pos;
    [SerializeField] Vector3 error3Pos;

    [Header("Bobbing Settings")]
    [SerializeField] float bobFrequency = 1.0f;
    [SerializeField] float bobAmplitude = 0.1f;

    [Header("Rotation Settings")]
    [SerializeField] float rotationAngle = 3.0f;
    [SerializeField] float rotationSpeed = 1.0f;

    private AudioSource secondErrorMovementSound;
    private Coroutine moveCoroutine;
    private bool isMoving = false;
    private float originalY;
    private float bobTimer = 0f;

    private void Start()
    {
        secondErrorMovementSound = GetComponent<AudioSource>();
        originalY = transform.position.y;
    }

    private void Update()
    {
        if (!isMoving)
        {
            ApplyBobbingEffect();
        }
        else
        {
            ApplyRotationEffect();
        }
    }

    private void ApplyBobbingEffect()
    {
        if (isMoving) return;
        bobTimer += Time.deltaTime * bobFrequency;
        float newY = originalY + Mathf.Sin(bobTimer) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void ApplyRotationEffect()
    {
        float rotationZ = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);
    }

    private void MoveToPosition(Vector3 targetPos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        isMoving = true;
        secondErrorMovementSound.Play();
        moveCoroutine = StartCoroutine(MoveOverTime(startPos, targetPos, secondErrorMovementSound.clip.length + 5f));
    }

    public void triggerSecondErrorMovement()
    {
        secondErrorMovementSound.Play();
        MoveToPosition(error2Pos);
    }

    private IEnumerator MoveOverTime(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Teleport exactly to the target position at the end
        transform.position = endPos;

        // Update originalY to match the new position for correct bobbing
        originalY = endPos.y;

        isMoving = false;
    }
}

