using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    [SerializeField] 
    float health;

    public bool isDead = false;
    
    public void addDamage(float damage)
    {
        health -= damage;
        Debug.Log("Me han dado: " + gameObject.name + " con un daño de: " + damage);

        if (health <= 0)
        {
            isDead = true;
            // Destroy this object
            // Set destroyed model

            Debug.Log("Me destrullo: " + gameObject.name);
        }
    }
}
