using UnityEngine;
using System.Collections;
using UnityEngine.Animations.Rigging;

public class ProceduralLegController : MonoBehaviour
{
    [System.Serializable]
    public class Leg
    {
        public string name;
        public Transform foot;             // IK foot (end effector)
        public Transform target;           // IK target
        public Transform hint;             // IK hint (knee)
        public float hintYOffset = 3.0f;
        public float hintZOffset = 0.0f;
        public float hintXOffset = 0.0f;
        public float stepHeight = 0.75f;
        public float stepDistance = 0.75f;
        public float stepCooldown = 0.2f;
        public Vector3 defaultOffset;      // Local offset for foot placement
        [HideInInspector] public bool isMoving;
        [HideInInspector] public Vector3 lastGroundedPos;
        [HideInInspector] public float lastStepTime;
        [HideInInspector] public Vector3 desiredTargetPos;
        [HideInInspector] public Vector3 lastBodyPos;
        [HideInInspector] public Quaternion initialFootRotation;
    }

    [Header("Legs: Order = RF, RB, LB, LF")]
    [SerializeField] private Leg[] legs = new Leg[4];

    [Header("IK Constraints")]
    [SerializeField] private TwoBoneIKConstraint[] footIKs = new TwoBoneIKConstraint[4];

    [Header("Grounding")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float sphereCastRadius = 0.2f;
    [SerializeField] private float sphereCastDistance = 2.0f;
    [SerializeField] private float footYOffset = 0.05f; // Prevents feet from clipping ground

    private Vector3 lastBodyPosition;

    void Start()
    {
        lastBodyPosition = transform.position;

        for (int i = 0; i < legs.Length; i++)
        {
            legs[i].lastGroundedPos = legs[i].foot.position;
            legs[i].desiredTargetPos = legs[i].foot.position;
            legs[i].lastBodyPos = transform.position;
            legs[i].initialFootRotation = legs[i].foot.rotation;
        }
    }

    void FixedUpdate()
    {
        Vector3 bodyDelta = transform.position - lastBodyPosition;

        // Update desired target positions
        for (int i = 0; i < legs.Length; i++)
        {
            Leg leg = legs[i];
            if (!leg.isMoving)
            {
                Vector3 worldTarget = transform.TransformPoint(leg.defaultOffset);
                Vector3 start = worldTarget + Vector3.up * 1.0f;
                if (Physics.SphereCast(start, sphereCastRadius, Vector3.down, out RaycastHit hit, sphereCastDistance, groundMask))
                {
                    leg.desiredTargetPos = hit.point + Vector3.up * footYOffset;
                }
            }

            // Adjust target position by body movement
            leg.desiredTargetPos += bodyDelta;
        }

        // Step logic
        for (int i = 0; i < legs.Length; i++)
        {
            Leg leg = legs[i];
            Leg oppositeLeg = legs[(i + 2) % 4];
            float dist = Vector3.Distance(leg.lastGroundedPos, leg.desiredTargetPos);

            if (!leg.isMoving &&
                Time.time - leg.lastStepTime > leg.stepCooldown &&
                dist > leg.stepDistance &&
                !oppositeLeg.isMoving)
            {
                StartCoroutine(StepLeg(leg, leg.desiredTargetPos));
            }
        }

        // Move targets and hints
        for (int i = 0; i < legs.Length; i++)
        {
            Leg leg = legs[i];
            if (!leg.isMoving)
            {
                leg.target.position = Vector3.Lerp(leg.target.position, leg.desiredTargetPos, 0.2f);
                leg.target.rotation = leg.initialFootRotation;
            }
            Vector3 footPos = leg.foot.position;
            Vector3 hintPos = new Vector3(footPos.x + leg.hintXOffset, footPos.y + leg.hintYOffset, footPos.z + leg.hintZOffset);
            leg.hint.position = hintPos;
        }

        lastBodyPosition = transform.position;
    }

    IEnumerator StepLeg(Leg leg, Vector3 newPos)
    {
        leg.isMoving = true;
        Vector3 start = leg.target.position;
        Vector3 end = newPos;
        float t = 0f;
        float duration = 0.18f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / duration;
            float yOffset = Mathf.Sin(Mathf.PI * t) * leg.stepHeight;
            leg.target.position = Vector3.Lerp(start, end, t) + Vector3.up * yOffset;
            yield return new WaitForFixedUpdate();
        }

        leg.target.position = end;
        leg.target.rotation = leg.initialFootRotation;
        leg.lastGroundedPos = end;
        leg.lastStepTime = Time.time;
        leg.isMoving = false;
    }
}

