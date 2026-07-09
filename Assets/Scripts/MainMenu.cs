using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator playerAnim;
    [SerializeField] private GameObject optionsMenu;

    public AudioMixer audioMixer;
    [SerializeField] private Slider sensSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    private float senseSettings;

    private float time = 0f;
    private float timeDelay = 1f;

    void Update()
    {
        
        time  += Time.deltaTime;
        if (time > timeDelay)
        {
            time = 0f;            
            timeDelay = Random.Range(15f, 25f);

            playerAnim.SetTrigger("kick");
        }
            
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Options()
    {
        // Open Options Menu
        optionsMenu.SetActive(true);
    }

    public void AdjustMusic(float volume)
    {
        // Adjust Music Volume
        audioMixer.SetFloat("MusicVolume", volume);
 
    }

    public void AdjustSFX(float volume)
    {
        // Adjust Sound Effects Volume
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void MouseSens()
    {
        senseSettings = sensSlider.value;
    }

    public void SavePlayerSettings()
    {
        // Save Music Volume Settings
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        // Save SFX Volume Settings
        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        // Save Mouse Sense Settings
        PlayerPrefs.SetFloat("MouseSens", senseSettings);

    }

    public void LoadPlayerSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume")){
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            sensSlider.value = PlayerPrefs.GetFloat("MouseSens");
        }  
    }

    public void CloseOptions()
    {
        // Close Options Menu
        optionsMenu.SetActive(false);
    }

    public void Quit()
    {
        //TODO: Quit Game
    }
}
