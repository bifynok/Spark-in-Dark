using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        SceneLoader.Instance.LoadSceneAdditive("Scene_MainMenu", true, () =>
        {
            SceneLoader.Instance.UnloadScene("Scene_Bootstrap");
        });
    }
}
