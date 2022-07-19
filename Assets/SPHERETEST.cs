using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHERETEST : MonoBehaviour
{
    GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.localScale = new Vector3(0.03f,0.03f,0.03f);
        StartCoroutine(Spawn());
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator Spawn(){
        while(true){
            Instantiate(cube,transform.position+ Random.insideUnitSphere*20,Quaternion.identity,transform);
            yield return new WaitForSeconds(3);

        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(transform.position, 100f);
    }

}
