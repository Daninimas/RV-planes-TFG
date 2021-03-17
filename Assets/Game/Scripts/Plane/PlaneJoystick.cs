using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneJoystick : MonoBehaviour
{
    public Transform joystickTransform;
    public ConfigurableJoint joystickJoint;

    private bool returnToTargetRotation = true;
    public float returningRotationVelocity = 0f;

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
    }

    public void OnSelect()
    {
        // When the joystick picked
        Debug.Log("On select event");
        returnToTargetRotation = false;
    }
    public void OnDeselect()
    {
        // When the joystick is release
        Debug.Log("On deselect event");
        returnToTargetRotation = true;
    }

    public void OnActivate()
    {
        // When trigger pressed and it is been picked
        Debug.Log("On activate event");
    }
    public void OnDeactivate()
    {
        // When trigger released and it is been picked
        Debug.Log("On deactivate event");
    }
}
