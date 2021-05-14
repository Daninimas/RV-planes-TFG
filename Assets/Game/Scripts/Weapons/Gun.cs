using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float rateOfFire;

    public bool shoot = false; // Se activa cuando le mandamos al arma disparar

    [SerializeField]
    GameObject bulletInstance;

    // Variables privadas
    float rateOfFireCounter;

    // Update is called once per frame
    void Update()
    {
        rateOfFireCounter += Time.deltaTime;

        if(rateOfFireCounter >= rateOfFire)
        {
            if (shoot)
            {
                createShoot();
            }
        }
    }

    private void createShoot()
    {
        GameObject newBullet = GameObject.Instantiate(bulletInstance);
        Projectile newBulletProj = newBullet.GetComponent<Projectile>();

        // Poner la posicion y direccion
        newBullet.transform.position = this.transform.position;
        newBullet.transform.forward = this.transform.forward;

        // Poner los datos de bullet
        newBulletProj.aliveTime = 2f;
        newBulletProj.damage = 1;
        newBulletProj.velocityMagnitude = 100f;
    }
}
