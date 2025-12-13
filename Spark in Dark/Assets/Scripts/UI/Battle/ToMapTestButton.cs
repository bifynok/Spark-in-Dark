using UnityEngine;
using System.Collections;

public class ToMapTestButton : MonoBehaviour
{
    public void ToMap()
    {
        SceneLoader.Instance.LoadSceneAdditive("Scene_Map", true, () =>
        {
            var mapManager = Object.FindFirstObjectByType<MapManager>();
            mapManager.OnSceneActivated();

            StartCoroutine(UnloadBattleNextFrame());
        });
    }

    private IEnumerator UnloadBattleNextFrame()
    {
        yield return null;
        SceneLoader.Instance.UnloadScene("Scene_Battle");
    }
}
