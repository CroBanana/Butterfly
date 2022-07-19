using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class WaspNest : MonoBehaviour
{
    [Header("HP and stuff"),Space(20)]
    public float hp;
    public float hpMax;
    public Image hpImage;
    public GameObject hpPosition;
    public Camera cam;
    public  float reduceSpeed =2;
    public float targetHP=1;
    Color tempColor;
    public float buffAfterDeath;



    [Header("Spawner"),Space(20)]
    public List<GameObject> waspsInNest = new List<GameObject>();
    public int numberOfwaspsMax;
    public float spawnRate;
    public GameObject wasp;




    private void Start() {
        tempColor = hpImage.color;
        tempColor.a = 0;
        hpImage.color = tempColor;
        hpMax = hp;
        cam = Camera.main;
    }

    public void StartCorutineSpawning(){
        StartCoroutine(StartSpawning());
        StartCoroutine(CheckHP());
    }

    private void Update() {
        UpdateHPBar();
    }

    public void UpdateHPBar(){
        if(hp<0)
            gameObject.SetActive(false);
        
        targetHP = hp/hpMax;
        if(hpImage.fillAmount !=targetHP){
            tempColor.a=1;
            //Debug.Log("image fill : "+hpImage.fillAmount+"   targethp  :"+targetHP);

            hpImage.color =tempColor;
            hpImage.fillAmount = Mathf.MoveTowards(hpImage.fillAmount, targetHP,reduceSpeed* Time.deltaTime);
            hpPosition.transform.rotation = Quaternion.LookRotation(hpPosition.transform.position-cam.transform.position);
        }
        else if(tempColor.a !=0){
            //Debug.Log("Here");
            tempColor.a =Mathf.MoveTowards(tempColor.a,0, Time.deltaTime);
            hpImage.color =tempColor;
        }

    }


    public IEnumerator StartSpawning(){
        //Debug.Log("Started Spawning");
        while(waspsInNest.Count<=numberOfwaspsMax){
            GameObject spawnedWasp = Instantiate(wasp,transform.position, Quaternion.identity);
            spawnedWasp.GetComponent<Wasp>().nest =this;
            waspsInNest.Add(spawnedWasp);
            //Debug.Log("Enemie Spawned");
            yield return new WaitForSeconds(spawnRate);
        }
        yield return new WaitForSeconds(spawnRate);
        StartCoroutine(StartSpawning());
    }

    public IEnumerator CheckHP(){
        yield return new WaitUntil (()=> hp<0);
        foreach (var wasp in waspsInNest)
        {
            wasp.GetComponent<Wasp>().moveSpeed = buffAfterDeath;
            wasp.transform.localScale = new Vector3(2,2,2);
        }
        gameObject.SetActive(false);
    }

    public Vector3 NextPatrolPointLocation(){
        return transform.position+ Random.insideUnitSphere*50;
    }
}
