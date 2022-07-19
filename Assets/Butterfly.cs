using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations.Rigging;
using TMPro;

public class Butterfly : MonoBehaviour
{
    [Header("Thirst and parameters"),Space(20)]
    public float thirstMax;
    public float thirstCurrent;
    public float thirstDownSpeed;
    public float oldThirstDownSpeed;
    public Image thirstMeter;
    public TextMeshProUGUI speedText;
    public bool onFlower;


    [Header("When Dead"),Space(20)]
    public bool dead;
    private bool setDeadObjects;
    //stuff to enable when dead
    public List<Collider> collidersChildren;
    private List<Rigidbody> rigidbodiesChildren = new List<Rigidbody>();

    [Header("general"),Space(20)]

    public Transform enemieMovmentPoint;

    //////////////stuff to disable when dead
    //animator
    //boxColliders
    public Collider[] collidersOnParent;
    //rigbuilder
    public RigBuilder rigBuilder;

    //rigidbody 
    Rigidbody rig;
    public bool gameSarted;
    public bool playPressed;



    [Header("Movement "),Space(20)]
    public float acceleration;
    public float speedLimit;
    public float xDirection, zDirection, yDirection;
    Vector3 moveDirection;
    public Transform cameraObject;
    public Transform mainCameraObject;
    public float rotationSpeed;
    public bool canLand;
    public Animator anim;


    [Header("Dodge"),Space(20)]
    public float dodgeTimer;
    private float dodgeTimerOld;
    public float dodgeStrenght;


    
    //////////raycasting for legs and distance to ground;
    RaycastHit hit;
    [Header("Dont worry about this"),Space(20)]
    public float raycastDistance;
    public LayerMask layerMask;

    public Transform frontRight_RaycastPosition;
    public Transform frontRight_IK;

    public Transform frontLeft_RaycastPosition;
    public Transform frontLeft_IK;

    public Transform backRight_RaycastPosition;
    public Transform backRight_IK;

    public Transform backLeft_RaycastPosition;
    public Transform backLeft__IK;

    private List<Transform> legRaycastPositions = new List<Transform>();
    private List<Transform> iks = new List<Transform>();

    private void Awake() {
        Physics.gravity = new Vector3(0,-1F,0);
        Collider[] collidersChildren2 = gameObject.GetComponentsInChildren<Collider>();
        Rigidbody[] rigidbodiesChildren2 = gameObject.GetComponentsInChildren<Rigidbody>();
        collidersOnParent=GetComponents<Collider>();



        foreach(Collider c in collidersChildren2)
            collidersChildren.Add(c);

        foreach(Collider c in collidersOnParent)
            collidersChildren.Remove(c);

        foreach(Collider c in collidersChildren)
            c.enabled=false;

        foreach(Rigidbody body in rigidbodiesChildren2)
            rigidbodiesChildren.Add(body);
        
        rigidbodiesChildren.Remove(GetComponent<Rigidbody>());

        foreach(Rigidbody body in rigidbodiesChildren){
            body.detectCollisions=false;
            body.isKinematic = true;
        }
    }

    private void Start() {
        rig = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        legRaycastPositions.Add(frontRight_RaycastPosition);
        legRaycastPositions.Add(frontLeft_RaycastPosition);
        legRaycastPositions.Add(backRight_RaycastPosition);
        legRaycastPositions.Add(backLeft_RaycastPosition);
        iks.Add(frontRight_IK);
        iks.Add(frontLeft_IK);
        iks.Add(backRight_IK);
        iks.Add(backLeft__IK);

        thirstCurrent = thirstMax;
        oldThirstDownSpeed =thirstDownSpeed;
        dodgeTimerOld=dodgeTimer;
        dodgeTimer=-1;


        //Physics.gravity = new Vector3(0,-0.01F,0);
        
    }

    private void Update() {
        if(dead){
            if(!setDeadObjects)
                WhenDead();
            return;
        }
        else if(playPressed){
            UpdateThirst();
        }

        //ako igra nije krenula
        if(!gameSarted){
            return;
        }


        if(thirstCurrent<=0)
            dead=true;
        
        xDirection = Input.GetAxis("Horizontal");
        zDirection = Input.GetAxis("Vertical");
        yDirection = Input.GetAxis("Up_Down");

        //SpeedControl();
        //RotateToFaceCamera();
        ChangeAnimation();
    }


    private void FixedUpdate() {
        if(dead){
            return;
        }

        if(!gameSarted){
            return;
        }

        if(canLand && xDirection==0 && zDirection==0){
            GroundDistanceCheck();
            anim.SetBool("Landed",canLand);
        }
        if(dodgeTimer<0){
            if(Input.GetKeyDown(KeyCode.Mouse1)){
                Dodge();
            }
        }
        Move();
        SpeedControl();
        RotateToFaceCamera();
    }



