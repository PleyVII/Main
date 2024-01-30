using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SphereColliderOnEnemy : MonoBehaviour {
    public UnityEvent<Collider> onTriggerEnter;
    public UnityEvent<Collider> onTriggerExit;
    //public UnityEvent<Collider> onTriggerStay;
    void OnTriggerEnter(Collider col) {
        if (onTriggerEnter != null) onTriggerEnter.Invoke(col);
    }
    // void OnTriggerStay(Collider col) 
    // {
    //     if(onTriggerStay != null) onTriggerStay.Invoke(col);
    // }
    void OnTriggerExit(Collider col) {
        if (onTriggerExit != null) onTriggerExit.Invoke(col);
    }
}