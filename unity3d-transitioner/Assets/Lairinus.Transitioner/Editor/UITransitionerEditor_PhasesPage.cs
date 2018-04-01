using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;

public partial class UITransitionerEditor : Editor
{
    private SerializedProperty _currentSelectedPhaseProperty = null;
    private int _currentlySelectedPhaseIndex = 0;

    private void DrawPhasesPage()
    {
        if (_currentPage != Pages.Phases)
            return;

        ShowAllPhaseSettingsBox();

        GUILayout.Space(20);
        DisplayMainButton(new GUIContent("Add Phase", "Creates a new Phase"), normalButtonColor, new Action(() => HandleOnClick_AddPhase()));

        ShowPhaseSingleSettingsBox();

        GUILayout.Space(20);
        DisplayMainButton(new GUIContent("Back"), backButtonColor, new Action(() => OnHandleClick_GoBack()));
    }

    private void DisplayPhase(string name, float minWidthBigButton, int phaseIndex)
    {
        GUIStyle oldButtonStyle = GUI.skin.button;

        int capturedIndex = phaseIndex;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(20);
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
        if (GUILayout.Button(name, GUILayout.MinWidth(minWidthBigButton), GUILayout.Height(24)))
        {
            SelectPhase(capturedIndex);
        }
        GUI.skin.button.alignment = TextAnchor.MiddleCenter;
        GUI.skin.button = _editorStyles.upArrowButtonStyle;
        if (GUILayout.Button("", GUILayout.Width(24), GUILayout.Height(24)))
        {
            HandleOnClick_MovePhaseUp(capturedIndex);
        }

        GUI.skin.button = _editorStyles.downArrowButtonStyle;
        if (GUILayout.Button("", GUILayout.Width(24), GUILayout.Height(24)))
        {
            HandleOnClick_MovePhaseDown(capturedIndex);
        }
        GUILayout.Space(20);
        EditorGUILayout.EndHorizontal();
        GUI.skin.button = oldButtonStyle;
        EditorGUILayout.Space();
    }

    private void HandleOnClick_AddPhase()
    {
        _phasesProperty.arraySize++;
    }

    private void SelectPhase(int index)
    {
        int arraySize = _phasesProperty.arraySize;
        if (arraySize == 1)
            index = 0;
        _currentSelectedPhaseProperty = _phasesProperty.GetArrayElementAtIndex(index);
        _currentlySelectedPhaseIndex = index;
    }

    private void HandleOnClick_MovePhaseUp(int index)
    {
    }

    private void HandleOnClick_MovePhaseDown(int index)
    {
    }

    private void ShowAllPhaseSettingsBox()
    {
        // Phases Settings Box - Add foreach to iterate through all phases...
        List<Action> displayPhaseActions = new List<Action>();
        for (var a = 0; a < _phasesProperty.arraySize; a++)
        {
            int captured = a;
            SerializedProperty currentElement = _phasesProperty.GetArrayElementAtIndex(captured);
            SerializedProperty phaseNameProperty = currentElement.FindPropertyRelative("_sf_name");
            Action phase = new Action(() => DisplayPhase(phaseNameProperty.stringValue, 100, captured));
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
        actions.Add(new Action(() => DisplayHorizontalProperty(lerpPlaystyleType, new GUIContent("Lerp Playstyle"), 20, true, false, 50)));
        actions.Add(new Action(() => DisplayMainButton(new GUIContent("Remove Phase"), backButtonColor, new Action(() => HandleOnClick_RemoveCurrentPhase()))));

        DisplaySettingBox("Phase Settings", actions.ToArray(), 20);
    }

    private void HandleOnClick_RemoveCurrentPhase()
    {
        _phasesProperty.DeleteArrayElementAtIndex(_currentlySelectedPhaseIndex);
        _currentSelectedPhaseProperty = null;
    }
}