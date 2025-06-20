using System.Collections;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class SpaceshipLocomotion : MonoBehaviour
{
    private GameObject spaceship;

    [Header("Manual Control Settings")]
    [SerializeField] private float thrust = 1000f;
    [SerializeField] private float upThrust = 500f;
    [SerializeField] private float strafeThrust = 500f;
    [SerializeField] private float pitchTorque = 250f;
    [SerializeField] private float rollTorque = 250f;
    [SerializeField, Range(0.001f, 0.999f)] private float linearDamping = 0.98f;
    [SerializeField, Range(0.001f, 0.999f)] private float angularDamping = 0.95f;

    [Header("Manual Input Devices")]
    public XRKnob wheel;
    public XRLever lever;
    public XRJoystick joystick;

    [Header("Auto Navigation Settings")]
    public Transform targetObject;
    public float moveSpeed = 10f;
    public bool autoMoveEnabled = false;

    [Header("Orbit Settings")]
    public float orbitRadius;
    private bool isOrbiting = false;

    private Coroutine autoMoveCoroutine;

    private float forwardVelocity = 0f;
    private Vector3 currentAngularVelocity = Vector3.zero;

    private void Start()
    {
        spaceship = transform.parent.gameObject;
    }

    void Update()
    {
        if (autoMoveEnabled)
            return;

        float forwardInput = lever.value ? 1f : 0f;
        float sideInput = Mathf.Lerp(-1f, 1f, wheel.value);
        float upInput = Mathf.Clamp(joystick.value.y, -1f, 1f);
        float rollInput = Mathf.Clamp(joystick.value.x, -1f, 1f);

        if (forwardInput > 0f)
        {
            forwardVelocity += thrust * Time.deltaTime;
        }
        forwardVelocity *= linearDamping;
        transform.Translate(Vector3.forward * forwardVelocity * Time.deltaTime, Space.Self);

        Vector3 angularInput = new Vector3(
            upInput * pitchTorque,
            sideInput * pitchTorque,
            -rollInput * rollTorque
        );
        currentAngularVelocity += angularInput * Time.deltaTime;
        currentAngularVelocity *= angularDamping;
        transform.Rotate(currentAngularVelocity * Time.deltaTime, Space.Self);
    }

    public void MoveToTarget(Transform target)
    {
        if (spaceship != null)
        {
            targetObject = target;
            autoMoveEnabled = true;

            if (autoMoveCoroutine != null)
                StopCoroutine(autoMoveCoroutine);

            autoMoveCoroutine = StartCoroutine(MoveShipSmoothly(targetObject.position));
        }
        else
        {
            Debug.LogWarning("Target object or spaceship is not assigned.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OrbitZone") && !isOrbiting && autoMoveEnabled)
        {
            if (autoMoveCoroutine != null)
                StopCoroutine(autoMoveCoroutine);

            orbitRadius = other.GetComponent<SphereCollider>().radius;
            StartCoroutine(OrbitAroundPlanet(other.transform));
            Debug.Log("Entered orbit zone, starting orbit.");
        }
    }

    IEnumerator MoveShipSmoothly(Vector3 targetPosition)
    {
        Vector3 relativePosition = (targetPosition - spaceship.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(relativePosition);

        while (Quaternion.Angle(targetRotation, spaceship.transform.rotation) > 1f)
        {
            Vector3 pivotOffset = spaceship.transform.forward * 10f;
            Vector3 pivotPosition = spaceship.transform.position + pivotOffset;

            float angleStep = moveSpeed * Time.deltaTime * 2f;
            Vector3 rotationAxis = spaceship.transform.up;

            spaceship.transform.RotateAround(pivotPosition, rotationAxis, angleStep);

            Vector3 newDirection = Vector3.RotateTowards(
                spaceship.transform.forward,
                relativePosition.normalized,
                moveSpeed * Time.deltaTime * 0.075f,
                0.0f
            );

            spaceship.transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }

        while (Vector3.Distance(spaceship.transform.position, targetPosition) > 1f)
        {
            spaceship.transform.position = Vector3.MoveTowards(
                spaceship.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        autoMoveEnabled = false;
    }

    IEnumerator OrbitAroundPlanet(Transform orbitCenter)
    {
        Vector3 toPlanet = orbitCenter.position - spaceship.transform.position;
        Vector3 toTarget = targetObject.position - orbitCenter.position;
        Vector3 orbitAxis = Vector3.Cross(toPlanet, toTarget).normalized;

        float breakAngleThreshold = 10f; // degrees

        while (true)
        {
            // Orbit movement
            spaceship.transform.RotateAround(orbitCenter.position, orbitAxis, moveSpeed * Time.deltaTime);

            // Face orbit tangent
            Vector3 fromCenter = (spaceship.transform.position - orbitCenter.position).normalized;
            Vector3 tangent = Vector3.Cross(orbitAxis, fromCenter);
            spaceship.transform.rotation = Quaternion.LookRotation(tangent);

            // Check angle toward target
            Vector3 toFinalTarget = (targetObject.position - spaceship.transform.position).normalized;
            float angleToTarget = Vector3.Angle(spaceship.transform.forward, toFinalTarget);

            if (angleToTarget < breakAngleThreshold && isOrbiting)
            {
                Debug.Log($"Ship is aligned toward target (angle: {angleToTarget:F2}°). Exiting orbit.");
                break;
            }

            isOrbiting = true;

            yield return null;
        }

        isOrbiting = false;

        if (autoMoveCoroutine != null)
            StopCoroutine(autoMoveCoroutine);

        autoMoveCoroutine = StartCoroutine(MoveShipSmoothly(targetObject.position));
    }
}
