using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public partial class UITransitionerEditor : Editor
{
    private void DrawPropertyManagerPage()
    {
        if (_currentPage != Pages.PropertyManager)
            return;

        EditorGUILayout.LabelField("Property Manager Page");
    }
}