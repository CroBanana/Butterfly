using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
public class Wasp : MonoBehaviour
{
    public Rigidbody rig;
    //rigidbody options
    private float mass = 1;
    private float drag = 10;
    private float angularDrag = 0.05f;
    private bool useGravity = false;
    private bool freezeRotationXYZ = true;
    private Collider[] colliders;
    private RigBuilder rigBuilder;

    private Animator anim;
    public SkinnedMeshRenderer skinRenderer;




    [Header("HP and stuff"),Space(20)]
    public float hp;
    public float hpMax;
    public Image hpImage;
    public GameObject hpPosition;
    public Camera cam;
    public  float reduceSpeed =2;
    public float targetHP=1;
    Color tempColor;



    [Header("Movement target and options"),Space(20)]
    public Transform target;
    Butterfly butterfly;
    public Transform patrolPoint;
    [Space(10)]
    public float rotationSpeed;
    public float moveSpeed;
    [Space(10)]
    public bool targetDetected;
    public float waitTime;
    private float waitTimeOld;
    public float playerDetectionRange;



    [Header("Collision avoidence"),Space(20)]
    public LayerMask layerMask;
    private RaycastHit hit;
    public bool closeToObsticle;
    public WaspNest nest;



    [Header("Distance to player"),Space(20)]
    public bool toFar;
    public float farDistance =150;
    public float teleportTime = 3;
    private float teleportTimeOld;

    private void Start() {
        rig = GetComponent<Rigidbody>();
        anim = transform.GetComponent<Animator>();
        butterfly = GameObject.FindGameObjectWithTag("Player").GetComponent<Butterfly>();
        target=butterfly.enemieMovmentPoint;
        skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        teleportTimeOld=teleportTime;
        //colliders = GetComponentsInChildren<Collider>();
        rigBuilder = GetComponent<RigBuilder>();
        hpMax = hp;
        cam = Camera.main;
        tempColor = hpImage.color;
        tempColor.a = 0;
        hpImage.color = tempColor;
    }

    private void Update() {
        UpdateHPBar();
    }

    private void FixedUpdate() {
        
        //ako je nest unisten
        if(!nest.isActiveAndEnabled)
            targetDetected = true;

        if(Vector3.Distance(transform.position, target.transform.position)>farDistance){
            ToFarAway();    //ako je igrac pre daleko trebalo bi izgasiti nepotrebne stvari
        }
        else{
            if(toFar){
                anim.enabled=true;
                if(colliders!=null)
                    foreach(Collider c in colliders)
                        c.enabled =true;
                gameObject.AddComponent<Rigidbody>();
                rig = GetComponent<Rigidbody>();
                rig.useGravity = useGravity;
                rig.mass = mass;
                rig.drag = drag;
                rig.angularDrag = angularDrag;
                rig.freezeRotation = freezeRotationXYZ;
                skinRenderer.enabled = true;
                toFar = false;
                rigBuilder.enabled=true;
            }

        
            if(targetDetected){

                LookDirection(target);
                SpeedControlForward();
            }
            else{

                LookDirection(patrolPoint);
                if(Vector3.Distance(transform.position, patrolPoint.position)>1f){
                    SpeedControlForward();
                    waitTime = waitTimeOld;
                }
                else{
                    CalculateNewPatrolPointWasp();
                }

                if(Vector3.Distance(transform.position,target.position)<playerDetectionRange){
                    targetDetected=true;
                    //Debug.Log("Detected player");
                }

            }
            //Debug.DrawRay(transform.position,transform.TransformDirection( Vector3.down) * raycastDistance, Color.red);
            if(closeToObsticle)
                CollisionAvoiding();
        }
    }

    void LookDirection(Transform lookAt){
        Vector3 direction = -(transform.position-lookAt.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

    }

    void SpeedControlForward(){
        Vector3 velocity = new Vector3(rig.velocity.x, rig.velocity.y, rig.velocity.z);

        rig.AddForce(transform.forward * moveSpeed, ForceMode.Force);
    }

    void CollisionAvoiding(){
        if(Physics.Raycast(transform.position, 
                        transform.TransformDirection((Vector3.forward+ (Vector3.right/3)).normalized),
                        out hit, 
                        5,
                        layerMask))
            {
                rig.AddForce(-transform.right*moveSpeed*1.5f,ForceMode.Force);
            }

        
        else if(Physics.Raycast(transform.position, 
                        transform.TransformDirection((Vector3.forward- (Vector3.right/3)).normalized),
                        out hit, 
                        5,
                        layerMask))
            {
                rig.AddForce(transform.right*moveSpeed*1.5f,ForceMode.Force);
            }
        else if(Physics.Raycast(transform.position, 
                    transform.TransformDirection(Vector3.forward),
                    out hit, 
                    5,
                    layerMask))
        {
            rig.AddForce(-transform.right*moveSpeed*1.5f,ForceMode.Force);
        }
    }

    public void CalculateNewPatrolPointWasp(){
        waitTime-=Time.deltaTime;
        if(waitTime<0){
            CalculateNewPatrolPoint();
            waitTime=waitTimeOld;
        }
    }

    public void CalculateNewPatrolPoint(){
        patrolPoint.transform.position = nest.NextPatrolPointLocation();
        StartCoroutine(LookAtPatrol());
    }

    IEnumerator LookAtPatrol(){
        yield return new WaitForSeconds(0.5f);
        transform.LookAt(patrolPoint.position);
    }


    public void ToFarAway(){
        if(!toFar){
            if(colliders!=null)
                foreach(Collider c in colliders)
                    c.enabled =false;
            anim.enabled=false;
            Destroy(rig);
            skinRenderer.enabled = false;
            toFar = true;
            rigBuilder.enabled=false;
        }
        if(transform.position == patrolPoint.position){
            CalculateNewPatrolPointWasp();
            teleportTime = teleportTimeOld;
            return;
        }
        teleportTime-=Time.deltaTime;
        if(teleportTime<0){
            Debug.Log("iTS hERE");
            if(Vector3.Distance(transform.position,patrolPoint.position)<teleportTimeOld){
                Debug.Log("2");
                transform.position=patrolPoint.position;
                teleportTime = teleportTimeOld;
                return;
            }
            transform.position = transform.position+transform.forward*teleportTimeOld;
            teleportTime = teleportTimeOld;
        }
    }

    public void UpdateHPBar(){
        if(hp<0)
            Destroyed();
        
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

    public void Destroyed(){
        anim.enabled = false;
        rig.mass = 0.5f;
        rig.drag=0;
        rig.angularDrag = 0.05f;
        rig.useGravity=true;
        rig.constraints = RigidbodyConstraints.None;
        nest.waspsInNest.Remove(gameObject);
    }

    IEnumerator ToFarAwayCorutine(){
        if(!toFar){
            foreach(Collider c in colliders)
                c.enabled =false;
            anim.enabled=false;
            Destroy(rig);
            skinRenderer.enabled = false;
            toFar = true;
            rigBuilder.enabled=false;
        }
        yield return new WaitForSeconds(2);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Player")){
            butterfly.dead=true;
        }
    }
}
