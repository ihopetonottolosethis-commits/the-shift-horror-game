using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// Main menu UI controller. Handles game start, settings, and quit.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Canvas menuCanvas;
    
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;
    
    private void Start()
    {
        startButton.onClick.AddListener(OnStartClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
        
        Debug.Log("[MainMenuUI] Main menu initialized");
    }
    
    private void OnStartClicked()
    {
        Debug.Log("[MainMenuUI] Game started");
        GameManager.Instance.StartGame();
        menuCanvas.gameObject.SetActive(false);
    }
    
    private void OnSettingsClicked()
    {
        Debug.Log("[MainMenuUI] Settings opened");
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
    
    private void OnCreditsClicked()
    {
        Debug.Log("[MainMenuUI] Credits opened");
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }
    
    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Game quit");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
