using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class InputManager : MonoBehaviour
{
    private ActionBasedController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();

        //************** FORMAS DE OBTENER INPUT ***************

        // Ver si un boton ha sido pulsado y esto devuelve true o false
        bool isActionPressed = controller.selectAction.action.ReadValue<bool>();
        // Cuando se pulsa un boton, se llama a una funcion
        controller.selectAction.action.performed += PulsadoBotonSelect;
    }

    private void PulsadoBotonSelect(InputAction.CallbackContext obj)
    {
        Debug.Log("Boton select pulsado");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
