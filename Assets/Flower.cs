using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public Transform player;
    public Butterfly butterfly;
    public float refilSpeed;
    public bool canRefil = true;
    public bool playerLandedAndIsRefiling=false;
    public GameObject ps;
    public float refilTime;


    [Header("Setting bullet stuff")]
    public GameObject bulletPrefab;
    public Shoot butterflyShoot;
    public float timeBetweenShoots;
    public bool setCurentBullet;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        butterfly = player.GetComponent<Butterfly>();
        butterflyShoot = player.GetComponent<Shoot>();
        ps.SetActive(true);
        
    }

    private void OnTriggerStay(Collider other) {
        if(!canRefil)
            return;

        if(other.gameObject ==player.gameObject){
            if (this.butterfly.anim.GetCurrentAnimatorStateInfo(0).IsName("IdleStand"))
            {
                playerLandedAndIsRefiling = true;
                RefilThirst();
                SetBullet();
            }
            if(butterfly.xDirection+butterfly.yDirection+butterfly.zDirection !=0 && playerLandedAndIsRefiling){
                StartCoroutine(TimeToRefil());
                playerLandedAndIsRefiling =false;
            }
        }

    }
    public void SetBullet(){
        if(!setCurentBullet){
            butterflyShoot.bulletPrefab = bulletPrefab;
            butterflyShoot.oldTime = timeBetweenShoots;
            setCurentBullet=true;
        }
    }

    public void RefilThirst(){
        //Debug.Log("Refil started");
        if(butterfly.thirstMax < butterfly.thirstCurrent){
            StartCoroutine(TimeToRefil());
            butterfly.thirstDownSpeed = butterfly.oldThirstDownSpeed;
            return;
        }
        butterfly.thirstDownSpeed = -refilSpeed;
        
    }

    IEnumerator TimeToRefil(){
        Debug.Log("Started corutine");
        butterfly.thirstDownSpeed = butterfly.oldThirstDownSpeed;
        canRefil = false;
        ps.SetActive(false);
        yield return new WaitForSecondsRealtime(refilTime);
        canRefil = true;
        setCurentBullet = false;
        ps.SetActive(true);
        Debug.Log("Can refil again");
    }

}
