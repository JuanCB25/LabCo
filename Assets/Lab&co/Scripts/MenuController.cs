using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    //cargar el primer nivel del videojuego
    public void PlayGame(){
        SceneManager.LoadScene("Level01_Sotano");
    }

    //salir del videojuego
    public void ExitGame(){
        Debug.Log("Cerrando Lab & co...");

#if UNITY_EDITOR
        //si estamos probando desde Unity, salimos del modo play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //si es el juego ejecutable, cerramos la aplicacion
        Application.Quit();
#endif
    }
}
