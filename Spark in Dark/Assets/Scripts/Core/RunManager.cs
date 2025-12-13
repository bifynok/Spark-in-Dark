using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public RunData CurrentRun { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance != null)
            return;

        var go = new GameObject("RunManager");
        Instance = go.AddComponent<RunManager>();
        DontDestroyOnLoad(go);
    }

    public bool HasActiveRun => CurrentRun != null;

    public void StartNewRun()
    {
        CurrentRun = new RunData();
    }

    public void EndRun()
    {
        CurrentRun = null;
    }
}
