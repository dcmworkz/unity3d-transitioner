using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private int _currentlySelectedPhaseIndex = 0;
        private SerializedProperty _currentSelectedPhaseProperty = null;
        private List<string> _phaseNames = new List<string>();

        private void DisplayPhase(string name, float minWidthBigButton, int phaseIndex, int totalPhaseMemberCount)
        {
            // Creates The Phase buttons and icons.
            int capturedIndex = phaseIndex;
            GUIStyle oldButtonStyle = GUI.skin.button;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Color heldColor = GUI.backgroundColor;
            if (_currentlySelectedPhaseIndex == capturedIndex)
            {
                GUI.backgroundColor = _editorStyles.lairinusGreen;
                if (GUILayout.Button(new GUIContent(name), GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
                    HandleOnClick_SelectPhase(capturedIndex);
            }
            else if (totalPhaseMemberCount <= 0)
            {
                GUI.backgroundColor = _editorStyles.lairinusRed;
                if (GUILayout.Button(new GUIContent(name, "There are no Members inside of this Phase. To add members, click this Phase and then click the \"Edit Members\" button"), GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
                    HandleOnClick_SelectPhase(capturedIndex);
            }
            else
            {
                if (GUILayout.Button(name, GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
                    HandleOnClick_SelectPhase(capturedIndex);
            }
            GUI.backgroundColor = heldColor;
            GUI.skin.button = _editorStyles.upArrowButtonStyle;

            if (capturedIndex != 0)
            {
                if (GUILayout.Button(Helper.content_MovePhaseUp, GUILayout.Width(24), GUILayout.Height(24)))
                    HandleOnClick_MovePhaseDown(capturedIndex);
            }
            else
            {
                GUI.skin.button = null;
                GUILayout.Button("", GUILayout.Height(24), GUILayout.Width(24));
            }

            if (capturedIndex != _phasesProperty.arraySize - 1)
            {
                GUI.skin.button = _editorStyles.downArrowButtonStyle;
                if (GUILayout.Button(Helper.content_MovePhaseDown, GUILayout.Width(24), GUILayout.Height(24)))
                    HandleOnClick_MovePhaseUp(capturedIndex);
            }
            else
            {
                GUI.skin.button = null;
                GUILayout.Button("", GUILayout.Height(24), GUILayout.Width(24));
            }

            GUI.skin.button = _editorStyles.deleteButtonStyle;
            if (GUILayout.Button(Helper.content_DeletePhaseButton, GUILayout.Width(24), GUILayout.Height(24)))
                HandleOnClick_RemovePhase(capturedIndex);

            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            GUI.skin.button = oldButtonStyle;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.Space();
        }

        private void DrawPhasesPage()
        {
            if (_currentPage != Pages.Phases)
                return;

            ShowAllPhaseSettingsBox();
            DisplayMainButton(Helper.content_button_addPhase, _editorStyles.lairinusGreen, new Action(() => HandleOnClick_AddPhase()), true, null, 20);
            ShowPhaseSingleSettingsBox();
            DisplayMainButton(Helper.content_button_Back, _editorStyles.lairinusRed, new Action(() => OpenPage(Pages.Main)), true, null, 20);
        }

        private void HandleOnClick_AddPhase()
        {
            _phasesProperty.arraySize++;
            SerializedProperty phaseSingle = _phasesProperty.GetArrayElementAtIndex(_phasesProperty.arraySize - 1);
            SerializedProperty phaseName = phaseSingle.FindPropertyRelative(Helper.phaseProp_name);
            phaseName.stringValue = "New Phase";
        }

        private void HandleOnClick_MovePhaseDown(int index)
        {
            // Moves the Phase to a lower index in its' array
            if (index > 0)
                _phasesProperty.MoveArrayElement(index, index - 1);

            _currentSelectedPhaseProperty = null;
            _currentlySelectedPhaseIndex = -1;
        }

        private void HandleOnClick_MovePhaseUp(int index)
        {
            // Moves the Phase to a higher index in its' array
            if (index < _phasesProperty.arraySize - 1)
                _phasesProperty.MoveArrayElement(index, index + 1);

            _currentSelectedPhaseProperty = null;
            _currentlySelectedPhaseIndex = -1;
        }

        private void HandleOnClick_RemovePhase(int index)
        {
            if (_phasesProperty.arraySize == 1)
                index = 0;

            _phasesProperty.GetArrayElementAtIndex(index).DeleteCommand();
            _currentSelectedPhaseProperty = null;
            _phaseNames.RemoveAt(index);
            if (_currentlySelectedPhaseIndex == index)
                _currentlySelectedPhaseIndex = -1;

            serializedObject.ApplyModifiedProperties();
        }

        private void HandleOnClick_SelectPhase(int index)
        {
            int arraySize = _phasesProperty.arraySize;
            if (arraySize == 1)
                index = 0;

            _currentSelectedPhaseProperty = _phasesProperty.GetArrayElementAtIndex(index);
            _currentlySelectedPhaseIndex = index;
            GUI.FocusControl("null");
        }

        private void ShowAllPhaseSettingsBox()
        {
            List<Action> displayPhaseActions = new List<Action>();
            if (Event.current.type != EventType.Repaint)
            {
                // We gather names that exist while NOT repainting because they no longer exist during a repaint
                _phaseNames.Clear();
                for (var a = 0; a < _phasesProperty.arraySize; a++)
                {
                    int capturedCurrentIndex = a;
                    SerializedProperty serializedPhase = _phasesProperty.GetArrayElementAtIndex(capturedCurrentIndex);
                    SerializedProperty serializedPhaseName = serializedPhase.FindPropertyRelative(Helper.phaseProp_name);
                    _phaseNames.Add(serializedPhaseName.stringValue);
                }
            }

            for (var a = 0; a < _phasesProperty.arraySize; a++)
            {
                int capturedCurrentIndex = a;
                SerializedProperty serializedPhase = _phasesProperty.GetArrayElementAtIndex(capturedCurrentIndex);
                SerializedProperty serializedMembersArray = serializedPhase.FindPropertyRelative(Helper.phaseProp_reflectedMembers);
                Action phase = new Action(() => DisplayPhase(_phaseNames[capturedCurrentIndex], 100, capturedCurrentIndex, serializedMembersArray.arraySize));
                displayPhaseActions.Add(phase);
            }

            DisplaySettingBox(Helper.content_SettingsBoxTitle_Phases, displayPhaseActions.ToArray(), 20);
        }

        private void ShowPhaseSingleSettingsBox()
        {
            // Shows the currently selected phase. If no phases are shown, this is hidden.
            if (_currentSelectedPhaseProperty == null)
                return;

            SerializedProperty phaseDisabled = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_disabled);
            SerializedProperty phaseName = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_name);
            SerializedProperty phaseDelay = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_Delay);
            SerializedProperty phaseDuration = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_Duration);
            SerializedProperty lerpPlaystyleType = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_lerpPlaystyleType);

            List<Action> actions = new List<Action>();
            actions.Add(new Action(() => DisplayHorizontalProperty(phaseDisabled, Helper.content_phaseDisabled, 20, false, true)));
            actions.Add(new Action(() => DisplayHorizontalProperty(phaseName, Helper.content_phaseName, 20, true, false)));
            actions.Add(new Action(() => DisplayHorizontalProperty(phaseDelay, Helper.content_phaseDelay, 20, true, false)));
            actions.Add(new Action(() => DisplayHorizontalProperty(phaseDuration, Helper.content_phaseDuration, 20, true, false)));
            actions.Add(new Action(() => ShowLerpCurve(Helper.content_PhaseAnimationCurve, lerpPlaystyleType, phaseDuration, phaseDelay)));
            actions.Add(new Action(() => DisplayMainButton(Helper.content_button_EditProperties, _editorStyles.lairinusGreen, new Action(() => OpenPage(Pages.PropertyManager)), true, null, 20)));

            DisplaySettingBox(Helper.content_SettingsBoxTitle_PhaseOptions, actions.ToArray(), 20);
        }
    }
}