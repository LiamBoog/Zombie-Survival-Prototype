using UnityEditor;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        if (!Application.isEditor)
            Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}
