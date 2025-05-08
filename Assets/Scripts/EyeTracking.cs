using UnityEngine;

public class EyeTrack : MonoBehaviour
{
    public Transform target; // Assign your target bone in Inspector

    void LateUpdate()
    {
        if (target)
        {
            // Make local Z axis point at the target, Y as up
            transform.LookAt(target.position, Vector3.up);
        }
    }
}
