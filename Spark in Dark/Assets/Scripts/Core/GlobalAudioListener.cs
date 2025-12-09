using UnityEngine;

public class GlobalAudioListener : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
