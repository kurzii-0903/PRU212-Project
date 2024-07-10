using UnityEngine;

public class SavePlayerPos : MonoBehaviour
{
    public Transform playerTransform; 
    const string PLAYER_X_KEY = "PlayerX";
    const string PLAYER_Y_KEY = "PlayerY";

    private void Start()
    {
        if (PlayerPrefs.HasKey(PLAYER_X_KEY) && PlayerPrefs.HasKey(PLAYER_Y_KEY))
        {
            float x = PlayerPrefs.GetFloat(PLAYER_X_KEY);
            float y = PlayerPrefs.GetFloat(PLAYER_Y_KEY);
            playerTransform.position = new Vector2(x, y);
        }
    }

    public void PlayerPosSave()
    {
        PlayerPrefs.SetFloat(PLAYER_X_KEY, playerTransform.position.x);
        PlayerPrefs.SetFloat(PLAYER_Y_KEY, playerTransform.position.y);
        PlayerPrefs.Save(); 
    }
}
