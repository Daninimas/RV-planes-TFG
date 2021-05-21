using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class PlanePhysics : MonoBehaviour
{
    [Header("Lift")]
    [SerializeField]
    AnimationCurve aileronsLiftAOACurve;
    [SerializeField]
    AnimationCurve rudderLiftAOACurve;

    [Header("Drag")]
    [SerializeField]
    float dragCoefficientAtZeroLift;
    [SerializeField]
    Vector3 angularDrag = new Vector3(1f, 1f, 1f);

    [Header("Steering")]
    [SerializeField]
    Vector3 turnSpeed;
    [SerializeField]
    Vector3 turnAcceleration;
    [SerializeField]
    AnimationCurve steeringCurve;

    [Header("Stabilization")]
    [SerializeField]
    Vector3 maxStabilizationTorque;
    [SerializeField]
    AnimationCurve stabilizationYCurve;


    // Variables publicas
    public Rigidbody Rigidbody { get; private set; }
    [Header("Control")]
    public PlaneController planeController;

    // Variables internas
    private float inducedDragCoeficient = 0f;
    [SerializeField]
    private Vector3 controlInput;
    public Vector3 Velocity { get; private set; }
    public Vector3 LocalVelocity { get; private set; }
    public Vector3 LocalAngularVelocity { get; private set; }
    public float AngleOfAttack { get; private set; }
    public float AngleOfAttackYaw { get; private set; }
    

    [Header("Hay que cambiar de sitio")]
    // Variables que hay que poner en otros lados
    [SerializeField]
    float maxThrust; // Esto pertenece al motor
    [SerializeField]
    float airDensity; // Esto pertenece al mundo
    [SerializeField]
    float wingSurface; // Esto pertenece a las alas del avion
    [SerializeField]
    float rotateYawNormalValue = 0;
    [SerializeField]
    InputActionReference RotateYawActionRef = null;


    public ActionBasedController controller;

    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();

        /// TODO: Quitar de aqui y poner en las alas
        // Mejor forma de obtener input con inputXR basado en acciones de esta forma de suscribes cuando se activa y cuando se desactiva
        // Cuando se pulsa un boton, se llama a una funcion
        RotateYawActionRef.action.started += UpdateYawNormal;
        RotateYawActionRef.action.canceled += UpdateYawNormal;
    }
    private void OnDestroy()
    {
        // Desuscribirse de las acciones cuando se destruya el objeto
        RotateYawActionRef.action.started -= UpdateYawNormal;
        RotateYawActionRef.action.canceled -= UpdateYawNormal;
    }

    /// TODO: Quitar de aqui y poner en las alas
    private void UpdateYawNormal(InputAction.CallbackContext context)
    {
        rotateYawNormalValue = context.action.ReadValue<float>();
    }



    void FixedUpdate()
    {
        UpdateControlInput();
        float dt = Time.fixedDeltaTime;
        inducedDragCoeficient = 0f;

        //calculate at start, to capture any changes that happened externally
        UpdateState();

        //apply updates
        UpdateThrust();
        UpdateLift();

        UpdateSteering(dt);

        UpdateDrag(wingSurface);
        UpdateAngularDrag();

        //calculate again, so that other systems can read this plane's state
        UpdateState();

        StabilizeFlight(dt);
    }

    /// <summary>
    /// Actualiza los controles de dirección del jugador mirando la palanca de control y los pedales y lo mete en un Vector3
    /// </summary>
    void UpdateControlInput()
    {
        // X -> Cabeceo    Y -> Guinyada    Z -> Alabeo
        controlInput = new Vector3(planeController.joystick.joystickNormal.x, rotateYawNormalValue, -planeController.joystick.joystickNormal.y);
    }

    /// <summary>
    /// Calculamos los datos del estado del avión que usaremos luego
    /// </summary>
    void UpdateState()
    {
        Quaternion invRotation = Quaternion.Inverse(Rigidbody.rotation);
        Velocity = Rigidbody.velocity;
        LocalVelocity = invRotation * Velocity;  //transform world velocity into local space
        LocalAngularVelocity = invRotation * Rigidbody.angularVelocity;  //transform into local space
        CalculateAngleOfAttack();
    }

    /// <summary>
    /// Calculamos los angulos de ataque que lleva de la guiñada y el alabeo, se hace Mathf.Atan2
    /// para obtener el angulo sin que de problemas de signo
    /// </summary>
    void CalculateAngleOfAttack()
    {
        if (LocalVelocity.sqrMagnitude < 0.1f)
        {
            AngleOfAttack = 0;
            AngleOfAttackYaw = 0;
            return;
        }

        AngleOfAttack = Mathf.Atan2(-LocalVelocity.y, LocalVelocity.z);
        AngleOfAttackYaw = Mathf.Atan2(LocalVelocity.x, LocalVelocity.z);
    }

    /// <summary>
    /// Calculo del empuje del motor. Throttle es la potencia que asigna el jugador entre 0 y 1
    /// maxThrust es la potencia maxima del avion en CV
    /// </summary>
    void UpdateThrust()
    {
        //Rigidbody.AddRelativeForce(controller.activateAction.action.ReadValue<float>() * maxThrust * Vector3.forward);
        Rigidbody.AddRelativeForce(planeController.mixtureControl.mixtureControlNormal * maxThrust * Vector3.forward);
    }

    /// <summary>
    /// Llama a calcular las fuerzas de sustentacion de las alas del avion y aplica esta fuerza en el avion
    /// </summary>
    void UpdateLift()
    {
        if (LocalVelocity.sqrMagnitude < 1f) return; // Si la velocidad es muy baja, no hace falta calcular nada

        var liftForce = CalculateLift(
            AngleOfAttack, Vector3.right,
            aileronsLiftAOACurve,
            airDensity,
            wingSurface
        );

        //var yawForce = CalculateLift(AngleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);

        Rigidbody.AddRelativeForce(liftForce);
        //Rigidbody.AddRelativeForce(yawForce);
    }

    /// <summary>
    /// Funcion generica del calculo de la fuerza de sustentacion. L = Cl * 0.5 * p * V^2 * A
    ///  Cl -> Coeficiente de sustentacion
    ///  p -> Densidad del aire
    ///  V -> Velocidad del avion en el aire
    ///  S -> Superficie del ala
    ///  Tambien hay que quitar la resistencia inducida
    /// </summary>
    ///  <param name="angleOfAttack">Angulo de ataque sobre el aire</param>
    ///  <param name="rightAxis">Eje sobre el que se produce la sustentacion</param>
    ///  <param name="aoaCurve">Angulo de ataque sobre el aire</param>
    ///  <param name="wingSurface">Area del ala sobre la que se aplica esta formula</param>
    Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, AnimationCurve aoaCurve, float airDensity, float wingSurface)
    {
        // Proyeccion de la velicidad en el plano YZ (aire)
        Vector3 liftVelocity = Vector3.ProjectOnPlane(LocalVelocity, rightAxis);

        // Sacamos el coeficiente con nuestra curva dependiendo del angulo de ataque
        float liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);

        // Calcular la fuerza de sustentacion con la formula
        float liftForce = liftCoefficient * 0.5f * airDensity * liftVelocity.sqrMagnitude * wingSurface;

        // La sustentacion es perpendicular a la velocidad
        Vector3 liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        Vector3 lift = liftDirection * liftForce;

        /// TODO MEJORA (AÑADIR induced drag)
        // Ahora que tenemos la sustentacion calculada, debemos quitarle la resistencia inducida
        //   inducedDragCoeficient = (Cl^2) / (pi * AR * e)
        // el e, que es el factor de eficiencia no lo necesitamos
        // AR = ratio de aspecto
        //inducedDragCoeficient += 

        return lift;
    }


    /// <summary>
    /// Calculo de la fuerza de resistencia que se le esta aplicando al avion
    /// </summary>
    /// <param name="wingSurface">Area del ala sobre la que se aplica esta formula</param>
    void UpdateDrag(float wingSurface)
    {
        // Update the coefficient (Cd = Cdo + Cdi)
        //  Cdo -> drag coefficient at zero lift
        //  Cdi -> induced drag coefficcient
        float dragCoefficient = dragCoefficientAtZeroLift + inducedDragCoeficient;

        // Ahora calculamos la fuerza de resistencia
        // D = Cd * 0.5 * p * V^2 * A 
        // wingSurface tiene que ser el total de la superficie de las alas
        float drag = dragCoefficient * 0.5f * airDensity * LocalVelocity.sqrMagnitude * wingSurface;
    
        Vector3 dragForce = drag * - LocalVelocity.normalized;    
        Rigidbody.AddRelativeForce(dragForce); //drag is opposite direction of velocity
    }


    /// <summary>
    /// Calcula el giro del avion. Lo ajusta con la velicidad de giro del avion al giro que queremos hacer para que 
    ///      cuando paremos de girar, este ponga a 0 la velocidad de giro y no siga girando un poco cuando hemos soltado la palanca
    /// </summary>
    /// <param name="dt">fixed delta time</param>
    /// <param name="angularVelocity">Velocidad angular actual del avion</param>
    /// <param name="targetVelocity">Velocidad total de giro a la que queremos llegar</param>
    /// <param name="acceleration">Aceleracion de giro del avion</param>
    /// <returns></returns>
    float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        float error = targetVelocity - angularVelocity;
        float accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }

    /// <summary>
    /// Actualiza la velocidad angular del avion con el input que le hemos dado.
    /// </summary>
    /// <param name="dt">fixed delta time</param>
    void UpdateSteering(float dt)
    {
        float speed = Mathf.Max(0, LocalVelocity.z);
        if (speed > 1f)
        {
            float steeringPower = steeringCurve.Evaluate(speed); // Sirve para cambiar la velocidad de giro a distintas velocidades del avion con la AnimationCurve de steeringCurve
            Vector3 targetAV = Vector3.Scale(controlInput, turnSpeed * steeringPower); // Velocidad angular objetivo (velocidad indicada por el input dependiente de la velocidad maxima de giro)
            Vector3 av = LocalAngularVelocity * Mathf.Rad2Deg; // Velocidad angular actual del avion
            Vector3 correction = new Vector3(
                CalculateSteering(dt, av.x, targetAV.x, turnAcceleration.x * steeringPower),
                CalculateSteering(dt, av.y, targetAV.y, turnAcceleration.y * steeringPower),
                CalculateSteering(dt, av.z, targetAV.z, turnAcceleration.z * steeringPower)
            );
            Rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);    //ignore rigidbody mass
        }
    }

    
    void UpdateAngularDrag() 
    {
        var av = LocalAngularVelocity;
        var drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        Rigidbody.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    void StabilizeFlight(float dt) // Estabiliza el vuelo para que mire hacia donde se dirige
    {
        /*
        // Otro modo
        var stabilizingDrag = new Vector3(0.5f, 2.0f, 0.0f);
        Vector3 dragDirection = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity);
        //stabilization (to keep the plane facing into the direction it's moving)
        Vector3 stabilizationForces = -Vector3.Scale(dragDirection, stabilizingDrag) * GetComponent<Rigidbody>().velocity.magnitude;
        GetComponent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(stabilizationForces), transform.position - transform.forward * 10);
        GetComponent<Rigidbody>().AddForceAtPosition(-transform.TransformDirection(stabilizationForces), transform.position + transform.forward * 10);*/

        /*
        // Modo propio
        Vector3 stabilizationVelocity = new Vector3();

        stabilizationVelocity.x = Mathf.Clamp(LocalVelocity.y, -maxStabilizationTorque.x, maxStabilizationTorque.x);
        stabilizationVelocity.y = Mathf.Clamp(stabilizationYCurve.Evaluate(LocalVelocity.x), -maxStabilizationTorque.y, maxStabilizationTorque.y);
        stabilizationVelocity.z = Mathf.Clamp(LocalVelocity.z, -maxStabilizationTorque.z, maxStabilizationTorque.z);

        Rigidbody.AddRelativeTorque(stabilizationVelocity, ForceMode.Acceleration);

        Debug.Log("LocalVelocity: "+LocalVelocity + " stabilizationVelocity: " + stabilizationVelocity + "maxStabilizationTorque : " + maxStabilizationTorque);
        */

        // Modo propio
        Vector3 stabilizationVelocity = new Vector3();

        stabilizationVelocity.x = Mathf.Clamp(LocalVelocity.y, -maxStabilizationTorque.x, maxStabilizationTorque.x);
        stabilizationVelocity.y = Mathf.Clamp(stabilizationYCurve.Evaluate(LocalVelocity.x), -maxStabilizationTorque.y, maxStabilizationTorque.y);
        stabilizationVelocity.z = Mathf.Clamp(LocalVelocity.z, -maxStabilizationTorque.z, maxStabilizationTorque.z);

        Rigidbody.AddRelativeTorque(stabilizationVelocity, ForceMode.Acceleration);

        //Debug.Log("LocalVelocity: " + LocalVelocity + " stabilizationVelocity: " + stabilizationVelocity + "maxStabilizationTorque : " + maxStabilizationTorque);
    }

}
