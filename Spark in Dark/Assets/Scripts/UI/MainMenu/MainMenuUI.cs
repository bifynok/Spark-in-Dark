using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public void OnNewGame()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnQuit()
    {
        GameManager.Instance.QuitGame();
    }
}
