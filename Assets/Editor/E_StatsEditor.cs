using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SO_EntityStats))]
public class E_StatsEditor : Editor
{
    private const string SCRIPT_PROP = "m_Script";

    private const string DASH_WINDUP_PROP = "_dashWindUpTime";
    private const string LOCKIN_DIRECTION_PROP = "_lockInDirectionPercent";
    private const string DASH_DISTANCE_PROP = "_dashDistance";
    private const string DASH_DURATION_PROP = "_dashDuration";
    private const string END_DASH_COOLDOWN_PROP = "_endDashCooldown";
    private const string DASH_ANIMATIONCURVE_PROP = "_dashAnimationCurve";

    private SerializedProperty scriptProp;


    private SO_EntityStats script;

    private void OnEnable()
    {
        scriptProp = serializedObject.FindProperty(SCRIPT_PROP);

        script = (SO_EntityStats)target;
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(scriptProp);
        GUI.enabled = true;

        switch (script.EntityType)
        {
            case EntityType.PopCorn:
                DrawPropertiesExcluding(serializedObject, SCRIPT_PROP);
                break;
            case EntityType.Elite:
            case EntityType.Boss:
            case EntityType.Player:
                DrawPropertiesExcluding(serializedObject, SCRIPT_PROP, DASH_WINDUP_PROP, LOCKIN_DIRECTION_PROP, DASH_DISTANCE_PROP, DASH_DURATION_PROP, END_DASH_COOLDOWN_PROP, DASH_ANIMATIONCURVE_PROP);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
