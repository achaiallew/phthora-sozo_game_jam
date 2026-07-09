using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private InputAction menuAction;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject settingMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private Volume globalVolume;

    public AudioMixer audioMixer;
    [SerializeField] private Slider sensSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    void Awake()
    {
        // Enable Menu (Esc) Action
        menuAction.Enable();

        // Ensure Settings and Options Hidden
        settingMenu.SetActive(false);
        optionsMenu.SetActive(false);

        LoadPlayerSettings();

    }

    void Update()
    {
        if (menuAction.triggered)
        {
            // Open Settings Menu
            settingMenu.SetActive(true);

            //TODO:Pause Game
            gameManager.gameActive = false;
            gameManager.camControl.enabled = false;
        }
        
    }

    public void Resume()
    {
        // Close Settings Menu
        settingMenu.SetActive(false);

        // Resume Game 
        gameManager.gameActive = true;
        gameManager.camControl.enabled = true;
  
    }

    public void Options()
    {
        // Open Options Menu
        optionsMenu.SetActive(true);

        // Close Settings Menu
        settingMenu.SetActive(false);
    }

    public void QuitToMenu()
    {
        // Load Main Menu
        SceneManager.LoadScene(0);
    }

    public void AdjustMusic(float volume)
    {
        // Adjust Music Volume
        audioMixer.SetFloat("MusicVolume", volume);
 
    }

    public void AdjustSFX(float volume)
    {
        // Adjust Sound Effects Volume
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void MouseSens()
    {
        gameManager.UpdateMouseSens(sensSlider.value);
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
        PlayerPrefs.SetFloat("MouseSens", sensSlider.value);

    }

    public void LoadPlayerSettings()
    {
        if (PlayerPrefs.HasKey("MusicVolume")){
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
        if (PlayerPrefs.HasKey("SFXVolume")){
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (PlayerPrefs.HasKey("MouseSens")){
            sensSlider.value = PlayerPrefs.GetFloat("MouseSens");
        }   
    }

    public void CloseOptions()
    {
        // Close Options Menu
        optionsMenu.SetActive(false);

        // Close Settings Menu
        settingMenu.SetActive(true);
    }
}
