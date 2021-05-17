using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float velocityMagnitude;
    private Vector3 velocityVector; 

    public int damage;

    public float aliveTime = 1;



    // Start is called before the first frame update
    void Start()
    {
        velocityVector = transform.forward * velocityMagnitude;
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
        Debug.Log(transform.forward);
        velocityVector += Physics.gravity * deltaTime;
        this.transform.position = transform.position + velocityVector * deltaTime;
        
    }
}
