using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaneController : MonoBehaviour
{
    [Space]
    [Header("Plane Parts")]
    public Gun[] guns;
    public float rateOfFire = 0f;

    [Space]
    [Header("Plane Input Data")]
    public Vector2 joystickNormal;
    public float mixtureControlNormal;
    public float rotateYawNormal = 0;


    /// <summary>
    /// Pone a disparar o no a todas las armas que tenga este avión
    /// </summary>
    /// <param name="shoot">Indica si las armas van a disparar o no</param>
    public abstract void updateGunsShoot(bool shoot);

}
