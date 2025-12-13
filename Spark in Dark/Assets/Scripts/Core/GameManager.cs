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
        SceneLoader.Instance.LoadSceneAdditive("Scene_Map", true, () =>
        {
            var mapManager = Object.FindFirstObjectByType<MapManager>();
            mapManager.OnSceneActivated();

            StartCoroutine(UnloadMenuNextFrame());
        });
    }

    private IEnumerator UnloadMenuNextFrame()
    {
        yield return null;
        SceneLoader.Instance.UnloadScene("Scene_MainMenu");
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
