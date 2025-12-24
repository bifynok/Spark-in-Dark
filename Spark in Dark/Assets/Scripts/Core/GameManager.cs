using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance != null) return;

        var gameManagerGO = new GameObject("GameManager");
        Instance = gameManagerGO.AddComponent<GameManager>();
        DontDestroyOnLoad(gameManagerGO);
    }

    public void StartNewGame()
    {
        SceneLoader.Instance.SwitchScene("Scene_Map","Scene_MainMenu");
    }

    public void ToMap()
    {
        SceneLoader.Instance.SwitchScene("Scene_Map","Scene_Battle");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
