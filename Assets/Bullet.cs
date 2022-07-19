using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime;
    public float BulletDMG;


    private void Start() {
        StartCoroutine(DestroyWhen());
    }

    IEnumerator DestroyWhen(){
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Enemie")){
            other.gameObject.GetComponent<Wasp>().hp -=BulletDMG;
        }

        if(other.gameObject.CompareTag("Nest")){
            other.gameObject.GetComponent<WaspNest>().hp -=BulletDMG;
        }
        //Debug.Log("Bullet collided with "+other.gameObject.name+" and is now destroying");
        Destroy(gameObject);
    }

}
