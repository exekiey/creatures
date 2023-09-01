using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SeekingScript))]
public class SeekingEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SeekingScript seekingScript = (SeekingScript)target;


        if (!seekingScript.followCursor )
        {

            seekingScript.tarjetObject = EditorGUILayout.ObjectField("Target Object", seekingScript.tarjetObject, typeof(GameObject), true) as GameObject;

        }

        serializedObject.ApplyModifiedProperties();

    }

}
