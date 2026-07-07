using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;

    private float time = 0f;
    private float timeDelay = 1f;

    void Update()
    {
        
        time  += Time.deltaTime;
        if (time > timeDelay)
        {
            time = 0f;            
            timeDelay = Random.Range(10f, 20f);

            playerAnim.SetTrigger("kick");
        }
            
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
