using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    public float lifeTime;
    public float BulletDMG;
    public int smallerBulletsCount;
    List<Quaternion> palletsRotation;
    public GameObject smallerBullet;
    public float spreadAngle;
    public float force ;

    private void Start() {
        StartCoroutine(DestroyWhen());
    }

    IEnumerator DestroyWhen(){
        yield return new WaitForSecondsRealtime(lifeTime);
        Debug.Log("Bullet destroyed, time run out");
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("Enemie")){
            other.gameObject.GetComponent<Wasp>().hp -=BulletDMG;
        }
        if(other.gameObject.CompareTag("Nest")){
            other.gameObject.GetComponent<WaspNest>().hp -=BulletDMG;
        }
        //Debug.Log("Bullet collided and is now destroying");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemie") || other.gameObject.CompareTag("Nest")){
            for (int i = 0; i < smallerBulletsCount; i++)
            {
                GameObject p = Instantiate(smallerBullet, transform.position,transform.rotation);
                p.transform.LookAt(other.transform);
                p.transform.localEulerAngles = new Vector3(p.transform.localEulerAngles.x+Random.Range(-spreadAngle,spreadAngle),
                                                           p.transform.localEulerAngles.y+Random.Range(-spreadAngle,spreadAngle),
                                                           p.transform.localEulerAngles.z );
                
                p.GetComponent<Rigidbody>().AddForce(p.transform.forward*force, ForceMode.Impulse);
                p.transform.SetParent(null);
            }
            Destroy(gameObject);
        }

    }
}
