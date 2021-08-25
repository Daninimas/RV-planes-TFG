using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public bool collided = false;

    private void OnTriggerEnter(Collider other)
    {
        collided = true;
    }
}
