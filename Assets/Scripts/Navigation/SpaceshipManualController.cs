using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class SpaceshipManualControl : MonoBehaviour
{
    [Header("Input Components")]
    public XRLever lever;
    public XRKnob wheel;
    public XRJoystick joystick;

    [Header("Movement Settings")]
    public float thrust = 5f;
    public float pitchTorque = 30f;
    public float rollTorque = 30f;
    public float linearDamping = 0.98f;
    public float angularDamping = 0.95f;

    private float forwardVelocity = 0f;
    private Vector3 currentAngularVelocity = Vector3.zero;

    void Update()
    {
        if (SpaceshipController.Instance == null) return;

        if (SpaceshipController.Instance.currentMode != SpaceshipController.TravelMode.Manual
            || !SpaceshipController.Instance.isMoving)
            return;

        // Get input
        float forwardInput = lever != null && lever.value ? 1f : 0f;
        float sideInput = wheel != null ? Mathf.Lerp(-1f, 1f, wheel.value) : 0f;
        float upInput = joystick != null ? Mathf.Clamp(joystick.value.y, -1f, 1f) : 0f;
        float rollInput = joystick != null ? Mathf.Clamp(joystick.value.x, -1f, 1f) : 0f;

        // Thrust
        if (forwardInput > 0f)
        {
            forwardVelocity += thrust * Time.deltaTime;
        }
        forwardVelocity *= linearDamping;

        transform.Translate(Vector3.forward * forwardVelocity * Time.deltaTime, Space.Self);

        // Rotation
        Vector3 angularInput = new Vector3(
            upInput * pitchTorque,
            sideInput * pitchTorque,
            -rollInput * rollTorque
        );

        currentAngularVelocity += angularInput * Time.deltaTime;
        currentAngularVelocity *= angularDamping;

        transform.Rotate(currentAngularVelocity * Time.deltaTime, Space.Self);
    }
}
