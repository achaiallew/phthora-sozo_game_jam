using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{

    public bool gameActive = false;
    [SerializeField] private GameObject player;
    private PlayerController playerController;
    private Animator playerAnim;

    [SerializeField] private CinemachineInputAxisController camControl;
    private SpawnManager spawnManager;

    private float score = 0f;

    public int killCount = 0;
    private float gameTime;
    private float accuracy;

    void Awake()
    {
        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        playerController = player.GetComponent<PlayerController>();
        playerAnim = player.GetComponent<Animator>();
        camControl.enabled = false;
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
        camControl.enabled = true;
        score = 0f;
        killCount = 0;
    }

     void Update()
    {
        gameTime += Time.deltaTime;

        if (playerController.shotsTaken > 0)
        {
            accuracy = playerController.shotsOnTarget/playerController.shotsTaken;

            score = (killCount*accuracy)/gameTime*100; 
        }
        
    }

    public void GameOver()
    {
        gameActive = false;
        camControl.enabled = false;
        Debug.Log("Game Over!");
        Debug.Log("Score: " + Mathf.Round(score));
    }
}
