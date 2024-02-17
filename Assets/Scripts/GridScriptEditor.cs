using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(GridScript))]
public class GridScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
            ((GridScript)target).Generate();
        if (GUILayout.Button("Refresh"))
            ((GridScript)target).Refresh();
    }
}
#endif