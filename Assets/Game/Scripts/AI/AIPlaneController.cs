using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AIPlaneController : PlaneController
{
    [Header("Plane IA Control")]
    [SerializeField]
    private Limit2D velocityLimits;

    public Vector3 targetPosition;
    public Transform pruebaTargetPosition;

    [Space]
    [Header("Plane IA Bombs")]
    [SerializeField]
    List<ExplosiveAI> planeBoms;
    [SerializeField]
    [Min(0)]
    float bombDropCooldown;
    [Header("Prediction variables")]
    [SerializeField]
    int maxPredictionSteps;
    [SerializeField]
    [Min(1)]
    int predictionFramesJump; // Cuantos frames te saltas entre los pasos de la predicción (a cuantos más, más lejos llega la predicción, pero pierde precisión)
    [SerializeField]
    GameObject predictionMarker;
    [SerializeField]
    float markerSize = 0.005f;

    PlanePhysics _planePhysics;
    Vector3 _lastInput;
    Vector3 _bombsCenter; // La posicion media de las bombas sin lanzar (Sirve para la predicción) -> Esto es en posicion local
    float _currentBombColdown = 0f;
    bool _bombDropActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        _planePhysics = this.GetComponent<PlanePhysics>();
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -100);

        predictionMarker.gameObject.SetActive(false);

        _bombsCenter = calculateBomsCenterPosition();

        foreach(ExplosiveAI bomb in planeBoms)
        {
            bomb.transform.parent = null;
        }
        predictionMarker.transform.parent = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        targetPosition = pruebaTargetPosition.position;
        float dt = Time.fixedDeltaTime;

        CalculateThrottle();
        CalculateSteering(dt);


        // For the bombs
        manageBombs(dt);
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



    void manageBombs(float dt)
    {
        if (planeBoms.Count > 0) {
            _currentBombColdown += dt;

            if (predictBombDrop(_bombsCenter + transform.position)) // Comprobar si va a caer la bomba en un punto donde tiene que bombardear
            {
                Debug.Log("Suelto bombas");
                _bombDropActivated = true;
            }

            if(_bombDropActivated && _currentBombColdown > bombDropCooldown)
            {
                planeBoms[0].dropBomb();
                planeBoms.RemoveAt(0);
                calculateBomsCenterPosition();
                _currentBombColdown = 0f;
            }
        }
    }
    
    bool predictBombDrop(Vector3 bombsCenterPosition)
    {
        Debug.Log(bombsCenterPosition);
        Vector3 lastPosition = bombsCenterPosition;
        float stepSize = Time.fixedDeltaTime * predictionFramesJump;
        Vector3 predictedBombVelocity = this.GetComponent<Rigidbody>().velocity;
        LayerMask layermask = Utils.GetPhysicsLayerMask(planeBoms[0].gameObject.layer);
        bool hitSomething = false;

        for (int step = 0; step < maxPredictionSteps && !hitSomething; ++step)
        {
            //predictionMarker.gameObject.SetActive(false);

            predictedBombVelocity += Physics.gravity * stepSize;
            Vector3 newPosition = lastPosition + predictedBombVelocity * stepSize;

            // Calcular si ha colisionado con algo entre estos puntos
            RaycastHit[] entitiesHit = Physics.RaycastAll(lastPosition, (newPosition - lastPosition).normalized, (newPosition - lastPosition).magnitude, layermask);
            // Ponemos el marcador en el primer objeto con el que choca
            foreach (RaycastHit entityHit in entitiesHit)
            {
                if (entityHit.collider.gameObject.layer != gameObject.layer)
                {
                    predictionMarker.gameObject.SetActive(true);
                    predictionMarker.transform.position = entityHit.point;
                    predictionMarker.transform.forward = entityHit.normal;
                    float scale = 0.005f * Vector3.Distance(Camera.main.transform.position, entityHit.point);
                    predictionMarker.transform.localScale = new Vector3(scale, scale, scale);
                    hitSomething = true;
                    break;
                }
            }

            lastPosition = newPosition;
        }

        return hitSomething;
    }


    Vector3 calculateBomsCenterPosition()
    {
        Vector3 center = new Vector3();

        foreach(ExplosiveAI bomb in planeBoms)
        {
            ParentConstraint constraint = bomb.GetComponent<ParentConstraint>();
            center += constraint.translationOffsets[0];
        }
        center /= planeBoms.Count;

        return center;
    }
}
