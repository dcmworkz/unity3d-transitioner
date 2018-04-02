using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

public partial class UITransitionerEditor : Editor
{
    private void DrawPropertyManagerPage()
    {
        if (_currentPage != Pages.PropertyManager)
            return;

        Action openPage = new Action(() => OpenPage(Pages.Phases));
        DisplayMainButton(new GUIContent("Back", "Returns to the Phases page"), _editorStyles.lairinusRed, openPage, true, null, 20, 20);
    }
}