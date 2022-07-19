using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaspSpawnZone : MonoBehaviour
{
    public List<Transform> nests = new List<Transform>();
    public List<Transform> activeNests;
    public int activeNestsNumber;

    private void Start() {
        StartCoroutine(StartDeacivation());
    }
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other) {
        nests.Add(other.transform);
        //Debug.Log(other.transform.localPosition);
        //Instantiate(waspNest, other.transform.localPosition+new Vector3(1,3,0),Quaternion.identity);
    }

    IEnumerator StartDeacivation(){
        yield return new WaitForSeconds(0.5f);
        foreach(Transform nest in nests)
            nest.gameObject.SetActive(false);
        Destroy(GetComponent<Rigidbody>());
        GetComponent<BoxCollider>().enabled = false;
    }

    IEnumerator StartActivateSpawners(){
        yield return new WaitForSeconds(1);
        Debug.Log("Spawning started");

        while(activeNestsNumber> activeNests.Count){
            int rand = Random.Range(0,nests.Count);
            //Debug.Log(rand);
            activeNests.Add(nests[rand]);
            nests[rand].gameObject.SetActive(true);
            nests[rand].gameObject.GetComponent<WaspNest>().StartCorutineSpawning();
            nests.RemoveAt(rand);
        }


    }

    IEnumerator CheckIfAllNestsActive(){
        yield return new WaitForSeconds(15);
        foreach (var nest in activeNests)
            if(!nest.gameObject.activeInHierarchy){
                activeNests.Remove(nest);
                nest.GetComponent<WaspNest>().hp = 100;
            }

        if(activeNestsNumber>activeNests.Count)
            StartCoroutine(StartActivateSpawners());

        StartCoroutine(CheckIfAllNestsActive());

    }
    
    public void StartSpawningNests(){
        StartCoroutine(StartActivateSpawners());
    }
}
