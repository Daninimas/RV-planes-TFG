using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaneMixtureControl : MonoBehaviour
{
    private enum Position{
        NONE,
        DELANTE,
        DETRAS
    }

    /// <summary>
    /// Vector normalizado entre 0 y 1 con la posicion de la palanca y sus limites
    /// </summary>
    [Header("Resultado de la normal")]
    public float mixtureControlNormal;


    [Header("Variables")]
    [SerializeField]
    float movingRotationVelocity = 0f;

    [SerializeField]
    Limit2D angleLimitsX;

    [SerializeField]
    float timeToSmooth = 0.2f;
    Vector3 smoothVelocity;

    // Variables privadas
    private bool selected;
    private Transform handInteractor;
    private Quaternion startLocalRotation;
    private Position objetiveRelativePosition = Position.NONE; // La posicion relativa donde está el objetivo que sigue la palanca
    private float objectiveAngleX;
    public Transform target;

    void Start()
    {
        gameObject.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnSelect);

        startLocalRotation = Quaternion.Euler(0f, 0f, 90f);
        transform.localEulerAngles = new Vector3(angleLimitsX.min, 0, 90f);
        objectiveAngleX = angleLimitsX.min;
    }

    
    private void OnDrawGizmos()
    {
        if (target != null)
        {
            float objectiveAngleX = 0f;
            float currentAngleX = transform.localEulerAngles.x;
            Plane rotationPlane = new Plane(-transform.up, transform.position);
            // Get the closest point on the plane
            Vector3 closestPoint = rotationPlane.ClosestPointOnPlane(target.position);
            Vector3 direction;
            direction = (closestPoint - transform.position);
           
            Gizmos.DrawRay(transform.position, direction);

            Quaternion auxRotation = transform.rotation;
            transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            Plane forwardBehindPlane = new Plane(transform.forward, transform.position);
            transform.rotation = auxRotation;
            float angle = 0;
            if (forwardBehindPlane.GetSide(closestPoint))
            {
                // Cuando esta el objeto delante de la palanca
                angle = Vector3.SignedAngle(transform.forward, direction, Vector3.right);
                angle = angle + transform.localEulerAngles.x;
                angle = angle - 360;

                transform.localEulerAngles = new Vector3(angle, 0, 90);
                Debug.Log("Delante");
            }
            else
            {
                // Cuando esta el objeto detras de la palanca
                angle = Vector3.SignedAngle(transform.forward, direction, Vector3.right);
                angle = angle + 180;
                angle = angle - transform.localEulerAngles.x;
                angle = -transform.localEulerAngles.x + 180;

                transform.localEulerAngles = new Vector3(angle, 0, 90);
                Debug.Log("Detras");
            }
            /*float angulo = -transform.localEulerAngles.x + 180;
            float angulo = transform.localEulerAngles.x - 360;*/
            Debug.Log(angle);

            /*
            //Comprobar si esta delante o detras de la palanca
            transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            Plane forwardBehindPlane = new Plane(transform.forward, transform.position);


            Debug.Log("Direction: " + direction + " Global rotation: " + transform.rotation.eulerAngles);
            transform.forward = direction.normalized;

            
            if (forwardBehindPlane.GetSide(closestPoint))
            {
                // Cuando esta el objeto delante de la palanca
                objectiveAngleX = transform.localEulerAngles.x;
                Debug.Log("Delante");
            }
            else
            {
                // Cuando esta el objeto detras de la palanca
                objectiveAngleX = -transform.localEulerAngles.x + 180f;
                Debug.Log("Detras");
            }
            objectiveAngleX = Utils.UnwrapAngle(objectiveAngleX);
            // Add velocity
            float angleDifference = objectiveAngleX - currentAngleX;
            float rotation = Mathf.Sign(angleDifference) * movingRotationVelocity * Time.deltaTime;
            if (Mathf.Abs(rotation) > Mathf.Abs(angleDifference))
                rotation = angleDifference;

            // Set value to the transform
            //transform.localEulerAngles = new Vector3(rotation + currentAngleX, 0f, 90f);
            //transform.localEulerAngles = Vector3.SmoothDamp(new Vector3(currentAngleX, 0f, 90f), new Vector3(objectiveAngleX, 0f, 90f), ref velocity, 0.3f);
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(rotation + currentAngleX), angleLimitsX.min, angleLimitsX.max), 0f, 90f);


            //objectiveAngleX = Utils.AngularClamp(objectiveAngleX, angleLimitsX.min, angleLimitsX.max);


            Debug.Log("Objective angle: " + objectiveAngleX + " Current angle: " + currentAngleX + " Rotation: " + rotation + " final angle = " + (rotation + currentAngleX));

            //transform.localEulerAngles = new Vector3(rotation, 0f, 90f);
            transform.localEulerAngles = new Vector3(objectiveAngleX, 0f, 90f); // Asigno esto antes de la velocidad para que no me de problemas con el cambio de angulos de unity
            /*rotation = Mathf.MoveTowards(currentAngleX, transform.localEulerAngles.x, 10 * Time.deltaTime);
            angleDifference = objectiveAngleX - currentAngleX;
            rotation = Mathf.Sign(angleDifference) * movingRotationVelocity * Time.deltaTime;
            if (Mathf.Abs(rotation) > Mathf.Abs(angleDifference))
                rotation = angleDifference;
            transform.localEulerAngles = new Vector3(rotation + currentAngleX, 0f, 90f); // Asigno esto antes de la velocidad para que no me de problemas con el cambio de angulos de unity
            Debug.Log("Objective angle: " + transform.localEulerAngles.x + " Current angle: " + currentAngleX + " Rotation: " + rotation + " final angle = " + (rotation + currentAngleX));*/
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(selected)
        {
            Plane rotationPlane = new Plane(-transform.up, transform.position);
            // Get the closest point on the plane
            Vector3 closestPoint = rotationPlane.ClosestPointOnPlane(handInteractor.transform.position);
            Vector3 direction;
            direction = (closestPoint - transform.position);

            //Comprobar si esta delante o detras de la palanca
            transform.localEulerAngles = new Vector3(0f, 0f, 90f);
            Plane forwardBehindPlane = new Plane(transform.forward, transform.position);

            transform.forward = direction.normalized;

            if (forwardBehindPlane.GetSide(closestPoint))
            {
                // Cuando esta el objeto delante de la palanca
                objectiveAngleX = transform.localEulerAngles.x;
                objetiveRelativePosition = Position.DELANTE;
                //Debug.Log("Delante");
            }
            else
            {
                // Cuando esta el objeto detras de la palanca
                objectiveAngleX = -transform.localEulerAngles.x + 180f;
                objetiveRelativePosition = Position.DETRAS;
                //Debug.Log("Detras");
            }
            objectiveAngleX = Utils.UnwrapAngle(objectiveAngleX);
            // Add velocity

            // Set value to the transform
            //transform.localEulerAngles = new Vector3(rotation + currentAngleX, 0f, 90f);
            //transform.localEulerAngles = Vector3.SmoothDamp(new Vector3(currentAngleX, 0f, 90f), new Vector3(objectiveAngleX, 0f, 90f), ref velocity, 0.3f);
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(rotation + currentAngleX), angleLimitsX.min, angleLimitsX.max), 0f, 90f);


            objectiveAngleX = Utils.AngularClamp(objectiveAngleX, angleLimitsX.min, angleLimitsX.max);


            //transform.localEulerAngles = new Vector3(rotation, 0f, 90f);
            transform.localEulerAngles = new Vector3(objectiveAngleX, 0f, 90f); // Asigno esto antes de la velocidad para que no me de problemas con el cambio de angulos de unity
        }
        normalizeMixtureControlValue();        
    }

    /// <summary>
    /// Calcula la normal entre 0 y 1 de la palanca de gases
    /// </summary>
    private void normalizeMixtureControlValue()
    {
        mixtureControlNormal = Utils.normalizeValues(0, 1, angleLimitsX.min, angleLimitsX.max, objectiveAngleX);
        //Debug.Log("mixtureControlNormal: " + mixtureControlNormal + " angle: " + objectiveAngleX);
    }




    /// <summary>
    /// When the joystick is picked
    /// </summary>
    public void OnSelect(SelectEnterEventArgs args)
    {
        selected = true;
        handInteractor = args.interactor.transform;
    }
    /// <summary>
    /// When the joystick is released
    /// </summary>
    public void OnDeselect()
    {
        selected = false;
    }

}
