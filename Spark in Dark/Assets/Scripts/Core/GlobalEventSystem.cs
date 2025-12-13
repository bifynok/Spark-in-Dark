using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class GlobalEventSystem : MonoBehaviour
{
    private static bool created;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (created) return;

        var eventSystemGO = new GameObject("EventSystem");

        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<InputSystemUIInputModule>();

        Object.DontDestroyOnLoad(eventSystemGO);
        created = true;
    }
}
