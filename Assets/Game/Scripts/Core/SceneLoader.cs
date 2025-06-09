using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosition;

    public async void LoadScene(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        GameObject[] rootObjects = loadedScene.GetRootGameObjects();

        foreach (var obj in rootObjects)
        {
            if (obj.name == "Root")
            {
                obj.transform.position = _spawnPosition.position;
                break;
            }
        }
    }
}