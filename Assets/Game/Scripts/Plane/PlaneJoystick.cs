using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneJoystick : MonoBehaviour
{
    public Transform joystickTransform;
    public ConfigurableJoint joystickJoint;

    private bool returnToTargetRotation = true;
    public float returningRotationVelocity = 0f;

    /// <summary>
    /// Vector normalizado entre 1 y -1 con la posicion de la palanca y sus limites
    /// </summary>
    public Vector2 joystickNormal = new Vector2();

    void Start()
    {
        if (joystickTransform == null)
        {
            joystickTransform = gameObject.transform;
        }

        if (joystickJoint == null)
        {
            joystickJoint = gameObject.GetComponent<ConfigurableJoint>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (returnToTargetRotation)
        {
            // The step size is equal to speed times frame time.
            var step = returningRotationVelocity * Time.deltaTime;

            // Rotate our transform a step closer to the target's.
            transform.rotation = Quaternion.RotateTowards(transform.rotation, joystickJoint.targetRotation, step);
        }

        normalizeJoystickValue();
    }

    /// <summary>
    /// Calcula la normal entre -1 y 1 de la palanca en los dos ejes
    /// </summary>
    private void normalizeJoystickValue()
    {
        joystickNormal.x = Utils.normalizeValues(-1, 1, joystickJoint.lowAngularXLimit.limit, joystickJoint.highAngularXLimit.limit, Utils.WrapAngle(joystickTransform.rotation.eulerAngles.x));
        joystickNormal.y = Utils.normalizeValues(-1, 1, -joystickJoint.angularZLimit.limit, joystickJoint.angularZLimit.limit, Utils.WrapAngle(joystickTransform.rotation.eulerAngles.z));
        //Debug.Log("holaaaaaaaaaaaa");
    }


    /// <summary>
    /// When the joystick is picked
    /// </summary>
    public void OnSelect()
    {
        Debug.Log("On select event");
        returnToTargetRotation = false;
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
