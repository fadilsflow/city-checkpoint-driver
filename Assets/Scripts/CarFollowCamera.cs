using UnityEngine;

public class CarFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4.2f, -7.5f);
    public float positionSmoothTime = 0.12f;
    public float rotationSpeed = 120f;
    public float lookAhead = 2f;

    private Vector3 posVelocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref posVelocity, positionSmoothTime);

        Vector3 lookTarget = target.position + target.forward * lookAhead + Vector3.up * 1.1f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
    }
}
