using UnityEngine;

public class ArenaCameraRig : MonoBehaviour
{
    private static ArenaCameraRig instance;

    [SerializeField] private Transform targetA;
    [SerializeField] private Transform targetB;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4.8f, -13.8f);
    [SerializeField] private float positionSmoothTime = 0.1f;
    [SerializeField] private float rotationSmooth = 10f;
    [SerializeField] private float verticalLookOffset = 2.25f;

    private Vector3 velocity;
    private float shakeTime;
    private float shakeMagnitude;

    private void Awake()
    {
        instance = this;
    }

    public void SetTargets(Transform a, Transform b)
    {
        targetA = a;
        targetB = b;
    }

    public static void TriggerShake(float magnitude, float duration)
    {
        if (instance == null)
        {
            return;
        }

        instance.shakeMagnitude = Mathf.Max(instance.shakeMagnitude, magnitude);
        instance.shakeTime = Mathf.Max(instance.shakeTime, duration);
    }

    private void LateUpdate()
    {
        if (targetA == null || targetB == null)
        {
            return;
        }

        Vector3 midpoint = (targetA.position + targetB.position) * 0.5f;
        Vector3 desiredPosition = new Vector3(midpoint.x + offset.x, offset.y, offset.z);
        if (shakeTime > 0f)
        {
            shakeTime -= Time.deltaTime;
            Vector2 jitter = Random.insideUnitCircle * shakeMagnitude;
            desiredPosition += new Vector3(jitter.x, jitter.y, 0f);
        }
        else
        {
            shakeMagnitude = 0f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

        Vector3 lookTarget = new Vector3(midpoint.x, verticalLookOffset, 0f);
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmooth * Time.deltaTime);
    }
}
