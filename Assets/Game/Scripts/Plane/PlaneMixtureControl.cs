using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaneMixtureControl : MonoBehaviour
{
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


    public Transform target;

    void Start()
    {
        gameObject.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnSelect);

        startLocalRotation = Quaternion.Euler(0f, 0f, 90f);
        transform.localEulerAngles = new Vector3(angleLimitsX.min, 0, 90f);
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

            Debug.Log("Objective angle: " + objectiveAngleX + " Current angle: " + currentAngleX + " Rotation: " + rotation + " final angle = " + (rotation + currentAngleX));
            // Set value to the transform
            //transform.localEulerAngles = new Vector3(rotation + currentAngleX, 0f, 90f);
            Vector3 velocity = new Vector3();
            //transform.localEulerAngles = Vector3.SmoothDamp(new Vector3(currentAngleX, 0f, 90f), new Vector3(objectiveAngleX, 0f, 90f), ref velocity, 0.3f);
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(rotation + currentAngleX), angleLimitsX.min, angleLimitsX.max), 0f, 90f);
            
            rotation = Mathf.MoveTowards(currentAngleX, objectiveAngleX, 10 * Time.deltaTime);

            //transform.localEulerAngles = new Vector3(rotation, 0f, 90f);
            transform.localEulerAngles = new Vector3(Utils.AngularClamp(objectiveAngleX, angleLimitsX.min, angleLimitsX.max), 0, 90);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(selected)
        {
            float objectiveAngleX = 0f;
            //float currentAngleX = transform.localEulerAngles.x;
            Plane rotationPlane = new Plane(-transform.up, transform.position);
            // Get the closest point on the plane
            Vector3 closestPoint = rotationPlane.ClosestPointOnPlane(handInteractor.transform.position);
            Vector3 direction;
            direction = (closestPoint - transform.position);

            /*if (direction.z < 0f)
            {
                onNegativeZ = true;
                direction.z = -direction.z;
            }
            // Calcular angulo
            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.right);
            if(onNegativeZ)
                angle = -angle;
            Debug.Log("Direction: " + direction + "Angle: " + angle + " local angle: " + transform.localEulerAngles.x + " Total: " + (angle + transform.localEulerAngles.x));
            // Add velocity
            float rotation = Mathf.Sign(angle) * 10 * Time.deltaTime;
            if (rotation > Mathf.Abs(angle))
                rotation = angle;
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(angle + transform.localEulerAngles.x), -90, 90), 0f, 90f);
            transform.localEulerAngles = new Vector3(angle + transform.localEulerAngles.x, 0f, 90f);*/

            //Comprobar si esta delante o detras de la palanca
            transform.localEulerAngles = startLocalRotation.eulerAngles;
            Plane forwardBehindPlane = new Plane(transform.forward, transform.position);


            transform.forward = direction.normalized;

            if (forwardBehindPlane.GetSide(closestPoint))
            {
                // Cuando esta el objeto delante de la palanca
                objectiveAngleX = transform.localEulerAngles.x;
            }
            else
            {
                // Cuando esta el objeto detras de la palanca
                objectiveAngleX = -transform.localEulerAngles.x + 180f;
            }

            /*// Add velocity
            float angleDifference = objectiveAngleX - currentAngleX;
            float rotation = Mathf.Sign(angleDifference) * movingRotationVelocity * Time.deltaTime;
            if (Mathf.Abs(rotation) > Mathf.Abs(angleDifference))
                rotation = angleDifference;*/

            // Set value to the transform
            //transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(rotation + currentAngleX), angleLimitsX.min, angleLimitsX.max), startLocalRotation.eulerAngles.y, startLocalRotation.eulerAngles.z);
            transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(objectiveAngleX), angleLimitsX.min, angleLimitsX.max), startLocalRotation.eulerAngles.y, startLocalRotation.eulerAngles.z);

        }
        normalizeMixtureControlValue();        
    }

    /// <summary>
    /// Calcula la normal entre 0 y 1 de la palanca de gases
    /// </summary>
    private void normalizeMixtureControlValue()
    {
        mixtureControlNormal = Utils.normalizeValues(1, 0, angleLimitsX.min, angleLimitsX.max, Utils.WrapAngle(transform.rotation.eulerAngles.x));
        //Debug.Log("mixtureControlNormal: " + mixtureControlNormal);
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
