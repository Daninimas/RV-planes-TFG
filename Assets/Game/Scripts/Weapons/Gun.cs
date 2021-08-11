using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float bulletVelocity;
    [SerializeField]
    float bulletAliveTime;
    [SerializeField]
    int damage;
    [SerializeField]
    GameObject bulletInstance;


    public void createShoot()
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
        //newBulletProj.borrarEsto = this;
        //newBulletProj.GetComponent<Rigidbody>().velocity = newBulletProj.transform.forward * bulletVelocity;

        //disparosRealizados++;
    }
}
