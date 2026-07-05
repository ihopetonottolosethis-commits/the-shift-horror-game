using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game over screen UI controller.
/// </summary>
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Canvas gameOverCanvas;
    [SerializeField] private Text messageText;
    [SerializeField] private Text statsText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;
    
    private void Start()
    {
        gameOverCanvas.gameObject.SetActive(false);
        retryButton.onClick.AddListener(OnRetryClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        
        GameManager.Instance.OnGameEnded += ShowGameOver;
        Debug.Log("[GameOverUI] Game over screen initialized");
    }
    
    private void ShowGameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
        GameManager.SessionData session = GameManager.Instance.GetSessionData();
        
        messageText.text = "ENTITY CLAIMED YOU";
        statsText.text = $"Playtime: {session.playtime:F1}s\nSanity: {session.finalSanity:F0}%\nTasks: {session.tasksCompleted}/7";
        
        Debug.Log("[GameOverUI] Game over screen shown");
    }
    
    private void OnRetryClicked()
    {
        Debug.Log("[GameOverUI] Restarting game");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayMuseum");
    }
    
    private void OnMainMenuClicked()
    {
        Debug.Log("[GameOverUI] Returning to main menu");
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
