using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

public partial class UITransitionerEditor : Editor
{
    private int _currentlySelectedPhaseIndex = 0;
    private SerializedProperty _currentSelectedPhaseProperty = null;
    private List<string> _phaseNames = new List<string>();

    private void DisplayPhase(string name, float minWidthBigButton, int phaseIndex)
    {
        GUIStyle oldButtonStyle = GUI.skin.button;

        int capturedIndex = phaseIndex;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        Color heldColor = GUI.backgroundColor;
        Color newColor = new Color();
        if (_currentlySelectedPhaseIndex == phaseIndex)
        {
            ColorUtility.TryParseHtmlString("#00aa00", out newColor);
            GUI.backgroundColor = newColor;
            if (GUILayout.Button(name, GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
            {
                SelectPhase(capturedIndex);
            }
        }
        else
        {
            if (GUILayout.Button(name, GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
            {
                SelectPhase(capturedIndex);
            }
        }
        GUI.backgroundColor = heldColor;
        GUI.skin.button = _editorStyles.upArrowButtonStyle;

        if (phaseIndex != _phasesProperty.arraySize - 1)
        {
            if (GUILayout.Button(new GUIContent("", "Move this phase one up"), GUILayout.Width(24), GUILayout.Height(24)))
            {
                HandleOnClick_MovePhaseUp(capturedIndex);
            }
        }
        else
        {
            GUI.skin.button = null;
            GUILayout.Button("", GUILayout.Height(24), GUILayout.Width(24));
        }

        if (phaseIndex != 0)
        {
            GUI.skin.button = _editorStyles.downArrowButtonStyle;
            if (GUILayout.Button(new GUIContent("", "Move this Phase one down"), GUILayout.Width(24), GUILayout.Height(24)))
            {
                HandleOnClick_MovePhaseDown(capturedIndex);
            }
        }
        else
        {
            GUI.skin.button = null;
            GUILayout.Button("", GUILayout.Height(24), GUILayout.Width(24));
        }

        GUI.skin.button = _editorStyles.deleteButtonStyle;
        if (GUILayout.Button(new GUIContent("", "Deletes the phase"), GUILayout.Width(24), GUILayout.Height(24)))
        {
            HandleOnClick_RemovePhase(capturedIndex);
        }
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
        DisplayMainButton(new GUIContent("Add Phase", "Creates a new Phase"), _editorStyles.lairinusGreen, new Action(() => HandleOnClick_AddPhase()), false, null, 20);
        ShowPhaseSingleSettingsBox();
        DisplayMainButton(new GUIContent("Back"), _editorStyles.lairinusRed, new Action(() => OpenPage(Pages.Main)), true, null, 20);
    }

    private void HandleOnClick_AddPhase()
    {
        _phasesProperty.arraySize++;
    }

    private void HandleOnClick_MovePhaseDown(int index)
    {
        if (index > 0)
            _phasesProperty.MoveArrayElement(index, index - 1);

        _currentSelectedPhaseProperty = null;
        _currentlySelectedPhaseIndex = -1;
    }

    private void HandleOnClick_MovePhaseUp(int index)
    {
        if (index < _phasesProperty.arraySize - 1)
            _phasesProperty.MoveArrayElement(index, index + 1);

        _currentSelectedPhaseProperty = null;
        _currentlySelectedPhaseIndex = -1;
    }

    private void HandleOnClick_OpenComponentSelect()
    {
        _currentPage = Pages.PropertyManager;
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

    private void SelectPhase(int index)
    {
        int arraySize = _phasesProperty.arraySize;
        if (arraySize == 1)
            index = 0;
        _currentSelectedPhaseProperty = _phasesProperty.GetArrayElementAtIndex(index);
        _currentlySelectedPhaseIndex = index;
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
                SerializedProperty serializedPhaseName = serializedPhase.FindPropertyRelative("_sf_name");
                _phaseNames.Add(serializedPhaseName.stringValue);
            }
        }

        for (var a = 0; a < _phasesProperty.arraySize; a++)
        {
            int capturedCurrentIndex = a;
            Action phase = new Action(() => DisplayPhase(_phaseNames[capturedCurrentIndex], 100, capturedCurrentIndex));
            displayPhaseActions.Add(phase);
        }

        DisplaySettingBox("Phases", displayPhaseActions.ToArray(), 20);
    }

    private void ShowPhaseSingleSettingsBox()
    {
        // Shows the currently selected phase. If no phases are shown, this is hidden.
        if (_currentSelectedPhaseProperty == null)
            return;

        SerializedProperty phaseEnabled = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_enabled");
        SerializedProperty phaseName = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_name");
        SerializedProperty phaseDelay = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_delay");
        SerializedProperty phaseDuration = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_duration");
        SerializedProperty lerpPlaystyleType = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_lerpPlaystyleType");

        List<Action> actions = new List<Action>();
        actions.Add(new Action(() => DisplayHorizontalProperty(phaseEnabled, new GUIContent("Enabled"), 20, false, true)));
        actions.Add(new Action(() => DisplayHorizontalProperty(phaseName, new GUIContent("Name"), 20, true, false)));
        actions.Add(new Action(() => DisplayHorizontalProperty(phaseDelay, new GUIContent("Delay"), 20, true, false)));
        actions.Add(new Action(() => DisplayHorizontalProperty(phaseDuration, new GUIContent("Duration"), 20, true, false)));
        actions.Add(new Action(() => DisplayHorizontalProperty(lerpPlaystyleType, new GUIContent("Animation Lerp"), 20, true, false, 50)));
        actions.Add(new Action(() => DisplayMainButton(new GUIContent("Edit Properties", "Allows you to add Properties and Fields from this component to lerp or set"), _editorStyles.lairinusGreen, new Action(() => HandleOnClick_OpenComponentSelect()), true, null, 20)));

        DisplaySettingBox("Phase Settings", actions.ToArray(), 20);
    }
}