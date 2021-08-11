using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.XR.Interaction.Toolkit;

public class Explosive : MonoBehaviour
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
    private bool _isBeingPicked = false;


    [SerializeField]
    GameObject copyBomb;
    [Header("Prediction variables")]
    [SerializeField]
    int maxPredictionSteps;
    [SerializeField]
    [Min(1)]
    int predictionFramesJump; // Cuantos frames te saltas entre los pasos de la predicción (a cuantos más, más lejos llega la predicción, pero pierde precisión)
    [SerializeField]
    GameObject predictionMarker;
    [SerializeField]
    float markerSize = 0.005f;


    private void Start()
    {
        _plane = gameObject.GetComponent<ParentConstraint>().GetSource(0).sourceTransform.gameObject;

        gameObject.GetComponent<XRGrabInteractable>().selectExited.AddListener(dropBomb);

        _lastPosition = transform.position;

        predictionMarker.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Para el marcador de donde va a caer
        if(_isBeingPicked)
            predictBombDrop();

        if (_active)
        {
            _currentTime += Time.deltaTime;
            if(_currentTime >= timer)
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
                if (entityHit.collider.gameObject.layer != 8)
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
            Health dest = nearbyObject.GetComponent<Health>();
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
        foreach(Collider nearbyObject in collidersToMove)
        {
            // Aplicar fuerza a estas entidades
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
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
        // Activar cuando toque algo que no sea el avion del jugador
        if (collision.gameObject.layer != 8 && _dropped)
        {
            activate();
        }
    }

    public void pickBomb() 
    {
        ParentConstraint constraint = gameObject.GetComponent<ParentConstraint>();
        ConstraintSource planeConstrintSource = constraint.GetSource(0);
        Instantiate(copyBomb, planeConstrintSource.sourceTransform.position + constraint.GetTranslationOffset(0), planeConstrintSource.sourceTransform.rotation);
        
        Destroy(gameObject.GetComponent<ParentConstraint>());

        _isBeingPicked = true;
    }

    public void dropBomb(SelectExitEventArgs args)
    {
        _isBeingPicked = false;
        _dropped = true;
        gameObject.layer = 12;

        // Poner a la bomba la misma velocidad que ti
        Vector3 PlaneVelocity = _plane.GetComponent<Rigidbody>().velocity;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.velocity = PlaneVelocity;

        //Adelantar la bomba un poco para que no aparezca atrás
        gameObject.transform.position = args.interactor.transform.position;
    }

    public void predictBombDrop()
    {
        Vector3 lastPosition = transform.position;
        float stepSize = Time.fixedDeltaTime * predictionFramesJump;
        Vector3 predictedBulletVelocity = _plane.GetComponent<Rigidbody>().velocity;
        LayerMask layermask = Utils.GetPhysicsLayerMask(gameObject.layer);
        bool hitSomething = false;

        for (int step = 0; step < maxPredictionSteps && !hitSomething; ++step)
        {
            //predictionMarker.gameObject.SetActive(false);

            predictedBulletVelocity += Physics.gravity * stepSize;
            Vector3 newPosition = lastPosition + predictedBulletVelocity * stepSize;

            // Calcular si ha colisionado con algo entre estos puntos
            RaycastHit[] entitiesHit = Physics.RaycastAll(lastPosition, (newPosition - lastPosition).normalized, (newPosition - lastPosition).magnitude, layermask);
            // Ponemos el marcador en el primer objeto con el que choca
            foreach (RaycastHit entityHit in entitiesHit)
            {
                if (entityHit.collider.gameObject.layer != 8)
                {
                    predictionMarker.gameObject.SetActive(true);
                    predictionMarker.transform.position = entityHit.point;
                    predictionMarker.transform.forward = entityHit.normal;
                    float scale = 0.005f * Vector3.Distance(Camera.main.transform.position, entityHit.point);
                    predictionMarker.transform.localScale = new Vector3(scale, scale, scale);
                    hitSomething = true;
                    break;
                }
            }

            lastPosition = newPosition;
        }
    }
}
