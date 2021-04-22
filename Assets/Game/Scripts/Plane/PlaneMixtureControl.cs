using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlaneMixtureControl : MonoBehaviour
{
    public Transform mixtureControlTransform;
    public HingeJoint mixtureControlJoint;
    public bool selected;
    /// <summary>
    /// Vector normalizado entre 0 y 1 con la posicion de la palanca y sus limites
    /// </summary>
    public float mixtureControlNormal;

    void Start()
    {
        if (mixtureControlTransform == null)
        {
            mixtureControlTransform = gameObject.transform;
        }

        if (mixtureControlJoint == null)
        {
            mixtureControlJoint = gameObject.GetComponent<HingeJoint>();
        }
    }


    // Update is called once per frame
    void Update()
    {
        normalizeMixtureControlValue();        
    }

    /// <summary>
    /// Calcula la normal entre 0 y 1 de la palanca de gases
    /// </summary>
    private void normalizeMixtureControlValue()
    {
        mixtureControlNormal = Utils.normalizeValues(1, 0, mixtureControlJoint.limits.min, mixtureControlJoint.limits.max, Utils.WrapAngle(mixtureControlTransform.rotation.eulerAngles.x));
        //Debug.Log("mixtureControlNormal: " + mixtureControlNormal);
        //mixtureControlNormal = 0.7f;
    }




    /// <summary>
    /// When the joystick is picked
    /// </summary>
    public void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("On select event: " + args.interactor.gameObject.name);
    }
    /// <summary>
    /// When the joystick is released
    /// </summary>
    public void OnDeselect()
    {
        Debug.Log("On deselect event");
    }

}
