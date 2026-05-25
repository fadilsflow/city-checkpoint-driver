using UnityEngine;

public class CheckpointVisualPulse : MonoBehaviour
{
    public float rotateSpeed = 80f;
    public float bobHeight = 0.35f;
    public float bobSpeed = 2.5f;
    public float pulseAmount = 0.12f;

    private Vector3 startLocalPosition;
    private Vector3 startLocalScale;

    private void Awake()
    {
        startLocalPosition = transform.localPosition;
        startLocalScale = transform.localScale;
    }

    private void OnEnable()
    {
        startLocalPosition = transform.localPosition;
        startLocalScale = transform.localScale;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        float wave = Mathf.Sin(Time.time * bobSpeed);
        transform.localPosition = startLocalPosition + Vector3.up * (wave * bobHeight);
        transform.localScale = startLocalScale * (1f + Mathf.Abs(wave) * pulseAmount);
    }
}
