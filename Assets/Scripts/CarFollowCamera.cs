using UnityEngine;

public class CarFollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 4.2f, -7.5f);
    public float positionSmooth = 8f;
    public float rotationSmooth = 10f;
    public float lookAhead = 4f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmooth * Time.deltaTime);

        Vector3 lookTarget = target.position + target.forward * lookAhead + Vector3.up * 1.1f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmooth * Time.deltaTime);
    }
}
