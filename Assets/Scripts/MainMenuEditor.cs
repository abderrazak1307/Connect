using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MainMenu))]
public class MainMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Update")){
            ((MainMenu)target).Generate();
        }
    }
}
#endif