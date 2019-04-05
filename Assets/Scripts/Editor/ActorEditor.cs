using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(Actor))]
public class ActorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Actor myActor = target as Actor;

        //Actor name can not be empty, default to name of game object
        myActor.actorName = EditorGUILayout.TextField("Actor Name", myActor.actorName);
        if(myActor.actorName == "")
        {
            myActor.actorName = myActor.name;
        }

        EditorGUILayout.Space();

        //Max HP must be > 0
        int newHP = EditorGUILayout.DelayedIntField("Max HP", myActor.maxHitPoints);
        if(newHP > 0)
        {
            myActor.maxHitPoints = newHP;
        }

        //Initiative must be between 10 and 100 in steps of 5
        const int STEP = 5;
        myActor.initiative = STEP * (EditorGUILayout.IntSlider("Initiative", myActor.initiative, 10, 100) / STEP);

        //Immunities
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Immunities", GUILayout.MinWidth(100));
        EditorGUILayout.BeginVertical();
        EditorGUILayout.ToggleLeft("Test", false);
        EditorGUILayout.ToggleLeft("Test2", true);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.ToggleLeft("Test3", true);
        EditorGUILayout.ToggleLeft("Test4", false);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        EditorGUILayout.ToggleLeft("Test5", true);
        EditorGUILayout.ToggleLeft("Test6", false);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}
