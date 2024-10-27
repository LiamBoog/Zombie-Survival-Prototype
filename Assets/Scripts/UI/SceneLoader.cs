using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneAsset scene;

    public void LoadScene()
    {
        SceneManager.LoadScene(scene.name, LoadSceneMode.Single);
    }
}
