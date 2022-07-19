using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsticleAvoidance : MonoBehaviour
{
    private Wasp wasp;
    public List<Transform> obsticleList = new List<Transform>();

    private void Start() {
        wasp= GetComponentInParent<Wasp>();
    }

    private void OnTriggerEnter(Collider other) {
        obsticleList.Add(other.transform);
        wasp.closeToObsticle=true;
        //Debug.Log("Obsticle close  "+ obsticleList.Count);
    }

    private void OnTriggerExit(Collider other) {
        obsticleList.Remove(other.transform);
        if(obsticleList.Count==0)
            wasp.closeToObsticle=false;
    }
}
