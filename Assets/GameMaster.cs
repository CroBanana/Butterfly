using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameMaster : MonoBehaviour


{

     [Header("Stuff for spawning on terrain")]
    public GameObject terrain;
    public GameObject waspNest;

    public WaspSpawnZone[] waspSpawnZone;
    public bool activatedAllNestSpawners;




    [Header("UI stuff")]
    public GameObject palyCamera, uiCamera;

    public Butterfly butterfly;
    public Shoot shootButterfly;

    public bool startedPlaying;

    public GameObject playButton, continueButton;

    public GameObject uiPlayer, uiMenu, uiOptions;

    [Header("Options")]

    public Slider mouseSpeed;
    public TextMeshProUGUI mouseSpeedText;
    public Cam camScript;
    public TextMeshProUGUI fullscreenButton;
    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] pcResolution;


    // Start is called before the first frame update
    void Start()
    {
        uiCamera.SetActive(true);
        palyCamera.SetActive(false);
        butterfly = GameObject.FindGameObjectWithTag("Player").GetComponent<Butterfly>();
        shootButterfly =GameObject.FindGameObjectWithTag("Player").GetComponent<Shoot>();
        shootButterfly.enabled=false;
        continueButton.SetActive(false);
        uiPlayer.SetActive(false);
        uiOptions.SetActive(false);
        mouseSpeedText.text=mouseSpeed.value.ToString();

        pcResolution = Screen.resolutions;

        int curentResulutionIndex =0;
        int i =0;
        List<string> tempRes = new List<string>();
        foreach (var res in pcResolution)
        {
            
            tempRes.Add(res.width+" x "+res.height);
            if(res.width == Screen.currentResolution.width &&
                res.height == Screen.currentResolution.height)
            {
                curentResulutionIndex = i;
            }
            i++;
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(tempRes);
        resolutionDropdown.value = curentResulutionIndex;
        resolutionDropdown.RefreshShownValue();

        TerrainData data;
        data = terrain.GetComponent<Terrain>().terrainData;
        waspSpawnZone = GetComponentsInChildren<WaspSpawnZone>();
        foreach(TreeInstance tree in data.treeInstances){
            if(tree.prototypeIndex ==0){
                Vector3 worldTreePos = Vector3.Scale(tree.position, data.size+ terrain.transform.position);
                GameObject nest = Instantiate(waspNest, worldTreePos+new Vector3(1,Random.Range(15,20),0),Quaternion.identity);
                nest.transform.SetParent(transform);
                continue;
            }

            if(tree.prototypeIndex ==1){
                Vector3 worldTreePos = Vector3.Scale(tree.position, data.size+ terrain.transform.position);
                GameObject nest =Instantiate(waspNest, worldTreePos+new Vector3(2,10,2),Quaternion.identity);
                nest.transform.SetParent(transform);
                continue;
            }

            if(tree.prototypeIndex ==2){
                Vector3 worldTreePos = Vector3.Scale(tree.position, data.size+ terrain.transform.position);
                GameObject nest =Instantiate(waspNest, worldTreePos+new Vector3(2,13,2),Quaternion.identity);
                nest.transform.SetParent(transform);
                continue;
            }

        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(startedPlaying){
            if(Input.GetKeyDown(KeyCode.Escape)){
                Debug.Log("Open close menu");
                PlayerCanMove(false);
                CamreasOnEscape();
                MouseOnMenu();
                startedPlaying = false;
                uiMenu.SetActive(true);
                shootButterfly.enabled=false;
            }
        }

    }

    public void PlayButton(){
        ActivateSpawners();
        Camreas();
        MouseOnPlay();
        PlayerCanMove(true);
        startedPlaying = true;
        butterfly.playPressed =true;
        playButton.SetActive(false);
        continueButton.SetActive(true);
        uiPlayer.SetActive(true);
        uiMenu.SetActive(false);
        shootButterfly.enabled=true;
    }

    public void Continue(){
        Camreas();
        MouseOnPlay();
        PlayerCanMove(true);
        startedPlaying = true;
        uiMenu.SetActive(false);
        shootButterfly.enabled=true;
    }

    void ActivateSpawners(){
        if(activatedAllNestSpawners)
            return;
        foreach (WaspSpawnZone zone in waspSpawnZone)
        {
            zone.StartSpawningNests();
        }
        activatedAllNestSpawners = true;
    }

    void Camreas(){
        palyCamera.SetActive(true);
        uiCamera.SetActive(false);
    }

    void CamreasOnEscape(){
        palyCamera.SetActive(false);
        uiCamera.SetActive(true);
    }

    void MouseOnPlay(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PlayerCanMove(bool set){
        butterfly.gameSarted = set;
    }

    void MouseOnMenu(){
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OptionsClick(){
        uiOptions.SetActive(true);
        uiMenu.SetActive(false);
    }

    public void OptionsBack(){
        uiOptions.SetActive(false);
        uiMenu.SetActive(true);
    }

    public void UpdateMouseSpeed(){
        mouseSpeedText.text=mouseSpeed.value.ToString("F2");
        camScript.sensitivityX = (int) (mouseSpeed.value *100);
        camScript.sensitivityY = (int) (mouseSpeed.value *100);
    }

    public void SetFullscreen(bool isFullscreen){
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex){
        Resolution res = pcResolution[resolutionIndex];
        Screen.SetResolution(res.width,res.height, Screen.fullScreen);
    }
    public void Quit(){
        Application.Quit();
    }
}
