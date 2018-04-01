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

    private Texture2D _textureUpArrow = null;
    private Texture2D _textureDownArrow = null;

    private Pages _lastPage = Pages.Main;
    private Pages _currentPage = Pages.Main;
    private SerializedProperty _targetGameObject = null;
    private SerializedProperty _enableTransition = null;
    private SerializedProperty _loop = null;
    private SerializedProperty _phasesProperty = null;
    private TransitionerEditorStyles _editorStyles = null;
    private GUIStyle downArrowButton = new GUIStyle();
    private GUIStyle upArrowButton = new GUIStyle();

    private string normalButtonColorHex = "#50C878";
    private string backButtonColorHex = "#B34B62";
    private Color normalButtonColor;
    private Color backButtonColor;

    private void SetPage(Pages newPage)
    {
        _lastPage = _currentPage;
        _currentPage = newPage;
    }

    private void OnEnable()
    {
        _editorStyles = Resources.Load<TransitionerEditorStyles>("TransitionerEditorStyles");
        if (_editorStyles == null)
            Debug.LogWarning("Editor Skin is null! Did you remove the Resources folder??");

        InitializeColors();
        InitializeProperties();
    }

    private void InitializeColors()
    {
        ColorUtility.TryParseHtmlString(normalButtonColorHex, out normalButtonColor);
        ColorUtility.TryParseHtmlString(backButtonColorHex, out backButtonColor);
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
        GUISkin oldSkin = GUI.skin;
        GUI.skin = _editorStyles.editorSkin;
        serializedObject.Update();
        DrawMainPage();
        DrawPhasesPage();
        DrawPropertyManagerPage();
        DrawPropertySelectorPage();
        serializedObject.ApplyModifiedProperties();
        GUI.skin = oldSkin;
    }

    private void OnClick_ShowPhasesPage()
    {
        _currentPage = Pages.Phases;
        _lastPage = Pages.Main;
    }

    private void OnHandleClick_GoBack()
    {
        Pages lastPage = _lastPage;
        _lastPage = _currentPage;
        _currentPage = lastPage;
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