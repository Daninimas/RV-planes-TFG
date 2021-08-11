using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;

public class ExplosiveAI : MonoBehaviour
{
    public float timer = 3f;
    public GameObject explosionEffect;
    public float explosionRadius = 5f;
    public float explsionDamage = 20f;
    public float explosionForce = 700f;


    private bool _active = false;
    private float _currentTime = 0f;
    private bool _dropped = false;
    private GameObject _plane;
    private Vector3 _lastPosition;



    private void Start()
    {
        _plane = gameObject.GetComponent<ParentConstraint>().GetSource(0).sourceTransform.gameObject;

        _lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_active)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= timer)
            {
                explode();
            }
        }
    }

    private void FixedUpdate()
    {
        // Para calcular la colision por raycast con los objetos
        if (_dropped)
        {
            // Comprobar con raycast si ha colisionado con algo, esto se hace porque muchas veces atraviesa el terreno
            RaycastHit[] entitiesHit = Physics.RaycastAll(_lastPosition, (transform.position - _lastPosition).normalized, (transform.position - _lastPosition).magnitude, Utils.GetPhysicsLayerMask(gameObject.layer));
            foreach (RaycastHit entityHit in entitiesHit)
            {
                if (entityHit.collider.gameObject.layer != _plane.layer)
                {
                    activate();

                    // Poner la bomba en la posición de impacto
                    transform.position = entityHit.point;
                }
            }
        }

        _lastPosition = transform.position;
    }

    void explode()
    {
        // Poner efecto de explosion
        GameObject expEffect = Instantiate(explosionEffect, transform.position, transform.rotation);
        expEffect.AddComponent<DeleteWithTime>().time = 2f;


        // Obtener todos los objetos en el radio de explosion para destruirlos
        Collider[] collidersToDamage = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in collidersToDamage)
        {
            // Dañar estas entidades
            Destructible dest = nearbyObject.GetComponent<Destructible>();
            if (dest != null)
            {
                // Calcular daño con la distancia al centro de la explosion
                float distance = Mathf.Abs(Vector3.Distance(transform.position, dest.transform.position));
                float normalizedDistance = Utils.normalizeValues(0, 1, 0, explosionRadius, distance);
                dest.addDamage(explsionDamage * normalizedDistance);
            }
        }

        // Volver a obtener todos los objetos en el radio de explosion para moverlos
        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            // Aplicar fuerza a estas entidades
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }

        // Destruir este objeto
        Destroy(gameObject);
    }



    public void activate()
    {
        if (!_active)
        {
            _active = true;
            _currentTime = 0;
        }
    }
    public void deactivate()
    {
        _active = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Activar cuando toque algo que no sea el avion de la IA
        if (collision.gameObject.layer != _plane.layer && _dropped)
        {
            activate();
        }
    }

    public void dropBomb()
    {
        // Delete parent Constraint
        ParentConstraint constraint = gameObject.GetComponent<ParentConstraint>();
        Destroy(gameObject.GetComponent<ParentConstraint>());

        _dropped = true;
        gameObject.layer = 12;

        // Poner a la bomba la misma velocidad que el avión
        Vector3 PlaneVelocity = _plane.GetComponent<Rigidbody>().velocity;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = PlaneVelocity;

        //Adelantar la bomba un poco para que no aparezca atrás
        //gameObject.transform.position += rb.velocity * Time.fixedDeltaTime;
    }
}
