using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float rateOfFire;

    [SerializeField]
    float bulletVelocity;
    [SerializeField]
    float bulletAliveTime;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject bulletInstance;
    

    [Space]
    public bool shoot = false; // Se activa cuando le mandamos al arma disparar

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

                rateOfFireCounter = 0f;
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
        newBulletProj.aliveTime = bulletAliveTime;
        newBulletProj.damage = damage;
        newBulletProj.velocityMagnitude = bulletVelocity;
        //newBulletProj.GetComponent<Rigidbody>().velocity = newBulletProj.transform.forward * bulletVelocity;
    }
}
