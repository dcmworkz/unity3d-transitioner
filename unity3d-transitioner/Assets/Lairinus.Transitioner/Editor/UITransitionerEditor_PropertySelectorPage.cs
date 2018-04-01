using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public partial class UITransitionerEditor : Editor
{
    private void DrawPropertySelectorPage()
    {
        if (_currentPage != Pages.PropertySelector)
            return;

        EditorGUILayout.LabelField("Property Selection Page");
    }
}