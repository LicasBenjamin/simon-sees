using UnityEngine;
using UnityEngine.SceneManagement; // For scene changing

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Unlocks the cursor
        Cursor.visible = true; // Makes cursor visible
        Debug.Log("Cursor unlocked in Main Menu!");
    }
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
