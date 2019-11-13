using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StationLattice))]
public class Lattice_GUI : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        StationLattice targetScript = (StationLattice)target;
        if(GUILayout.Button("update_turf"))
        {
            targetScript.UpdateTurf();
        }
        if(GUILayout.Button("add_plating"))
        {
            targetScript.AddPlating();
        }
        if(GUILayout.Button("remove_plating"))
        {
            targetScript.RemovePlating();
        }
        if(GUILayout.Button("build_wall"))
        {
            targetScript.AddWallGirder();
        }
        if(GUILayout.Button("remove_wall"))
        {
            targetScript.RemoveWallGirder();
        }
        if(GUILayout.Button("upgrade_wall"))
        {
            targetScript.UpgradeWall(StationLattice.StationWallTypes.reinforced);
        }
        if(GUILayout.Button("downgrade_wall"))
        {
            targetScript.DowngradeWall();
        }
        if(GUILayout.Button("build_floor"))
        {
            targetScript.AddFloor();
        }
        if(GUILayout.Button("remove_floor"))
        {
            targetScript.RemoveFloor();
        }
    }
}
