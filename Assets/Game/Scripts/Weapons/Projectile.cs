using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float velocityMagnitude;
    private Vector3 velocityVector; 

    public int damage;

    public float aliveTime = 1;


    //Variables privadas
    Vector3 lastPosition;
    LayerMask bulletLayerMask;


    //public Gun borrarEsto;
    // Start is called before the first frame update
    void Start()
    {
        velocityVector = transform.forward * velocityMagnitude;
        lastPosition = transform.position;
        bulletLayerMask = Utils.GetPhysicsLayerMask(gameObject.layer);
    }

    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;
        lastPosition = transform.position;


        // Bullet destruction with time
        aliveTime -= deltaTime;
        if(aliveTime <= 0)
        {
            Destroy(this.gameObject);
        }


        // Bullet movement
        // Restar gravedad al componente Y
        velocityVector += Physics.gravity * deltaTime;
        this.transform.position = transform.position + velocityVector * deltaTime;

        // Check collision with raycast
        RaycastHit[] entitiesHit = Physics.RaycastAll(lastPosition, (transform.position - lastPosition).normalized, (transform.position - lastPosition).magnitude, bulletLayerMask);
        foreach(RaycastHit entityHit in entitiesHit)
        {
            if(entityHit.collider.gameObject.layer == 9)
            {
                //borrarEsto.impactos++;
                Health dest = entityHit.collider.GetComponent<Health>();
                if (dest != null)
                {
                    // Aplicar daño a entidad
                    dest.addDamage(this.damage);
                }
            }
        }
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if(other.gameObject.layer == 9)
        {
            borrarEsto.impactos++;
        }
    }*/
}
