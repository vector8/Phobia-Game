using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class FlipTiles : EditorWindow
{
    [MenuItem("Custom Windows/Flip Tiles")]
    public static void showWindow()
    {
        EditorWindow.GetWindow(typeof(FlipTiles));
    }

    void OnGUI()
    {

        if (GUILayout.Button("Flip Selected Tiles"))
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                Vector3 rot = go.transform.localEulerAngles;
                rot.z = 90;
                go.transform.localEulerAngles = rot;
            }
        }
    }
}