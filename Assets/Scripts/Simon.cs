using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simon : MonoBehaviour
{
    [Header("Positions for errors")]
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 error2Pos;
    [SerializeField] Vector3 error3Pos;

    private AudioSource secondErrorMovementSound;
    private void Start()
    {
        secondErrorMovementSound = GetComponent<AudioSource>();
    }
    public void triggerSecondErrorMovement()
    {
        secondErrorMovementSound.Play(); //Play spooky sound
        StartCoroutine(MoveOverTime(startPos, error2Pos, secondErrorMovementSound.clip.length+5f)); //for now, just lerp from start pos to error2pos
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

        transform.position = endPos; // Snap to final position
    }
}
