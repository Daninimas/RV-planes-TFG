using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMixtureControl : MonoBehaviour
{
    public Transform mixtureControlTransform;
    public HingeJoint mixtureControlJoint;

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
    }
}
