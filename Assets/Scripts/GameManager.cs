using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    public bool gameActive = false;
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private Animator playerAnim;

    public CinemachineInputAxisController camControl;
    [SerializeField] private Camera firstPersonCam;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private CinemachineCamera thirdPersonCam;

    private float score = 0f;

    public int killCount = 0;
    private float gameTime;

    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI excessBulletText;
    [SerializeField] private TextMeshProUGUI magBulletText;
    public int magazine = 6;
    public int excessBullets = 10;
    public int magBullets = 6;

    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject pannel;

    private VoiceLineManager voiceLineManager;
    [SerializeField] private AudioSource systemSource;
    [SerializeField] private AudioSource playerSource;
    public AudioClip emptyMagSound;
    public AudioClip reloadSound;
    public AudioClip fullMagSound;

    [SerializeField] private TypewriterText dialogue;

    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject pistolOnTable;

    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject tutorialPannel;

    private Coroutine openSceneCoroutine;

    [SerializeField] private GameObject gameOverDisplay;

    [SerializeField] private GameObject endGamePannel;
    [SerializeField] private GameObject winGamePannel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject scoreDisplay;


    void Awake()
    {
        //spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        playerController = player.GetComponent<PlayerController>();
        playerAnim = player.GetComponent<Animator>();
        camControl.enabled = false;
        thirdPersonCam.enabled = false;
        firstPersonCam.enabled = false;
        blackScreen.SetActive(true);
        playerHUD.SetActive(false);
        pannel.SetActive(false);
        tutorialPannel.SetActive(false);
        endGamePannel.SetActive(false);
        winGamePannel.SetActive(false);
        scoreDisplay.SetActive(false);
        
        pistol.SetActive(false);
        gameOverDisplay.SetActive(false);

        gameActive = false;

        // Assign Voice Line Manager
        voiceLineManager = GetComponent<VoiceLineManager>();
    }

    void Start()
    {
        // Start Cutscene
        openSceneCoroutine = StartCoroutine(OpenScene());
    }

    IEnumerator OpenScene()
    {
        // Play System Reboot Voice Line
        PlaySystemVoiceLine(voiceLineManager.warning);
        yield return new WaitForSeconds(voiceLineManager.warning.length);
        PlaySystemVoiceLine(voiceLineManager.lockdown);
        yield return new WaitForSeconds(voiceLineManager.lockdown.length + 2);

        // Enable FP Camera
        firstPersonCam.enabled = true;
        blackScreen.SetActive(false);

        // Player Get Up Animation
        Invoke("GetUp", 4f);

        // Play Player Confusion Voice Line
        PlayerVoiceLine(voiceLineManager.v1);
        yield return new WaitForSeconds(voiceLineManager.v1.length + 3);

        // Set AI Pannel Active
        pannel.SetActive(true);
       
        // Play System Intructions Voice Line
        PlaySystemVoiceLine(voiceLineManager.mission);
        // Play Mission Text
        dialogue.PlayText("Hello M-5356,\n\nI am the voice of the MNEMOS Main System.\n"
                        + "There has been a mass security breach and all advanced\n androids under the MNEMOS system have been rebooted. \nFacility lockedown protocols have been initiated.\n"
                        + "\nDestroy enemy operatives \n and congregate to the main deck.");
        yield return new WaitForSeconds(voiceLineManager.mission.length + 2);

        // Set AI Pannel Active
        pannel.SetActive(false);
        
        // Play Player Question AI Voice Line
        PlayerVoiceLine(voiceLineManager.v2);
        yield return new WaitForSeconds(voiceLineManager.v2.length + 2);

        // Play System Intructions Voice Line
        PlaySystemVoiceLine(voiceLineManager.confirmAI);
        yield return new WaitForSeconds(voiceLineManager.confirmAI.length + 2);

        // Play Player Accept AI Voice Line
        PlayerVoiceLine(voiceLineManager.v3);
        yield return new WaitForSeconds(voiceLineManager.v3.length + 2);

        // Play System Intructions Voice Line
        PlaySystemVoiceLine(voiceLineManager.retrieveWeapon);
        yield return new WaitForSeconds(voiceLineManager.retrieveWeapon.length);

        // Indicate Pistol with Lighting
        if (pistolOnTable.activeInHierarchy)
        {
            pistolOnTable.GetComponent<Light>().enabled = true;
        }
    
    }


    void GetUp()
    {
        // Player Get Up Animation
        playerAnim.SetTrigger("getUp");

        // Stand Up
        Invoke("StandUp", 30f);
    }


    void StandUp()
    {
        // Player Stand Up Animation
        playerAnim.SetTrigger("standUp");

        // Start Game
        Invoke("StartGame", 1f);
    }

    void StartGame()
    {
        // Activate Game
        gameActive = true;
        // Disable Player Actions
        playerController.shootAction.Disable();
        playerController.sprintAction.Disable();
        playerController.reloadAction.Disable();
        playerController.jumpAction.Disable();

        thirdPersonCam.enabled = true;
        firstPersonCam.enabled = false;
        camControl.enabled = true;
        
    }

     void Update()
    {

        TrackPlayerHealth();  
        TrackPlayerBullets();
    }

    public void Tutorial(string text)
    {
        // Prompt User with Tutorial Pannel
        tutorialPannel.SetActive(true);
        tutorialText.text = text;       
    }

    void TrackPlayerHealth()
    {
        float health = playerController.playerHealth;
        health = Mathf.Clamp(health, 0f, 100);        
        healthBar.fillAmount = health/100;



        if (health <= 0)
        {
            playerAnim.SetTrigger("isDead");
            GameOver();
            // SceneManager.LoadScene(2); // Game Over Screen
        }

    }

    public void PickUpGun()
    {
        //Stop Opening Scene Dialogue
        if (openSceneCoroutine != null)
        {
            StopCoroutine(openSceneCoroutine);
        }
        

        // PickUp Gun Animation
        playerAnim.SetTrigger("pickupGun");

        // Destory Gun on Table
        Destroy(pistolOnTable);

        // Enable All Player Actions
        playerController.shootAction.Enable();
        playerController.sprintAction.Enable();
        playerController.reloadAction.Enable();
        playerController.jumpAction.Enable();

        // Play Player Pickup Gun Voice Line
        PlayerVoiceLine(voiceLineManager.v4);

        //Add Pistol to Hand
        pistol.SetActive(true);

        // Ensure Tutorial Disabled
        tutorialPannel.SetActive(false);

        // Add Player HUD
        playerHUD.SetActive(true);

        // Player Has Gun
        playerController.haveGun = true;

        // Ensure Score and Kill Count is Zero
        score = 0f;
        killCount = 0;
    }

    void TrackPlayerBullets()
    {
        magBulletText.text = magBullets.ToString();
        excessBulletText.text = excessBullets.ToString();

        if (magBullets == 0)
        {
            playerController.shootAction.Disable();
        }
        
        //TODO: Pick Up Bullets
    }

    public void PickUpBullets()
    {
        int ammo = Random.Range(2, 4);
        excessBullets += ammo;

        // Disable Tutorial
        tutorialPannel.SetActive(false);

        // Collect Ammo Once
        playerController.collectAmmo = true;


    }

    public void UpdateMouseSens(float mouseSens)
    {
        foreach (var c in camControl.Controllers)
        {
            if (c.Name == "Look X (Pan)")
            {
                c.Input.Gain = mouseSens;
                break;
            }
        }
    }

    public void EndGamePannel()
    {
        gameActive = false;
        camControl.enabled = false;

        // Disable All Other UI
        playerHUD.SetActive(false);
        tutorialPannel.SetActive(false);
        winGamePannel.SetActive(false);

        // Enable End Game Pannel
        endGamePannel.SetActive(true);
    }

    void PlaySystemVoiceLine(AudioClip clip)
    {
        systemSource.PlayOneShot(clip);
    }

    public void PlayerVoiceLine(AudioClip clip)
    {
        playerSource.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip)
    {
        systemSource.PlayOneShot(clip);
    }

    public void WinGame()
    {
        gameActive = false;
        camControl.enabled = false;

        // Disable All Other UI
        playerHUD.SetActive(false);
        tutorialPannel.SetActive(false);
        endGamePannel.SetActive(false);

        // Enable Win Screen
        winGamePannel.SetActive(true);
        scoreDisplay.SetActive(true);

        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameActive = false;
        camControl.enabled = false;

        // Disable All Other UI
        playerHUD.SetActive(false);
        tutorialPannel.SetActive(false);

        // Enable Game Over
        gameOverDisplay.SetActive(true);
        scoreDisplay.SetActive(true);

        float endTime = Time.time;
        Debug.Log("End Time: " + endTime);
        score = playerController.accuracy*killCount*10000/endTime; 
        Debug.Log("Score: " + score);

        scoreText.text = "Score: " + Mathf.Round(score);
        Debug.Log("Score: " + score);
    }

    public void Retry()
    {
        SceneManager.LoadScene(1);
    }
}
