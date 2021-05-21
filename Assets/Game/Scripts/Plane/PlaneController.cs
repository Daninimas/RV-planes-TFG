using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaneController : MonoBehaviour
{
    [Header("Plane Input Control")]
    public PlaneMixtureControl mixtureControl;
    public PlaneJoystick joystick;

    [Space]
    [Header("Plane Parts")]
    public Gun[] guns;

    /// <summary>
    /// Pone a disparar o no a todas las armas que tenga este avión
    /// </summary>
    /// <param name="shoot">Indica si las armas van a disparar o no</param>
    public void updateGunsShoot(bool shoot)
    {
        foreach(Gun gun in guns)
        {
            gun.setShoot(shoot);
        }
    }
}
