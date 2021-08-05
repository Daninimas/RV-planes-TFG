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
    public float rateOfFire = 0f;

    uint _gunToShoot = 0;
    float _rateOfFireCounter = 0f;
    bool _shootGuns = false;

    /// <summary>
    /// Pone a disparar o no a todas las armas que tenga este avión
    /// </summary>
    /// <param name="shoot">Indica si las armas van a disparar o no</param>
    public void updateGunsShoot(bool shoot)
    {
        _shootGuns = shoot;
    }

    void Update()
    {
        if (guns.Length > 0)
        {
            _rateOfFireCounter += Time.deltaTime;

            if (_rateOfFireCounter >= rateOfFire && _shootGuns)
            {
                guns[_gunToShoot].createShoot();

                _rateOfFireCounter = 0f;

                // Set next gun to shoot
                ++_gunToShoot;
                if (_gunToShoot >= guns.Length)
                    _gunToShoot = 0;
            }
        }
    }
}
