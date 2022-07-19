using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFolowPlayer : MonoBehaviour
{
    public Transform targetPosition;
    public Transform deadPosition;
    public float smoothSpeed = 0.02f;
    public Butterfly butterfly;

    private void Start() {
        butterfly = targetPosition.GetComponent<Butterfly>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(butterfly.dead)
            targetPosition = deadPosition;


        float smoothToClose =0;
        if (butterfly.yDirection != 0)
            smoothToClose = 0.08f;
        
        if( Vector3.Distance(targetPosition.position,Camera.main.transform.position)< 0.8f){
            smoothToClose = 0.1f;
        }
        //Debug.Log(Vector3.Distance(targetPosition.position,Camera.main.transform.position));
        transform.position = Vector3.Lerp(transform.position,
                                        targetPosition.position, 
                                        smoothSpeed+smoothToClose);
    }

    
}
