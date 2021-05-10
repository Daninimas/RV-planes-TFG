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

        startLocalRotation = transform.localRotation;
    }


    private void OnDrawGizmos()
    {
        if (target != null)
        {
            /*
            Vector3 direction = (target.position - transform.position);
            direction.x = 0f;
            Gizmos.DrawRay(transform.position, direction);
            float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.right);
            Debug.Log(angle + transform.localEulerAngles.x);
        */
            bool onNegativeZ = false;
            Plane rotationPlane = new Plane(-transform.up, transform.position);
            // Get the closest point on the plane
            Vector3 closestPoint = rotationPlane.ClosestPointOnPlane(target.position);
            Vector3 direction;
            direction = (closestPoint - transform.position);
           
            Gizmos.DrawRay(transform.position, direction);
            if (direction.z < 0f)
            {
                onNegativeZ = true;
                direction.z = -direction.z;
            }
            // Calcular angulo
            float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.right);
            if(onNegativeZ)
                angle = -angle;
            Debug.Log("Direction: " + direction + "Angle: " + angle + " local angle: " + transform.localEulerAngles.x + " Total: " + (angle + transform.localEulerAngles.x));
            transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(angle + transform.localEulerAngles.x),-90, 90), 0f, 90f);
            //Debug.Log(-angle);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(selected)
        {
            /*
            Vector3 direction = (handInteractor.transform.position - transform.position);
            direction.x = 0f;
            float angle = Vector3.SignedAngle(transform.forward, direction.normalized, Vector3.right);
            Debug.DrawRay(transform.position, direction, Color.green);
            Debug.Log(angle);

            // Ponemos los limites a la rotacion
            Vector3 newLocalAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(angle + transform.localEulerAngles.x), angleLimitsX.min, angleLimitsX.max), startLocalRotation.eulerAngles.y, startLocalRotation.eulerAngles.z);
            transform.localEulerAngles = newLocalAngles;
            //transform.localEulerAngles = Vector3.SmoothDamp(transform.localEulerAngles, newLocalAngles, ref smoothVelocity, timeToSmooth);
            */
            Plane rotationPlane = new Plane(-transform.up, transform.position);
            // Get the closest point on the plane
            Vector3 closestPoint = rotationPlane.ClosestPointOnPlane(target.position);
            Vector3 direction = (closestPoint - transform.position);
            // Calcular angulo
            float angle = Vector3.Angle(transform.forward, direction);
            Debug.Log(-angle + transform.localEulerAngles.x);
            transform.Rotate(new Vector3(0, -angle, 0));
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
