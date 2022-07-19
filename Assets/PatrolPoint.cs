using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint : MonoBehaviour
{
    public Wasp wasp;
    RaycastHit hit;
    public LayerMask layerMask;
    private void Start() {
        wasp = GetComponentInParent<Wasp>();
        transform.SetParent(null);
        wasp.CalculateNewPatrolPoint();
    }

    private void Update() {
        Debug.DrawRay(transform.position,transform.up*100, Color.white);
        if(transform.hasChanged){
            if(Physics.Raycast(transform.position, 
                    -transform.up,
                    out hit, 
                    100,
                    layerMask))
            {
                //Debug.Log("HitGround");
            }else
                wasp.CalculateNewPatrolPoint();


        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer ==7 ||other.gameObject.layer ==8)
            wasp.CalculateNewPatrolPoint();
    }
}
