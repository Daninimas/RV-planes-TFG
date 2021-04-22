using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputControllerManager : MonoBehaviour
{
    private ActionBasedController controller;

    public GameObject handModelPrefab;
    private GameObject spawnedHandModel; // The hand gameObject when it is spawned

    // For the hand animation
    private Animator handAnimator;

    public Vector2 joystickNormal = new Vector2();

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();

        //************** FORMAS DE OBTENER INPUT ***************

        // Ver si un boton ha sido pulsado y esto devuelve true o false
        bool isActionPressed = controller.selectAction.action.ReadValue<bool>();
        // Cuando se pulsa un boton, se llama a una funcion
        controller.selectAction.action.performed += PulsadoBotonSelect;

        spawnedHandModel = Instantiate(handModelPrefab, transform);

        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }

    private void PulsadoBotonSelect(InputAction.CallbackContext obj)
    {
        //Debug.Log("Boton select pulsado: " + obj.ReadValue<float>());
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();
        UpdateJoystickInput();
    }

    void UpdateHandAnimation()
    {
        float triggerValue = controller.activateAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float actionValue = controller.selectAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", actionValue);
    }

    void UpdateJoystickInput()
    {
        String rotateValue = controller.rotationAction.action.ToString();

        String positionValue = controller.positionAction.action.ToString();
        //Debug.Log("rotateValue: " + rotateValue.ToString()+ " positionValue: " + positionValue.ToString());
    }
}
