
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    [Header("Panel de final de partida")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text tituloPanel;
    [SerializeField] private GameObject botonContinuar;
    [SerializeField] private TMPro.TMP_Text mensajeFinal;

    [Header("Escenas")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string siguienteNivelSceneName = "Level02_Sotano";

    [Header("Progresión")]
    [SerializeField] private int nivelActual = 1;

    [Header("Mensajes de final")]
    [SerializeField] private GameObject gameOverMessage;
    [SerializeField] private GameObject levelCompleteMessage;

    private const string CLAVE_NIVEL_DESBLOQUEADO =
        "LabCo_NivelMaxDesbloqueado";

    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (botonContinuar != null)
            botonContinuar.SetActive(false);
    }

    // FINAL POR TIEMPO AGOTADO
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        if (tituloPanel != null)
            tituloPanel.text = "GAME OVER";

        if (gameOverMessage != null)
            gameOverMessage.SetActive(true);

        if (levelCompleteMessage != null)
            levelCompleteMessage.SetActive(false);

        if (botonContinuar != null)
            botonContinuar.SetActive(false);

        if (mensajeFinal != null)
        {
            mensajeFinal.text = "Te han pillado en el laboratorio. Se ha terminado el tiempo.";
        }
        Time.timeScale = 0f;

        Debug.Log("GAME OVER: Tiempo agotado");
    }

    // FINAL POR COMPLETAR EL NIVEL
    public void ShowLevelComplete()
    {
        gameOverPanel.SetActive(true);
       
        if (tituloPanel != null)
            tituloPanel.text = "¡NIVEL SUPERADO!";

        if (gameOverMessage != null)
            gameOverMessage.SetActive(false);

        if (levelCompleteMessage != null)
            levelCompleteMessage.SetActive(true);

        if (botonContinuar != null)
            botonContinuar.SetActive(true);

        int nivelDesbloqueado =
            PlayerPrefs.GetInt("LabCo_NivelMaxDesbloqueado", 1);

        int nuevoNivelDesbloqueado = nivelActual + 1;

        if (mensajeFinal != null)
        {
            mensajeFinal.text ="¡Pedido entregado correctamente! Has desbloqueado el siguiente nivel.";
        }

        if (nuevoNivelDesbloqueado > nivelDesbloqueado)
        {
            PlayerPrefs.SetInt(
                "LabCo_NivelMaxDesbloqueado",
                nuevoNivelDesbloqueado
            );

            PlayerPrefs.Save();
        }

        Time.timeScale = 0f;

        Debug.Log("¡Nivel " + nivelActual + " superado!");
    }

    // REINTENTAR NIVEL
    public void RetryLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // VOLVER AL MENÚ
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    // CONTINUAR AL SIGUIENTE NIVEL
    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;

        if (string.IsNullOrEmpty(siguienteNivelSceneName))
        {
            Debug.LogError("No se ha asignado la escena del siguiente nivel.");
            return;
        }

        SceneManager.LoadScene(siguienteNivelSceneName);
    }
}
