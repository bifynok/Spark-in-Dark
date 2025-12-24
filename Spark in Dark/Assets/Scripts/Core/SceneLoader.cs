using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private readonly HashSet<string> loadingScenes = new();
    
    private readonly HashSet<string> unloadingScenes = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (Instance != null) return;

        var sceneLoaderGO = new GameObject("SceneLoader");
        Instance = sceneLoaderGO.AddComponent<SceneLoader>();
        DontDestroyOnLoad(sceneLoaderGO);
    }

    public void LoadSceneAdditive(string sceneName, bool setActive = false, Action onLoaded = null)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, setActive, onLoaded));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, bool setActive, Action onLoaded)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.isLoaded || loadingScenes.Contains(sceneName))
        {
            if (setActive && scene.isLoaded)
                SceneManager.SetActiveScene(scene);

            onLoaded?.Invoke();
            yield break;
        }

        loadingScenes.Add(sceneName);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        if (setActive)
        {
            scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        loadingScenes.Remove(sceneName);

        onLoaded?.Invoke();
    }

    public void UnloadScene(string sceneName, Action onUnloaded = null)
    {
        StartCoroutine(UnloadSceneCoroutine(sceneName, onUnloaded));
    }

    private IEnumerator UnloadSceneCoroutine(string sceneName, Action onUnloaded)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (!scene.isLoaded || unloadingScenes.Contains(sceneName))
        {
            onUnloaded?.Invoke();
            yield break;
        }

        unloadingScenes.Add(sceneName);

        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);
        while (!asyncUnload.isDone)
            yield return null;

        unloadingScenes.Remove(sceneName);

        onUnloaded?.Invoke();
    }

    public void LoadOverlay(string sceneName, Action onLoaded = null)
    {
        LoadSceneAdditive(sceneName, setActive: false, onLoaded);
    }

    public void SwitchScene(string newScene, string oldScene)
    {
        LoadSceneAdditive(newScene, setActive: true, () =>
        {
            StartCoroutine(EnterAndUnloadCoroutine(newScene, oldScene));
        });
    }

    private void EnterScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        foreach (var root in scene.GetRootGameObjects())
        {
            foreach (var entry in root.GetComponentsInChildren<ISceneEntryPoint>(true))
            {
                entry.OnSceneEnter();
            }
        }
    }

    private IEnumerator EnterAndUnloadCoroutine(string newScene, string oldScene)
    {
        EnterScene(newScene);

        yield return null;

        UnloadScene(oldScene);
    }
}
