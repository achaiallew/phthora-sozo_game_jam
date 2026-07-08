using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{

    public bool gameActive = false;
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private Animator playerAnim;

    [SerializeField] private CinemachineInputAxisController camControl;
    [SerializeField] private Camera firstPersonCam;
    [SerializeField] private CinemachineCamera thirdPersonCam;
    private SpawnManager spawnManager;

    private float score = 0f;

    public int killCount = 0;
    private float gameTime;
    private float accuracy;

    [SerializeField] private Image healthBar;
    [SerializeField] private TextMeshProUGUI excessBulletText;
    [SerializeField] private TextMeshProUGUI magBulletText;
    private int magazine = 6;
    private int excessBullets = 10;
    private int magBullets = 6;

    [SerializeField] private GameObject playerHUD;
    [SerializeField] private GameObject pannel;

    

    void Awake()
    {
        //spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        playerController = player.GetComponent<PlayerController>();
        playerAnim = player.GetComponent<Animator>();
        camControl.enabled = false;
        thirdPersonCam.enabled = false;
        firstPersonCam.enabled = true;
        playerHUD.SetActive(false);
    }

    void Start()
    {
        // Cut Scene?
        playerAnim.SetTrigger("getUp");
        Invoke("StandUp", 6f);
    
    }

    void StandUp()
    {
        playerAnim.SetTrigger("standUp");
        Invoke("StartGame", 3f);
    }

    void StartGame()
    {
        gameActive = true;
        thirdPersonCam.enabled = true;
        firstPersonCam.enabled = false;
        camControl.enabled = true;
        playerHUD.SetActive(true);
        score = 0f;
        killCount = 0;
    }

     void Update()
    {
        CalculateScore();  
        TrackPlayerHealth();     
    }

    void CalculateScore()
    {
        gameTime += Time.deltaTime;

        if (playerController.shotsTaken > 0)
        {
            accuracy = playerController.shotsOnTarget/playerController.shotsTaken;
            //gameTime and kill count

            score = accuracy*100; 
        }
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

    public void TrackPlayerBullets()
    {
        if (magBullets > 0)
        {
            magBullets --;
            magBulletText.text = magBullets.ToString();

            if (magBullets == 0)
            {
                //TODO: Empty Mag Sound or Reload Text
                playerController.shootAction.Disable();
            }
        }
        

        
        excessBulletText.text = excessBullets.ToString();

        //TODO: Reloading
        // excessBullets -= magazine;
        // magBullets += magazine;

        
        //TODO: Pick Up Bullets



    }

    public void GameOver()
    {
        gameActive = false;
        camControl.enabled = false;
        Debug.Log("Game Over!");
        Debug.Log("Score: " + score);
    }
}
