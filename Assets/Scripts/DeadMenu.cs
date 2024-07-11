using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenu : MonoBehaviour
{
    [SerializeField] private GameObject deadmenu;

    void Start()
    {
        deadmenu.SetActive(false);
    }

    void Update()
    {
      
    }

    public void RestartGame()
    {
        if (deadmenu != null)
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Game");
        }
    }

    public void QuitToMenu()
    {
        if (deadmenu != null)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
