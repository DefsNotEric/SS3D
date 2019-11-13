using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Turf))]
public class Turf_GUI : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Turf targetScript = (Turf)target;
        if(GUILayout.Button("build_turf"))
        {
            targetScript.BuildTurf();
        }
        if(GUILayout.Button("update_turf"))
        {
            targetScript.UpdateTurf();
        }
    }
}
