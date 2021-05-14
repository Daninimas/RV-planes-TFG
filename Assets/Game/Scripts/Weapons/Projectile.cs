using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float velocityMagnitude;
    private Vector3 velocityVector; 

    public int damage;

    public float aliveTime = 1;


    // ******** Esto no va aqui, cambiarlo de sitio
    float gravity = 9.8f; // Esto pertenece al mundo


    // Start is called before the first frame update
    void Start()
    {
        velocityVector *= velocityMagnitude;
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Bullet destruction with time
        aliveTime -= deltaTime;
        if(aliveTime <= 0)
        {
            Destroy(this.gameObject);
        }


        // Bullet movement
        // Restar gravedad al componente Y
        this.transform.position += transform.forward * (deltaTime * velocityMagnitude);
    }
}
