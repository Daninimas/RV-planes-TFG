using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour
{
    [SerializeField]
    Gun[] guns;
    [SerializeField]
    float rateOfFire = 0f;
    [Min(0)]
    [SerializeField]
    float range;
    [SerializeField]
    Limit2D aimingLimitsX;
    [SerializeField]
    Limit2D aimingLimitsY;
    [SerializeField]
    [Min(0)]
    float rotationVelocity;
    public GameObject target;



    // Variables privadas
    uint _gunToShoot = 0;
    float _rateOfFireCounter = 0f;
    bool _shootGuns = false;


    void Update()
    {
        if (guns.Length > 0)
        {
            if (target != null)
            {
                if (checkTargetInsideRange())
                {
                    aimObjective();
                }
                else
                {
                    _shootGuns = false;
                }
            }
            else
            {
                _shootGuns = false;
            }

            updateGuns();
        }
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


    bool checkTargetInsideRange()
    {
        if(Vector3.Distance(this.transform.position, target.transform.position) <= range)
        {
            return true;
        }

        return false;
    }

    void aimObjective()
    {
        Vector3 targetPosition = getPredictedShootPosition();

        // Calculamos la rotacion que debe tener para mirar a la mano
        var targetRotation = Quaternion.LookRotation(targetPosition - this.transform.position);
        // Rotar con velocidad hacia esta
        Quaternion objectiveRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationVelocity * Time.deltaTime);
        transform.rotation = objectiveRotation;

        Vector3 previousLocalEuler = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);

        // Ponemos los limites a la rotacion
        transform.localEulerAngles = new Vector3(Mathf.Clamp(Utils.WrapAngle(transform.localEulerAngles.x), aimingLimitsX.min, aimingLimitsX.max), Mathf.Clamp(Utils.WrapAngle(transform.localEulerAngles.y), aimingLimitsY.min, aimingLimitsY.max), 0f);
    
        if(transform.localEulerAngles != previousLocalEuler)
        {
            // Esto es cuando no puede girar porque está fuera del ángulo de visión
            _shootGuns = false;
        }
        else
        {
            _shootGuns = true;
        }
    }

    /// <summary>
    /// Primero calcula la distancia entre el objetivo y el disparador.
    /// Luego calcula cuanto tiempo tardará la bala en llegar a ese punto
    /// Finalmente obtiene que posición tendrá el objetivo pasado ese tiempo
    /// </summary>
    /// <returns>Posicion predecida</returns>
    Vector3 getPredictedShootPosition()
    {
        /*Distance = Length(Target_Position - Firing_Position)
         Time = Distance / Bullet_Speed
         Predicted_Position = Target_Position + (Target_Velocity * Time)*/

        Vector3 predictedPosition = target.transform.position;
        Rigidbody targetRB = target.GetComponent<Rigidbody>();

        if (targetRB != null)
        {
            Vector3 targetVelocity = targetRB.velocity;
            float distance = Vector3.Distance(target.transform.position, this.transform.position);
            float travelTime = distance / guns[0].bulletVelocity;

            predictedPosition = target.transform.position + (targetVelocity * travelTime);
        }

        return predictedPosition;
    }
}
