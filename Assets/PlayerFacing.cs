using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    [SerializeField] Transform playerHead;
    public bool onMovingObject = true;
    // Update is called once per frame
    void LateUpdate()
    {
        if(onMovingObject)
            this.transform.LookAt(playerHead);
    }
}
