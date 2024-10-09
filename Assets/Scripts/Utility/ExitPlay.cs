#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ExitPlay
{
    static ExitPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Debug.Log("Exiting playmode.");
            GameObject go = GameObject.Find("WorldGeneration");
            go.GetComponent<World>().Clear();
        }
    }
}
#endif