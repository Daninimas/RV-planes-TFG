using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaneJoystick : MonoBehaviour
{
    [System.Serializable]
    public struct Limit2D
    {
        public float min;
        public float max;
    }

    [SerializeField]
    Transform planeTransform;
    public float returningRotationVelocity = 0f;
    public float movingRotationVelocity = 0f;
    [SerializeField]
    Limit2D angleLimitsX;
    [SerializeField]
    Limit2D angleLimitsZ;
    public Transform agarrador;
    public Transform parent;
    private bool returnToTargetRotation = true;
    private Vector3 offsertPositionFromPlane;
    private Transform handInteractor;
    private Quaternion startRotation;
    private float timeCount = 0.0f; // For the returning slerp
    /// <summary>
    /// Vector normalizado entre 1 y -1 con la posicion de la palanca y sus limites
    /// </summary>
    public Vector2 joystickNormal = new Vector2();

    void Start()
    {

        offsertPositionFromPlane = gameObject.transform.position - planeTransform.position;
        startRotation = transform.localRotation;

        gameObject.GetComponent<XRGrabInteractableExtended>().selectEntered.AddListener(OnSelect);
    }



    // Update is called once per frame
    /*void Update()
    {
        if (returnToTargetRotation)
        {
            // The step size is equal to speed times frame time.
            var step = returningRotationVelocity * Time.deltaTime;

            // Rotate our transform a step closer to the target's.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, joystickJoint.targetRotation, step);
        }

        normalizeJoystickValue();
    }*/

    private void LateUpdate()
    {
        if (returnToTargetRotation)
        {
            /*
            // The step size is equal to speed times frame time.
            var step = returningRotationVelocity * Time.deltaTime;
            // Rotate our transform a step closer to the target's.
            transform.localRotation = Quaternion.Lerp(transform.rotation, startRotation, step);
            */
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
        }
        else
        {
            timeCount = 0f;
            //transform.LookAt(handInteractor.transform.position, transform.right);
            var targetRotation = Quaternion.LookRotation(handInteractor.transform.position - transform.position);
            // Smoothly rotate towards the target point.
            Quaternion objectiveRotation = Quaternion.Lerp(transform.rotation, targetRotation, movingRotationVelocity * Time.deltaTime);
            transform.rotation = objectiveRotation;
            /*
            // Mi solucion a hacer un clamp de este angulo
            // Guardar la anterior rotacion
            Quaternion lastAngle = transform.rotation;
            // Rotar el objeto para ver si se pasa de los limites
            transform.rotation = objectiveRotation;
            // Calcular los angulos
            float angleX = Vector3.Angle(new Vector3(1, 0, 0), transform.forward) - 90f;
            float angleZ = Vector3.Angle(new Vector3(0, 0, 1), transform.forward) - 90f;
            // Comprobar si se sale de los limites
            if(angleX > angleLimitsX.max)
            {
                transform.Rotate(new Vector3(angleLimitsX.max-angleX, 0f, 0f));
            }
            else if(angleX < angleLimitsX.min)
            {
                transform.Rotate(new Vector3(angleX-angleLimitsX.min, 0f, 0f));
            }

            if (angleZ > angleLimitsZ.max)
            {
                new Vector3(0f, 0f, angleLimitsX.max - angleZ)
                transform.Rotate();
            }
            else if (angleZ < angleLimitsX.min)
            {
                transform.Rotate(new Vector3(0f, 0f, angleZ - angleLimitsZ.min));
            }
            */


        }
        normalizeJoystickValue();
    }
    float getPan(Transform t)
    {
        return t.localEulerAngles.z;
    }

    float getRoll(Transform originalTransform)
    {
        GameObject tempGO = new GameObject();
        Transform t = tempGO.transform;
        t.localRotation = originalTransform.localRotation;

        t.Rotate(0, 0, t.localEulerAngles.z * -1);

        GameObject.Destroy(tempGO);
        return t.localEulerAngles.x;
    }

    float getTilt(Transform originalTransform)
    {
        GameObject tempGO = new GameObject();
        Transform t = tempGO.transform;
        t.localRotation = originalTransform.localRotation;

        t.Rotate(0, 0, t.localEulerAngles.z * -1);
        t.Rotate(t.localEulerAngles.x * -1, 0, 0);

        GameObject.Destroy(tempGO);
        return t.localEulerAngles.y;
    }
    /*
    private Quaternion targetRotation(float x, float y, float z)
    {
        // twist
        float angle = x;
        Vector3 axis = Vector3.right;

        if (angle > 270)
            angle = 270;
        if (angle < 180)
            angle = 180;
        Quaternion twist = Quaternion.AngleAxis(angle, axis);

        // swing
        angle = Mathf.Sqrt(y * y + z * z);
        axis = new Vector3(0, y, z);

        Vector3 t = twist * axis;
        float a = t.y / 10;
        float b = t.z / 10;
        float l = a * a + b * b;
        if (l > 1)
            angle = angle / Mathf.Sqrt(l);

        Quaternion swing = Quaternion.AngleAxis(angle, axis);

        return twist * swing;
    }*/


    /// <summary>
    /// Calcula la normal entre -1 y 1 de la palanca en los dos ejes
    /// </summary>
    private void normalizeJoystickValue()
    {
        float angleX = Vector3.Angle(new Vector3(1, 0, 0), transform.forward) - 90f;
        float angleZ = Vector3.Angle(new Vector3(0, 0, 1), transform.forward) - 90f;
        Debug.Log("Angle X: " + angleX + "Angle Z: " + angleZ);
        joystickNormal.x = Utils.normalizeValues(-1, 1, angleLimitsX.min, angleLimitsX.max, Utils.WrapAngle(angleX));
        joystickNormal.y = Utils.normalizeValues(-1, 1, angleLimitsZ.min, angleLimitsZ.max, Utils.WrapAngle(angleZ));
        Debug.Log(joystickNormal.ToString());
    }


    /// <summary>
    /// When the joystick is picked
    /// </summary>
    public void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("On select event: " + args.interactor.gameObject.name);
        returnToTargetRotation = false;
        handInteractor = args.interactor.transform;
    }
    /// <summary>
    /// When the joystick is released
    /// </summary>
    public void OnDeselect()
    {
        Debug.Log("On deselect event");
        returnToTargetRotation = true;
    }


    /// <summary>
    /// When trigger is pressed and it is been picked
    /// </summary>
    public void OnActivate()
    {
        Debug.Log("On activate event");
    }
    /// <summary>
    /// When trigger is released and it is been picked
    /// </summary>
    public void OnDeactivate()
    {
        Debug.Log("On deactivate event");
    }
}
