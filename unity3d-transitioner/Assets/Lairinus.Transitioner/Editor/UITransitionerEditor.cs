using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(UITransitioner))]
public class UITransitionerEditor : Editor
{
    private AnimationCurve curve = new AnimationCurve();

    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        curve = EditorGUI.CurveField(new Rect(10, 10, 100, 100), curve);

        serializedObject.ApplyModifiedProperties();
    }
}