    void Dodge(){
        Debug.Log("Dodge");
        Vector3 velocity = new Vector3(rig.velocity.x, rig.velocity.y, rig.velocity.z);
        rig.AddForce(velocity.normalized*dodgeStrenght,ForceMode.Impulse);
        dodgeTimer=dodgeTimerOld;
        StartCoroutine(DodgeTimerDEC());
        return;
    }

    IEnumerator DodgeTimerDEC(){
        yield return new WaitForSeconds(dodgeTimer);
        dodgeTimer=-1;
    }

    void Move(){
        Debug.DrawRay(transform.position,transform.TransformDirection( Vector3.down) * raycastDistance, Color.red);
        moveDirection = mainCameraObject.forward * zDirection + mainCameraObject.right*xDirection+ mainCameraObject.up*yDirection;
        
        rig.AddForce(moveDirection.normalized * acceleration, ForceMode.Force);
    }

    void SpeedControl(){
        Vector3 velocity = new Vector3(rig.velocity.x, rig.velocity.y, rig.velocity.z);

        if(velocity.magnitude >0){
            if(velocity.magnitude > speedLimit){
                if(Input.GetKey(KeyCode.LeftShift)){
                    Vector3 limVel = velocity.normalized * speedLimit*2;
                    rig.velocity = new Vector3(limVel.x, limVel.y, limVel.z);
                    anim.speed=2;
                }
                else
                {
                    Vector3 limVel = velocity.normalized * speedLimit;
                    rig.velocity = new Vector3(limVel.x, limVel.y, limVel.z);
                    anim.speed=1;
                }
            }

        }
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log(other.gameObject.name);
    }


    void ChangeAnimation(){
        //Debug.Log(anim.GetBool("Falling"));
        //Debug.Log(yDirection);
        speedText.text = rig.velocity.magnitude.ToString();
        if(Input.GetKey(KeyCode.LeftControl))
            if(!anim.GetBool("Falling"))
                anim.SetBool("Falling",true);
        if(Input.GetKeyUp(KeyCode.LeftControl))
            anim.SetBool("Falling", false);
    }
    
    void RotateToFaceCamera(){
        if(xDirection !=0 || zDirection !=0 ||yDirection !=0){
            transform.rotation = Quaternion.RotateTowards(transform.rotation, cameraObject.rotation, rotationSpeed * Time.deltaTime);
        }else
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                                        Quaternion.Euler(0,transform.eulerAngles.y, transform.eulerAngles.z),
                                                        rotationSpeed * Time.deltaTime);
    }


    private void GroundDistanceCheck(){
        
        if(Physics.Raycast(transform.position, 
                        transform.TransformDirection( Vector3.down),
                        out hit, 
                        raycastDistance,
                        layerMask))
            {
                float distance =Vector3.Distance(transform.position,hit.point);
                //Debug.Log("GroundCheck");
                //Debug.Log(Vector3.Distance(transform.position,hit.point));
                for (int i = 0; i < legRaycastPositions.Count; i++)
                {
                    if(Physics.Raycast(legRaycastPositions[i].position, 
                                    legRaycastPositions[i].TransformDirection( Vector3.down),
                                    out hit, 
                                    raycastDistance,
                                    layerMask))
                    {
                        iks[i].position = hit.point;
                    }
                }
                if(distance>0.08f){
                    transform.Translate(Vector3.down*Time.deltaTime, Space.World);
                    //Debug.Log(Vector3.Distance(transform.position,hit.point));
                }
            }
    }

    private void UpdateThirst(){
        thirstCurrent+=-Time.deltaTime*thirstDownSpeed;
        thirstMeter.fillAmount =thirstCurrent/100;
        if(thirstCurrent>thirstMax )
            thirstDownSpeed =oldThirstDownSpeed;
    }

    private void WhenDead(){
        anim.enabled=false;
        rig.detectCollisions = false;
        rig.isKinematic = true;
        foreach(Collider c in collidersOnParent)
            c.enabled = false;

        foreach(Collider c in collidersChildren)
            c.enabled = true;

        foreach(Rigidbody body in rigidbodiesChildren){
            body.detectCollisions=true;
            body.isKinematic=false;
        }
        setDeadObjects = true;
        Debug.Log("Is dead");
    }



    private void OnTriggerEnter(Collider other) {
        if(dead)
            return;
        //Debug.Log("He can land");
        canLand= true;

    }
    private void OnTriggerExit(Collider other) {
        if(dead)
            return;
        //Debug.Log("He has flawn away");
        canLand=false;
        anim.SetBool("Landed",false);
    }
}
