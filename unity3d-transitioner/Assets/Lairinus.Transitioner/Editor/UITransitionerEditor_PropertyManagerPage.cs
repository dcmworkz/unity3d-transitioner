using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;

public partial class UITransitionerEditor : Editor
{
    private List<Action> _componentActions = new List<Action>();

    private void DrawPropertyManagerPage()
    {
        if (_currentPage != Pages.PropertyManager)
            return;

        DisplaySettingBox("Components on GameObject", _componentActions.ToArray(), 20);

        Action openPage = new Action(() => OpenPage(Pages.Phases));
        DisplayMainButton(new GUIContent("Back", "Returns to the Phases page"), _editorStyles.lairinusRed, openPage, true, null, 20, 20);
    }

    private void InitializePropertyManagerPage()
    {
        UITransitioner uiTransition = (UITransitioner)target;
        Component[] components = uiTransition.gameObject.GetComponents(typeof(Component));
        _componentActions.Clear();
        foreach (Component c in components)
        {
            if (c.GetType() == typeof(UITransitioner))
                continue;

            Action componentAction = () => DisplayComponentSelectUI(c);
            _componentActions.Add(componentAction);

            foreach (FieldInfo componentFieldInfo in c.GetType().GetFields())
            {
                Debug.Log(componentFieldInfo.Name);
            }

            foreach (PropertyInfo componentPropertyInfo in c.GetType().GetProperties())
            {
                if (componentPropertyInfo.CanRead && componentPropertyInfo.CanWrite)
                    Debug.Log(componentPropertyInfo.Name);
            }
        }
    }

    private void DisplayComponentSelectUI(Component reflectedComponent)
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUIContent guiContent = new GUIContent(reflectedComponent.GetType().FullName);
        if (GUILayout.Button(guiContent))
        {
        }
        GUILayout.Space(20);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}