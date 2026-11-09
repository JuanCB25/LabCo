
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Main Menu")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start(){
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver(){
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("GAME OVER: Tiempo agotado");
    }

    public void RetryLevel(){
        Time.timeScale = 1f;
         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu(){
        Time.timeScale = 1f;
         SceneManager.LoadScene(mainMenuSceneName);
    }
}
