using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaneJoystick : MonoBehaviour
{
    /// <summary>
    /// Vector normalizado entre 1 y -1 con la posicion de la palanca y sus limites
    /// </summary>
    [Header("Resultado de la normal")]
    public Vector2 joystickNormal = new Vector2();

    [Header("Variables")]
    [SerializeField]
    float returningRotationVelocity = 0f;
    [SerializeField]
    float movingRotationVelocity = 0f;

    [SerializeField]
    Limit2D angleLimitsX;
    [SerializeField]
    Limit2D angleLimitsY;

    [Header("Plane Controller")]
    [SerializeField]
    PlaneController planeController;

    private bool returnToTargetRotation = true;
    private Transform handInteractor;
    private Quaternion startRotation;
    private float timeCount = 0.0f; // For the returning slerp

    
    void Start()
    {
        startRotation = transform.localRotation;

        gameObject.GetComponent<XRSimpleInteractable>().selectEntered.AddListener(OnSelect);
    }


    private void LateUpdate()
    {
        if (returnToTargetRotation)
        {
            // Volver a la posicion inicial
            transform.localRotation = Quaternion.Slerp(transform.localRotation, startRotation, timeCount);
            timeCount = timeCount + Time.deltaTime;
        }
        else
        {
            timeCount = 0f;
            // Calculamos la rotacion que debe tener para mirar a la mano
            var targetRotation = Quaternion.LookRotation(handInteractor.transform.position - transform.position);
            // Rotar con velocidad hacia esta
            Quaternion objectiveRotation = Quaternion.Lerp(transform.rotation, targetRotation, movingRotationVelocity * Time.deltaTime);
            transform.rotation = objectiveRotation;

            // Ponemos los limites a la rotacion
            transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(transform.localEulerAngles.x), angleLimitsX.min, angleLimitsX.max), Mathf.Clamp(Utils.WrapAngle(transform.localEulerAngles.y), angleLimitsY.min, angleLimitsY.max), transform.localEulerAngles.z);
        }

        normalizeJoystickValue();
    }


    /// <summary>
    /// Calcula la normal entre -1 y 1 de la palanca en los dos ejes
    /// </summary>
    private void normalizeJoystickValue()
    {
        joystickNormal.x = Utils.normalizeValues(-1, 1, angleLimitsX.min, angleLimitsX.max, Utils.WrapAngle(transform.localEulerAngles.x));
        joystickNormal.y = Utils.normalizeValues(-1, 1, angleLimitsY.min, angleLimitsY.max, Utils.WrapAngle(transform.localEulerAngles.y));
        //Debug.Log(joystickNormal.ToString());
    }


    /// <summary>
    /// When the joystick is picked
    /// </summary>
    public void OnSelect(SelectEnterEventArgs args)
    {
        returnToTargetRotation = false;
        handInteractor = args.interactor.transform;
    }
    /// <summary>
    /// When the joystick is released
    /// </summary>
    public void OnDeselect()
    {
        returnToTargetRotation = true;
    }


    /// <summary>
    /// When trigger is pressed and it is been picked
    /// </summary>
    public void OnActivate()
    {
        planeController.updateGunsShoot(true);
    }
    /// <summary>
    /// When trigger is released and it is been picked
    /// </summary>
    public void OnDeactivate()
    {
        planeController.updateGunsShoot(false);
    }
}
