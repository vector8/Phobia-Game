using UnityEngine;
using UnityEditor;
using CitaNet;

public class CitaNetConfig : EditorWindow
{
    [MenuItem ("CitaNet/Config")]
    public static void showWindow()
    {
        EditorWindow.GetWindow(typeof(CitaNetConfig));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Assign Network IDs"))
        {
            NetworkedObject[] netObjs = Resources.FindObjectsOfTypeAll<NetworkedObject>();
            Undo.RecordObjects(netObjs, "Assign Network IDs");

            int currentID = 0;
            foreach(NetworkedObject n in netObjs)
            {
                if(!EditorUtility.IsPersistent(n.gameObject))
                {
                    n.networkID = currentID;
                    currentID++;
                }
            }
        }
    }
}
