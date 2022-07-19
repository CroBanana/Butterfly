using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerDestructuin : MonoBehaviour
{
    public GameObject flower;
    

    private void Start() {
        StartCoroutine(DeleteThis());
    }

    IEnumerator DeleteThis(){
        yield return new WaitForSecondsRealtime(1.5f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        //Debug.Log("Tree to close");
        Destroy(flower);
    }
}
