using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

[CustomEditor(typeof(TentacleGenerator))]
public class PointSerializer : Editor
{
    private bool updateInspector = false;
    private void OnEnable()
    {
        // Register to the update loop
        EditorApplication.update += UpdateInspector;
    }

    private void OnDisable()
    {
        // Unregister from the update loop
        EditorApplication.update -= UpdateInspector;
    }
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();

        TentacleGenerator tentacleGenerator = (TentacleGenerator)target;
        updateInspector = EditorGUILayout.Toggle("Update Inspector", updateInspector);
        try
        {
            foreach(TentacleGenerator.Segment segment in tentacleGenerator.segments)
            {

                EditorGUILayout.LabelField("Read-Only Text", segment.pointA.currentPosition + ":" + segment.pointB.currentPosition);

            }

        } catch (NullReferenceException)
        {

        }


    }

    void UpdateInspector()
    {
        if (updateInspector) Repaint();
    }



}
