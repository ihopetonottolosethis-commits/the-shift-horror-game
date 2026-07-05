using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Victory screen UI controller - shown when player survives until 6 AM.
/// </summary>
public class VictoryUI : MonoBehaviour
{
    [SerializeField] private Canvas victoryCanvas;
    [SerializeField] private Text messageText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;
    
    private void Start()
    {
        victoryCanvas.gameObject.SetActive(false);
        continueButton.onClick.AddListener(OnContinueClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        GameManager.Instance.OnGameEnded += ShowVictory;
        Debug.Log("[VictoryUI] Victory screen initialized");
    }
    
    private void ShowVictory()
    {
        if (GameManager.Instance.GetCurrentState() != GameManager.GameState.Victory) return;
        
        victoryCanvas.gameObject.SetActive(true);
        GameManager.SessionData session = GameManager.Instance.GetSessionData();
        
        messageText.text = "YOU SURVIVED THE NIGHT";
        statsText.text = $"Playtime: {session.playtime:F1}s\nSanity: {session.finalSanity:F0}%\nTasks Completed: {session.tasksCompleted}/7";
        
        Debug.Log("[VictoryUI] Victory screen shown");
    }
    
    private void OnContinueClicked()
    {
        Debug.Log("[VictoryUI] Continuing to main menu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
    
    private void OnMainMenuClicked()
    {
        Debug.Log("[VictoryUI] Returning to main menu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
