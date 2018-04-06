using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private void DrawMainPage()
        {
            if (_currentPage != Pages.Main && _targetGameObject != null)
                return;

            DrawBasicSettings();
        }

        private void DrawBasicSettings()
        {
            GUIContent targetGameObjectContent = new GUIContent("GameObject", "The GameObject that will handle all of the Transitions and Phases");
            GUIContent transitionEnabledContent = new GUIContent("Disable", "Determines if whether the current Transition can be processed or not");
            GUIContent transitionLoopedContent = new GUIContent("Loop", "If enabled, the transition will continuously play until it is manually stopped, or this property gets disabled");

            Action targetGOProperty = () => DisplayHorizontalProperty(_targetGameObject, targetGameObjectContent, 20, false, true);
            Action enableTransitionProperty = () => DisplayHorizontalProperty(disableTransition, transitionEnabledContent, 20, false, true);
            Action loopProperty = () => DisplayHorizontalProperty(_loop, transitionLoopedContent, 20, false, false);
            Action[] actions = { targetGOProperty, enableTransitionProperty, loopProperty };
            DisplaySettingBox("Basic Settings", actions, 20);
            GUILayout.Space(20);
            DisplayMainButton(new GUIContent("Modify Phases"), _editorStyles.lairinusGreen, new Action(() => OpenPage(Pages.Phases)));
        }

        private void DisplaySettingBox(string title, Action[] propertyActions, float space, float preSpace = 0, float postSpace = 0)
        {
            GUILayout.Space(preSpace);
            GUILayout.Space(space);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(Screen.width * 0.8F));
            GUILayout.Space(space);

            foreach (Action action in propertyActions)
            {
                if (action != null)
                    action();
            }

            GUILayout.Space(space);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
            GUILayout.Space(space);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(postSpace);
        }

        private void DisplayMainButton(GUIContent content, Color buttonColor, Action onClick, bool isEnabled = true, Action disabledOnClick = null, float paddingTop = 0, float paddingBottom = 0)
        {
            if (paddingTop > 0)
                GUILayout.Space(paddingTop);

            Color heldColor = GUI.backgroundColor;
            GUI.backgroundColor = buttonColor;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(content, GUILayout.Width(150), GUILayout.Height(40)))
            {
                if (onClick != null && isEnabled)
                    onClick();
                else if (!isEnabled && disabledOnClick != null)
                    disabledOnClick();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUI.backgroundColor = heldColor;

            if (paddingBottom > 0)
                GUILayout.Space(paddingBottom);
        }
    }
}