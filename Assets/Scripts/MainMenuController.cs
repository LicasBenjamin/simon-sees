using UnityEngine;
using UnityEngine.SceneManagement; // For scene changing

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // This only works in the built game, not the Editor
    }
}
