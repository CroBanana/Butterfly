using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBone : MonoBehaviour
{
    Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        rig=GetComponent<Rigidbody>();
        transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
        if(rig.isKinematic)
            rig.isKinematic=false;
    }
}
