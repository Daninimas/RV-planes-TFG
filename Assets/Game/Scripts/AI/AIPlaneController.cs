using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlaneController : PlaneController
{
    [Header("Plane IA Control")]
    [SerializeField]
    private Limit2D velocityLimits;

    public Vector3 targetPosition;
    public Transform pruebaTargetPosition;


    PlanePhysics _planePhysics;
    Vector3 _lastInput;

    // Start is called before the first frame update
    void Start()
    {
        _planePhysics = this.GetComponent<PlanePhysics>();
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -100);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = pruebaTargetPosition.position;
        float dt = Time.fixedDeltaTime;

        CalculateThrottle();
        CalculateSteering(dt);
    }


    /// <summary>
    /// Hace que mantenga su velocidad de crucero
    /// </summary>
    void CalculateThrottle()
    {
        mixtureControlNormal = 0.5f;

        if (_planePhysics.LocalVelocity.z < velocityLimits.min)
        {
            mixtureControlNormal = 1;
        }
        else if (_planePhysics.LocalVelocity.z > velocityLimits.max)
        {
            mixtureControlNormal = 0;
        }
    }


    void CalculateSteering(float dt)
    {
        float pitchUpThreshold = -15f;
        float fineSteeringAngle = 10;
        float rollBias = 0.01f; // Para evitar que esté haciendo pequeños giros todo el rato
        float steeringSpeed = 5f;

        var error = targetPosition - transform.position;
        error = Quaternion.Inverse(transform.rotation) * error;   //transform into local space

        var errorDir = error.normalized;
        var pitchError = new Vector3(0, error.y, error.z).normalized;
        var rollError = new Vector3(error.x, error.y, 0).normalized;

        var targetInput = new Vector3();

        var pitch = Vector3.SignedAngle(Vector3.forward, pitchError, Vector3.right);
        if (-pitch < pitchUpThreshold) pitch += 360f;
        targetInput.x = pitch;

        if (Vector3.Angle(Vector3.forward, errorDir) < fineSteeringAngle)
        {
            targetInput.y = error.x;
        }
        else
        {
            var roll = Vector3.SignedAngle(Vector3.up, rollError, Vector3.forward);
            targetInput.z = (roll+180) * rollBias;
        }

        targetInput.x = Mathf.Clamp(targetInput.x, -1, 1);
        targetInput.y = Mathf.Clamp(targetInput.y, -1, 1);
        targetInput.z = Mathf.Clamp(targetInput.z, -1, 1);

        var input = Vector3.MoveTowards(_lastInput, targetInput, steeringSpeed * dt);
        _lastInput = input;

        this.joystickNormal.x = input.x;
        this.rotateYawNormal = input.y;
        this.joystickNormal.y = input.z;
        /*
        /// Mi implementacion
        Plane planeYZ = new Plane(transform.right, transform.position); // Plano para saber el pitch
        Plane planeXZ = new Plane(transform.up, transform.position); // Plano para saber el rol y yaw

        // Calculamos el cabeceo (Pitch)
        Vector3 targetPitchPosition = new Vector3(transform.position.x, targetPosition.y, targetPosition.z);
        float anglePitch = Vector3.SignedAngle(transform.position, targetPitchPosition, Vector3.right);
        Debug.Log("anglePitch: " + anglePitch);
        Vector3 targetInput = new Vector3();

        var input = Vector3.MoveTowards(_lastInput, targetInput, steeringSpeed * dt);
        _lastInput = input;

        this.joystickNormal.x = input.x;
        this.rotateYawNormal = input.y;
        this.joystickNormal.y = input.z;*/
    }
}
