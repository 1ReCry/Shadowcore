using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public int NextLevelID;
    public GameObject cameram;
    public float SmoothPos = 1f;
    public GameObject cameraPos0;
    public GameObject cameraPos1;
    public GameObject cameraPos2;
    public GameObject cameraPos3;
    public GameObject cameraPos4;

    //scene loading
    AsyncOperation asyncOperation;
    public Slider sliderLoading;
    public GameObject loadingObj;
    private float activationTimer;
    public TextMeshProUGUI startCampaignText;
    public TextMeshProUGUI nextLevelText;
    public GameObject notesButton;
    public AudioSource menuMusic;
    public GameObject CoreObj;
    public TextMeshProUGUI adventuresText;
    public GameObject storyButton;
    public Slider musicVolumeSlider;
    public Slider soundVolumeSlider;
    public Button survivalButton;
    public GameObject completeSurvivalAlert;

    private bool volumes_loaded;

    void Start()
    {
        Globals.survivalMode = false;
        volumes_loaded = false;
        Time.timeScale = 1f;
        sliderLoading.gameObject.SetActive(false);
        loadingObj.SetActive(false);
        Globals.MenuCameraPosID = 0;
        Cursor.visible = true;
        NextLevelID = Globals.completedLevelID + 1;
        int randomMusic = Random.Range(1,3);
        if(randomMusic == 1) menuMusic.clip = Resources.Load<AudioClip>("Audio/Soundtrack/mainMenu");
        if(randomMusic == 2) menuMusic.clip = Resources.Load<AudioClip>("Audio/Soundtrack/mainMenu2");
        menuMusic.Play();
        if(Application.isEditor) menuMusic.Play();//menuMusic.Stop();
        Debug.Log("Random music is " + randomMusic);

        if(Globals.firstLoadAlready)
        {
            Globals.SaveData();
            soundVolumeSlider.value = Globals.soundVolume;
            musicVolumeSlider.value = Globals.musicVolume;
            volumes_loaded = true;
        }
        if(!Globals.firstLoadAlready)
        {
            Globals.LoadData();
            Globals.firstLoadAlready = true;
            soundVolumeSlider.value = Globals.soundVolume;
            musicVolumeSlider.value = Globals.musicVolume;
            volumes_loaded = true;
        }
    }

    void Update()
    {
        NextLevelID = Globals.completedLevelID + 1;
        if(Globals.completedLevelID == 0 || Globals.completedLevelID < 0){
            startCampaignText.text = "Начать приключение";
        }
        if(Globals.completedLevelID > 0){
            startCampaignText.text = "Продолжить";
        }

        if(Globals.completedLevelID < 3 && Globals.gameCompletesCount <= 0){
            notesButton.SetActive(false);
        }
        if(Globals.completedLevelID >= 3 || Globals.gameCompletesCount > 0){
            notesButton.SetActive(true);
        }
        if(NextLevelID != 10){
            menuMusic.pitch = 1f;
            CoreObj.SetActive(false);
        }
        if(NextLevelID == 10){
            menuMusic.pitch = 0.7f;
            CoreObj.SetActive(true);
        }

        //прохождение
        if(Globals.completedLevelID >= 10)
        {
            Globals.completedLevelID = 0;
            Globals.gameCompletesCount += 1;
            NextLevelID = Globals.completedLevelID + 1;
        }
        if(Globals.gameCompletesCount <= 0) adventuresText.text = "Приключение";
        if(Globals.gameCompletesCount > 0) adventuresText.text = "Приключение (пройдено х"+Globals.gameCompletesCount+")";

        if(Globals.gameCompletesCount > 0){
            storyButton.SetActive(true);
        }
        if(Globals.gameCompletesCount <= 0){
            storyButton.SetActive(false);
        }

        nextLevelText.text = Globals.levelName[NextLevelID];

        //if(Input.GetKeyUp(KeyCode.LeftAlt) && Application.isEditor)
        //{
        //    Globals.completedLevelID += 1;
        //}


        if(Globals.MenuCameraPosID == 0)
        {
            cameram.transform.position = Vector3.Lerp(cameram.transform.position,cameraPos0.gameObject.transform.position, SmoothPos * Time.deltaTime);
            cameram.transform.rotation = Quaternion.Lerp(cameram.transform.rotation,cameraPos0.gameObject.transform.rotation, SmoothPos * Time.deltaTime);
        }
        if(Globals.MenuCameraPosID == 1)
        {
            cameram.transform.position = Vector3.Lerp(cameram.transform.position,cameraPos1.gameObject.transform.position, SmoothPos * Time.deltaTime);
            cameram.transform.rotation = Quaternion.Lerp(cameram.transform.rotation,cameraPos1.gameObject.transform.rotation, SmoothPos * Time.deltaTime);
        }
        if(Globals.MenuCameraPosID == 2)
        {
            cameram.transform.position = Vector3.Lerp(cameram.transform.position,cameraPos2.gameObject.transform.position, SmoothPos * Time.deltaTime);
            cameram.transform.rotation = Quaternion.Lerp(cameram.transform.rotation,cameraPos2.gameObject.transform.rotation, SmoothPos * Time.deltaTime);
        }
        if(Globals.MenuCameraPosID == 3)
        {
            cameram.transform.position = Vector3.Lerp(cameram.transform.position,cameraPos3.gameObject.transform.position, SmoothPos * Time.deltaTime);
            cameram.transform.rotation = Quaternion.Lerp(cameram.transform.rotation,cameraPos3.gameObject.transform.rotation, SmoothPos * Time.deltaTime);
        }
        if(Globals.MenuCameraPosID == 4)
        {
            cameram.transform.position = Vector3.Lerp(cameram.transform.position,cameraPos4.gameObject.transform.position, SmoothPos * Time.deltaTime);
            cameram.transform.rotation = Quaternion.Lerp(cameram.transform.rotation,cameraPos4.gameObject.transform.rotation, SmoothPos * Time.deltaTime);
        }

        if (asyncOperation != null)
        {
            if(asyncOperation.progress <= 0){
                sliderLoading.gameObject.SetActive(false);
                loadingObj.SetActive(false);
            }
            if(asyncOperation.progress > 0){
                sliderLoading.gameObject.SetActive(true);
                loadingObj.SetActive(true);
                sliderLoading.value = asyncOperation.progress;
            }
        }

        if(activationTimer > 0 && asyncOperation != null)
        {
            activationTimer -= Time.deltaTime;
        }
        if(activationTimer <= 0 && asyncOperation != null)
        {
            asyncOperation.allowSceneActivation = true;
        }

        if(volumes_loaded)
        {
            Globals.soundVolume = soundVolumeSlider.value;
            Globals.musicVolume = musicVolumeSlider.value;
        }

        if(Globals.gameCompletesCount <= 0)
        {
            survivalButton.interactable = false;
            completeSurvivalAlert.SetActive(true);
        }
        if(Globals.gameCompletesCount > 0)
        {
            survivalButton.interactable = true;
            completeSurvivalAlert.SetActive(false);
        }
    }

    public void SaveData()
    {
        Globals.SaveData();
    }


    public void StartDevLevel(int id)
    {
        activationTimer = 0.6f;
        Globals.CurrentLevelID = id;
        LevelLoader(id);
        
        asyncOperation.allowSceneActivation = false;
    }
    public void ChangeCameraPosID(int id)
    {
        Globals.MenuCameraPosID = id;
    }

    public void StartCampaign()
    {
        activationTimer = 0.6f;
        Globals.CurrentLevelID = Globals.completedLevelID+1;
        LevelLoader(Globals.completedLevelID+1);
        asyncOperation.allowSceneActivation = false;
    }

    public void LevelLoader(int id)
    {
        if(id==0) asyncOperation = SceneManager.LoadSceneAsync("LevelTest");
        if(id==1) asyncOperation = SceneManager.LoadSceneAsync("StartCutscene");
        if(id==2) asyncOperation = SceneManager.LoadSceneAsync("Level2");
        if(id==3) asyncOperation = SceneManager.LoadSceneAsync("Level3");
        if(id==4) asyncOperation = SceneManager.LoadSceneAsync("Level4");
        if(id==5) asyncOperation = SceneManager.LoadSceneAsync("Level5");
        if(id==6) asyncOperation = SceneManager.LoadSceneAsync("Level6");
        if(id==7) asyncOperation = SceneManager.LoadSceneAsync("Level7");
        if(id==8) asyncOperation = SceneManager.LoadSceneAsync("Level8");
        if(id==9) asyncOperation = SceneManager.LoadSceneAsync("Level9");
        if(id==10) asyncOperation = SceneManager.LoadSceneAsync("Level10");
        if(id==11) asyncOperation = SceneManager.LoadSceneAsync("EndCutscene");
        if(id==12) {
            Globals.survivalMode = true;
            asyncOperation = SceneManager.LoadSceneAsync("Survival");
        }
    }

    public void Exit()
    {
        Application.Quit();
    }
}

//коллизии у деревьев новых оффнуть
//анимация смерти 2 раза проигрывается после смерти
//зомби не кусают а СТРЕЛЯЮТ
