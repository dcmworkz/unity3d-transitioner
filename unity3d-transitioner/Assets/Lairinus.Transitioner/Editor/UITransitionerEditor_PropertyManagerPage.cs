using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private void DrawPropertyManagerPage()
        {
            if (_currentPage != Pages.PropertyManager)
                return;

            Action openPropertySelectionPage = new Action(() => OpenPage(Pages.PropertySelector));
            DisplayMainButton(new GUIContent("Add Members", "Adds more fields and/or properties to this phase"), _editorStyles.lairinusGreen, openPropertySelectionPage, true, null, 20, 20);

            Action openPage = new Action(() => OpenPage(Pages.Phases));
            DisplayMainButton(new GUIContent("Back", "Returns to the Phases page"), _editorStyles.lairinusRed, openPage, true, null, 20, 20);
        }
    }
}