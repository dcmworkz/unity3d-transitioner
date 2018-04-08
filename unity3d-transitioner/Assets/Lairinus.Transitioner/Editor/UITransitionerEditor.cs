using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
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
        private SerializedProperty disableTransition = null;
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
            disableTransition = serializedObject.FindProperty("_sf_disableTransition");
            _loop = serializedObject.FindProperty("_sf_loop");
            _phasesProperty = serializedObject.FindProperty("_sf_phases");
        }

        private string GetStringTypeFromComponentProperty(SerializedProperty componentProperty)
        {
            try
            {
                return componentProperty.objectReferenceValue.GetType().ToString();
            }
            catch
            {
                return "*Invalid Type*";
            }
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
                DrawPage_PropertySelector();
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
            if (newCurrentPage == Pages.PropertySelector)
                InitializePropertySelectorPage();
            if (newCurrentPage == Pages.Phases)
            {
                _currentlySelectedPhaseIndex = -1;
                _currentSelectedPhaseProperty = null;
            }
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

        private static class Helper
        {
            // Phases -> Properties
            public const string phaseProp_Delay = "_sf_delay";

            public const string phaseProp_Duration = "_sf_duration";

            // Phase Members -> Properties
            public const string phaseMemberProp_availableMemberType = "_sf_serializedPropertyType";

            public const string phaseMemberProp_memberName = "_sf_memberName";
            public const string phaseMemberProp_parentComponent = "_sf_parentComponent";
            public const string phaseMemberProp_isDisabled = "_sf_isDisabled";
            public const string phaseMemberProp_canBeLerped = "_sf_canBeLerped";
            public const string phaseMemberProp_memberValueString = "_sf_memberValueString";
            public const string phaseMemberProp_useSeparateAnimationCurve = "_sf_useSeparateAnimationCurve";
            public const string phaseMemberProp_animationCurve = "_sf_animationCurve";

            // GUI Contents
            public static GUIContent content_getValueButton = new GUIContent("Get", "Applies the value found on the object to this Phase Member");

            public static GUIContent content_setValueButton = new GUIContent("Set", "Updates the object's values based on this Phase Member");
            public static GUIContent content_disableTransition = new GUIContent("Disable Transition", "Determines if whether the current Transition can be processed or not");
            public static GUIContent content_targetGameObject = new GUIContent("Target GameObject", "The GameObject that will handle all of the Transitions and Phases");
            public static GUIContent content_loopTransition = new GUIContent("Loop", "If enabled, the transition will continuously play until it is manually stopped, or this property gets disabled");
            public static GUIContent content_mainButton_modifyPhases = new GUIContent("Modify Phases", "Allows you to modify the Phases inside of this transition.");
            public static GUIContent content_SettingsBoxTitle_BasicSettings = new GUIContent("Basic Settings", "Allows you to adjust the top-most level of settings for this Transition");
            public static GUIContent content_SettingsBoxTitle_Phases = new GUIContent("Phases", "Shows the \"Phases\" setting page. ");
            public static GUIContent content_SettingsBoxTitle_PhaseOptions = new GUIContent("Phase Options", "All of the options inside of this box affect how the selected Phase behaves");
            public static GUIContent content_SettingsBoxTitle_ComponentsOnGameObject = new GUIContent("Components on GameObject", "Lists all of the valid components on the GameObject. ");
            public static GUIContent content_SettingsBoxTitle_MembersOnGameObject = new GUIContent("Members on GameObject", "Displays all of the valid members that a component has. Invalid components will not be listed!");

            // Help Boxes
            public const string helpbox_cannotBeLerped = "The member type of %%custom%% cannot be lerped. The value will be set at the end of the current Phase";
        }
    }
}