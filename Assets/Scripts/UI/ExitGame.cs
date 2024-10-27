using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        if (!Application.isEditor)
            Application.Quit();

        EditorApplication.isPlaying = false;
    }
}
