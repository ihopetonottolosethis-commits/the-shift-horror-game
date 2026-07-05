using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Pause menu UI controller. Handles pause, resume, and settings.
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private GameObject settingsPanel;
    
    private bool isPaused = false;
    
    private void Start()
    {
        pauseCanvas.gameObject.SetActive(false);
        resumeButton.onClick.AddListener(OnResumeClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        Debug.Log("[PauseMenuUI] Pause menu initialized");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Menu"))
        {
            if (isPaused)
                OnResumeClicked();
            else if (GameManager.Instance.IsGameRunning())
                ShowPauseMenu();
        }
    }
    
    private void ShowPauseMenu()
    {
        isPaused = true;
        pauseCanvas.gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
        Debug.Log("[PauseMenuUI] Pause menu shown");
    }
    
    private void OnResumeClicked()
    {
        isPaused = false;
        pauseCanvas.gameObject.SetActive(false);
        GameManager.Instance.ResumeGame();
        Debug.Log("[PauseMenuUI] Game resumed");
    }
    
    private void OnSettingsClicked()
    {
        Debug.Log("[PauseMenuUI] Settings opened from pause menu");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    private void OnMainMenuClicked()
    {
        isPaused = false;
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        Debug.Log("[PauseMenuUI] Returning to main menu");
    }
}
