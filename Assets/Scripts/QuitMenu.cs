using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitMenu : MonoBehaviour
{
    SavePlayerPos playerPosData;
    GameObject player;

    private void Start()
    {
        playerPosData = FindObjectOfType<SavePlayerPos>();
    }

    public void Quit()
    {
        if (playerPosData != null)
        {
            playerPosData.PlayerPosSave();
        }

        SceneManager.LoadScene("MainMenu");
    }
}
