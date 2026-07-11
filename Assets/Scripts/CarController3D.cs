using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController3D : MonoBehaviour
{
    [Header("Wheel Colliders")]
    public WheelCollider frontLeftCollider;
    public WheelCollider frontRightCollider;
    public WheelCollider rearLeftCollider;
    public WheelCollider rearRightCollider;

    [Header("Wheel Visuals")]
    public Transform frontLeftVisual;
    public Transform frontRightVisual;
    public Transform rearLeftVisual;
    public Transform rearRightVisual;

    [Header("Driving")]
    public float motorTorque = 1650f;
    public float reverseTorque = 900f;
    public float brakeTorque = 3200f;
    public float handBrakeTorque = 5200f;
    public float maxSteerAngle = 32f;
    public float steerResponse = 9f;
    public float maxSpeedKph = 130f;
    public float downforce = 45f;

    [Header("Stability")]
    public Vector3 centerOfMass = new Vector3(0f, -0.45f, 0.05f);
    public float lateralDrag = 2.5f;

    [Header("Mobile Input")]
    public MobileCarControls mobileControls;
    public MobileBrakeButton brakeButton;
    public bool useMobileControls = true;

    private Rigidbody rb;
    private float steerAngle;
    private float throttle;
    private bool braking;
    private bool handBraking;
    private float steerInputCached;

    private Quaternion frontLeftVisualOffset = Quaternion.identity;
    private Quaternion frontRightVisualOffset = Quaternion.identity;
    private Quaternion rearLeftVisualOffset = Quaternion.identity;
    private Quaternion rearRightVisualOffset = Quaternion.identity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        frontLeftVisualOffset = GetVisualRotationOffset(frontLeftCollider, frontLeftVisual);
        frontRightVisualOffset = GetVisualRotationOffset(frontRightCollider, frontRightVisual);
        rearLeftVisualOffset = GetVisualRotationOffset(rearLeftCollider, rearLeftVisual);
        rearRightVisualOffset = GetVisualRotationOffset(rearRightCollider, rearRightVisual);
    }

    private void Update()
    {
        ResolveMobileControls();

        Vector2 mobileInput = GetMobileInput();
        throttle = UseStrongerAxis(Input.GetAxis("Vertical"), mobileInput.y);
        bool mobileBrake = brakeButton != null && brakeButton.IsPressed;
        braking = Input.GetKey(KeyCode.Space) || mobileBrake;
        handBraking = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        steerInputCached = UseStrongerAxis(Input.GetAxis("Horizontal"), mobileInput.x);
        UpdateWheelVisuals();
    }

    private void FixedUpdate()
    {
        ResolveMobileControls();

        float speedKph = rb.linearVelocity.magnitude * 3.6f;
        float speedSteerFactor = Mathf.Lerp(1f, 0.45f, Mathf.InverseLerp(25f, maxSpeedKph, speedKph));
        steerAngle = Mathf.Lerp(steerAngle, steerInputCached * maxSteerAngle * speedSteerFactor, steerResponse * Time.fixedDeltaTime);

        frontLeftCollider.steerAngle = steerAngle;
        frontRightCollider.steerAngle = steerAngle;

        float torque = 0f;
        if (speedKph < maxSpeedKph || Mathf.Sign(throttle) != Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward)))
            torque = throttle >= 0f ? throttle * motorTorque : throttle * reverseTorque;

        rearLeftCollider.motorTorque = torque;
        rearRightCollider.motorTorque = torque;
        frontLeftCollider.motorTorque = 0f;
        frontRightCollider.motorTorque = 0f;

        float appliedBrake = braking ? brakeTorque : 0f;
        frontLeftCollider.brakeTorque = appliedBrake;
        frontRightCollider.brakeTorque = appliedBrake;
        rearLeftCollider.brakeTorque = handBraking ? handBrakeTorque : appliedBrake;
        rearRightCollider.brakeTorque = handBraking ? handBrakeTorque : appliedBrake;

        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude, ForceMode.Force);
        ApplyLateralGrip();
    }

    private void ApplyLateralGrip()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x = Mathf.Lerp(localVelocity.x, 0f, lateralDrag * Time.fixedDeltaTime);
        rb.linearVelocity = transform.TransformDirection(localVelocity);
    }

    private void ResolveMobileControls()
    {
        if (!useMobileControls || mobileControls != null) return;
        mobileControls = FindFirstObjectByType<MobileCarControls>();
    }

    private Vector2 GetMobileInput()
    {
        if (!useMobileControls || mobileControls == null) return Vector2.zero;
        return mobileControls.Value;
    }

    private static float UseStrongerAxis(float keyboardAxis, float mobileAxis)
    {
        return Mathf.Abs(mobileAxis) > Mathf.Abs(keyboardAxis) ? mobileAxis : keyboardAxis;
    }

    public void ResetCarState()
    {
        throttle = 0f;
        braking = false;
        handBraking = false;
        steerAngle = 0f;

        ResetWheel(frontLeftCollider);
        ResetWheel(frontRightCollider);
        ResetWheel(rearLeftCollider);
        ResetWheel(rearRightCollider);
        if (mobileControls != null) mobileControls.ResetInput();
        UpdateWheelVisuals();
    }

    private static void ResetWheel(WheelCollider wheel)
    {
        if (wheel == null) return;
        wheel.motorTorque = 0f;
        wheel.brakeTorque = 0f;
        wheel.steerAngle = 0f;
    }

    private void UpdateWheelVisuals()
    {
        UpdateWheel(frontLeftCollider, frontLeftVisual, frontLeftVisualOffset);
        UpdateWheel(frontRightCollider, frontRightVisual, frontRightVisualOffset);
        UpdateWheel(rearLeftCollider, rearLeftVisual, rearLeftVisualOffset);
        UpdateWheel(rearRightCollider, rearRightVisual, rearRightVisualOffset);
    }

    private static Quaternion GetVisualRotationOffset(WheelCollider collider, Transform visual)
    {
        if (collider == null || visual == null) return Quaternion.identity;
        collider.GetWorldPose(out _, out Quaternion wheelRotation);
        return Quaternion.Inverse(wheelRotation) * visual.rotation;
    }

    private static void UpdateWheel(WheelCollider collider, Transform visual, Quaternion visualOffset)
    {
        if (collider == null || visual == null) return;
        collider.GetWorldPose(out Vector3 position, out Quaternion rotation);
        visual.SetPositionAndRotation(position, rotation * visualOffset);
    }
}
