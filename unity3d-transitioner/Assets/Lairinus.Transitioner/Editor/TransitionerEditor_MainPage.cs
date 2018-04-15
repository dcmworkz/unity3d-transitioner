using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

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
            UnityEngine.Object oldObjectValue = _targetGameObject.objectReferenceValue;
            List<Action> basicSettingsArray = new List<Action>();
            basicSettingsArray.Add(() => DisplayHorizontalProperty(_targetGameObject, Helper.content_targetGameObject, 20, false, true));
            basicSettingsArray.Add(() => DisplayHorizontalProperty(disableTransition, Helper.content_disableTransition, 20, false, true));
            basicSettingsArray.Add(() => DisplayHorizontalProperty(_loop, Helper.content_loopTransition, 20, false, true));
            basicSettingsArray.Add(() => DisplayHorizontalProperty(_phaseProperty_PlayOnAwake, Helper.content_playOnAwake, 20, false, false));
            DisplaySettingBox(Helper.content_SettingsBoxTitle_BasicSettings, basicSettingsArray.ToArray(), 20);

            UnityEngine.Object newObjectValue = _targetGameObject.objectReferenceValue;
            if (oldObjectValue != null && oldObjectValue != newObjectValue)
            {
                if (EditorUtility.DisplayDialog("Confirm changing 'Target GameObject'", "Are you sure that you wish to change the target GameObject? This will reset all of the Phases attached to this Transitioner", "Yes", "Cancel"))
                {
                    _currentlySelectedPhaseIndex = 0;
                    _currentSelectedPhaseProperty = null;
                    _allReflectedPhaseMembers = new List<PhaseMember>();
                    _phasesProperty.arraySize = 0;
                }
                else
                    _targetGameObject.objectReferenceValue = oldObjectValue;
            }

            GUILayout.Space(20);
            DisplayMainButton(Helper.content_mainButton_modifyPhases, _editorStyles.lairinusGreen, new Action(() => OpenPage(Pages.Phases)));
        }

        private void DisplaySettingBox(GUIContent titleContent, Action[] propertyActions, float space, float preSpace = 0, float postSpace = 0)
        {
            GUILayout.Space(preSpace);
            GUILayout.Space(space);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(titleContent, EditorStyles.boldLabel);
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