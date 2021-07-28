using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteWithTime : MonoBehaviour
{
    public float time = 2f;


    void Start()
    {
        Destroy(gameObject, time);
    }
}
