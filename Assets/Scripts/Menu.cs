using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");

        SceneManager.LoadScene("Game"); 
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game"); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
