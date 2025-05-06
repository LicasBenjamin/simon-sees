using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        Debug.Log("Switching to Main Menu...");
        SceneManager.LoadScene("Menu"); // Replace with your scene's exact name
    }
}
