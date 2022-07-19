using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;


    float xRotation;
    float yRotation;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(Cursor.lockState == CursorLockMode.Locked){
            //Debug.Log("ITS HERE!!!!!");
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivityX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivityY;

            yRotation +=mouseX;
            xRotation -=mouseY;

            xRotation = Mathf.Clamp(xRotation, -60,60);

            transform.rotation = Quaternion.Euler(xRotation, yRotation,0);
        }

    }
}
