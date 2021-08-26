using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    [SerializeField]
    GameObject explosionEffect;

    Health tankHealth;

    private void Start()
    {
        tankHealth = gameObject.GetComponent<Health>(); ;
    }

    private void Update()
    {
        manageHealth();
    }

    void manageHealth()
    {

        if (tankHealth != null)
        {
            if (tankHealth.isDead)
            {
                GameObject expEffect = Instantiate(explosionEffect, transform.position, transform.rotation);
                expEffect.AddComponent<DeleteWithTime>().time = 2f;

                // Destruir este objeto
                Destroy(gameObject);
            }
        }
    }
}
