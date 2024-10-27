using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField, HideInInspector] private string sceneName; 
    
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset scene;

    private void OnValidate()
    {
        sceneName = scene.name;
    }
#endif

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
