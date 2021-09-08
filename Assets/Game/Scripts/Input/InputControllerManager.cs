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

    // Input de acciones propias que vamos a usar
    [Header("InputXR Actions")]
    [SerializeField]
    InputActionReference triggerAnimActionRef = null;
    [SerializeField]
    InputActionReference gripAnimActionRef = null;
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();

        //************** FORMAS DE OBTENER INPUT ***************

        // Ver si un boton ha sido pulsado y esto devuelve true o false
        bool isActionPressed = controller.selectAction.action.ReadValue<bool>();

        /*
        // Mejor forma de obtener input con inputXR basado en acciones de esta forma de suscribes cuando se activa y cuando se desactiva
        // Cuando se pulsa un boton, se llama a una funcion
        RotateYawActionRef.action.started += UpdateYawNormal;
        RotateYawActionRef.action.canceled += UpdateYawNormal;
        */


        // Para la animacion de las manos
        spawnedHandModel = Instantiate(handModelPrefab, transform);

        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }
    /*
    private void OnDestroy()
    {
        // Desuscribirse de las acciones cuando se destruya el objeto
        RotateYawActionRef.action.started  -= UpdateYawNormal;
        RotateYawActionRef.action.canceled -= UpdateYawNormal;
    }
    */
    // Update is called once per frame
    void Update()
    {
        UpdateHandAnimation();

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    void UpdateHandAnimation()
    {
        float triggerValue = triggerAnimActionRef.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimActionRef.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }

    /*
    private void UpdateYawNormal(InputAction.CallbackContext context)
    {
        rotateYawNormalValue = context.ReadValue<int>();
        Debug.Log(gameObject.name + " YawValue: " + rotateYawNormalValue);
    }*/
}
