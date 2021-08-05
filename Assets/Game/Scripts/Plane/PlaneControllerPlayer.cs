using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlaneControllerPlayer : PlaneController
{
    [Header("Plane Input Control")]
    [SerializeField]
    private PlaneMixtureControl mixtureControl;
    [SerializeField]
    private PlaneJoystick joystick;
    [SerializeField]
    private InputActionReference RotateYawActionRef = null;


    uint _gunToShoot = 0;
    float _rateOfFireCounter = 0f;
    bool _shootGuns = false;


    void Start()
    {
        RotateYawActionRef.action.Enable();
    }


    /// <summary>
    /// Pone a disparar o no a todas las armas que tenga este avión
    /// </summary>
    /// <param name="shoot">Indica si las armas van a disparar o no</param>
    public override void updateGunsShoot(bool shoot)
    {
        _shootGuns = shoot;
    }


    void Update()
    {
        updatePlaneInputData();

        updateGuns();
        
    }


    void updateGuns()
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

    void updatePlaneInputData()
    {
        joystickNormal = joystick.joystickNormal;
        mixtureControlNormal = mixtureControl.mixtureControlNormal;
        rotateYawNormal = RotateYawActionRef.action.ReadValue<float>();
    }
}
