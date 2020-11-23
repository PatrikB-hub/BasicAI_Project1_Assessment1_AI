using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Menu : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#endif
        Application.Quit();
    }
}
