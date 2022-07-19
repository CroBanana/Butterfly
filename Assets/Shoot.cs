using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Shoot : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float shootForce;
    public float spread;

    public Transform spawnLocation;
    Vector3 target;

    public float timeBetweenShoots;
    public float oldTime;
    public Camera cam; 
    public LayerMask layer;

    private void Start() {
        cam = Camera.main;
        oldTime=timeBetweenShoots;
    }

    private void Update() {
        if(timeBetweenShoots>0){
            timeBetweenShoots-=Time.deltaTime;
            return;
        }
        if(Input.GetKey(KeyCode.Mouse0)){
            Fire();
        }
    }

    private void Fire(){
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,layer)){
            target = hit.point;
            Debug.Log(hit.transform.name);
        }
        else
            target = ray.GetPoint(100);

        Vector3 direction = target-spawnLocation.position;
        float x = Random.Range(-spread,spread);
        float y = Random.Range(-spread,spread);

        Vector3 spreadDirection = direction + new Vector3(x,y,0);
        Debug.DrawLine(spawnLocation.position,direction,Color.yellow,0.1f);

        GameObject spawnedBullet = Instantiate(bulletPrefab,spawnLocation.position,Quaternion.identity);
        spawnedBullet.transform.LookAt(target);
        spawnedBullet.GetComponent<Rigidbody>().AddForce(spreadDirection.normalized *shootForce, ForceMode.Impulse);
        timeBetweenShoots=oldTime;
    }
}
