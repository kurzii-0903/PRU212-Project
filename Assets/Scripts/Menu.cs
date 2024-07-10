using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void NewGame()
    {
        PlayerPrefs.DeleteKey("PlayerX");
        PlayerPrefs.DeleteKey("PlayerY");

        Time.timeScale = 1;

        SceneManager.LoadScene("Game");
    }


    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            float playerX = PlayerPrefs.GetFloat("PlayerX", 0);
            float playerY = PlayerPrefs.GetFloat("PlayerY", 0);

            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.transform.position = new Vector2(playerX, playerY);
            }
            Time.timeScale = 1;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
