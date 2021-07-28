using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{

    [SerializeField] 
    float health;
    
    public void addDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            // Destroy this object
            // Set destroyed model
        }
    }
}
