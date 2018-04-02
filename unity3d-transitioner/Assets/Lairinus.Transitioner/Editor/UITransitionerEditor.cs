using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(UITransitioner))]
public partial class UITransitionerEditor : Editor
{
    private enum Pages
    {
        Main,
        Phases,
        PropertyManager,
        PropertySelector
    }

    private Pages _currentPage = Pages.Main;
    private SerializedProperty _targetGameObject = null;
    private SerializedProperty _enableTransition = null;
    private SerializedProperty _loop = null;
    private SerializedProperty _phasesProperty = null;
    private TransitionerEditorStyles _editorStyles = null;

    private void OnEnable()
    {
        _editorStyles = Resources.Load<TransitionerEditorStyles>("TransitionerEditorStyles");
        InitializeProperties();
    }

    private void InitializeProperties()
    {
        _targetGameObject = serializedObject.FindProperty("_sf_targetGameObject");
        _enableTransition = serializedObject.FindProperty("_sf_enableTransition");
        _loop = serializedObject.FindProperty("_sf_loop");
        _phasesProperty = serializedObject.FindProperty("_sf_phases");
    }

    private void OnDisable()
    {
    }

    public override void OnInspectorGUI()
    {
        try
        {
            GUISkin oldSkin = GUI.skin;
            if (_editorStyles != null)
                GUI.skin = _editorStyles.editorSkin;

            serializedObject.Update();
            DrawMainPage();
            DrawPhasesPage();
            DrawPropertyManagerPage();
            DrawPropertySelectorPage();
            serializedObject.ApplyModifiedProperties();
            GUI.skin = oldSkin;
        }
        catch
        {
        }
    }

    private void OpenPage(Pages newCurrentPage)
    {
        _currentPage = newCurrentPage;
        if (newCurrentPage == Pages.PropertyManager)
            InitializePropertyManagerPage();
    }

    private void DisplayHorizontalProperty(SerializedProperty property, GUIContent content, float padding, bool includeSpaceBefore = false, bool includeSpaceAfter = false, float customHeight = -1)
    {
        if (includeSpaceBefore)
            EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(padding);

        if (customHeight < 0)
            EditorGUILayout.PropertyField(property, content);
        else
            EditorGUILayout.PropertyField(property, content, GUILayout.Height(customHeight));

        GUILayout.Space(padding);
        EditorGUILayout.EndHorizontal();

        if (includeSpaceAfter)
            EditorGUILayout.Space();
    }
}