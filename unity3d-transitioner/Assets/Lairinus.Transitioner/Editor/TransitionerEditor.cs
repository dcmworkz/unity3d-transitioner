using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
    [CustomEditor(typeof(Transitioner))]
    public partial class UITransitionerEditor : Editor
    {
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

        private enum Pages
        {
            Main,
            Phases,
            PropertyManager,
            PropertySelector
        }

        private Pages _currentPage = Pages.Main;
        private TransitionerEditorStyles _editorStyles = null;
        private SerializedProperty _loop = null;
        private SerializedProperty _phasesProperty = null;
        private SerializedProperty _targetGameObject = null;
        private SerializedProperty disableTransition = null;

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

        private void InitializeProperties()
        {
            _targetGameObject = serializedObject.FindProperty("_sf_targetGameObject");
            disableTransition = serializedObject.FindProperty("_sf_disableTransition");
            _loop = serializedObject.FindProperty("_sf_loop");
            _phasesProperty = serializedObject.FindProperty("_sf_phases");
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            _editorStyles = Resources.Load<TransitionerEditorStyles>("TransitionerEditorStyles");
            InitializeProperties();
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

        private void ShowLerpCurve(GUIContent content, SerializedProperty animationCurveProperty, SerializedProperty durationProperty, SerializedProperty delayProperty, float leftRightPadding = 20)
        {
            if (durationProperty.floatValue <= 0)
                return;

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            GUILayout.Space(leftRightPadding);
            EditorGUILayout.LabelField(content);
            AnimationCurve curve = animationCurveProperty.animationCurveValue;
            Rect ranges = new Rect(delayProperty.floatValue, 0, durationProperty.floatValue, 1);
            GUILayout.Space(leftRightPadding);
            curve = EditorGUILayout.CurveField(curve, _editorStyles.lairinusBlue, ranges, GUILayout.Height(100), GUILayout.Width(150));
            GUILayout.Space(leftRightPadding);
            GUILayout.EndHorizontal();
            animationCurveProperty.animationCurveValue = curve;
            serializedObject.ApplyModifiedProperties();
        }

        private static class Helper
        {
            // Help Boxes
            public const string helpbox_cannotBeLerped = "The member type of %%custom%% cannot be lerped. The value will be set at the end of the current Phase";

            public const string phaseMemberProp_animationCurve = "_sf_animationCurve";

            // Phase Members -> Properties
            public const string phaseMemberProp_availableMemberType = "_sf_serializedPropertyType";

            public const string phaseMemberProp_canBeLerped = "_sf_canBeLerped";

            public const string phaseMemberProp_isDisabled = "_sf_isDisabled";

            public const string phaseMemberProp_memberName = "_sf_memberName";

            public const string phaseMemberProp_memberValueString = "_sf_memberValueString";

            public const string phaseMemberProp_parentComponent = "_sf_parentComponent";

            public const string phaseMemberProp_useSeparateAnimationCurve = "_sf_useSeparateAnimationCurve";

            public const string phaseMmemberProp_memberType = "_sf_memberType";

            // Phases -> Properties
            public const string phaseProp_Delay = "_sf_delay";

            public const string phaseProp_disabled = "_sf_disabled";
            public const string phaseProp_Duration = "_sf_duration";
            public const string phaseProp_lerpPlaystyleType = "_sf_lerpPlaystyleType";
            public const string phaseProp_name = "_sf_name";
            public const string phaseProp_reflectedMembers = "_sf_reflectedMembers";
            public static GUIContent content_button_addPhase = new GUIContent("Add Phase", "Creates a new Phase and adds it to this Transition.");

            public static GUIContent content_button_Back = new GUIContent("Back", "Returns to the previous page");

            public static GUIContent content_button_EditProperties = new GUIContent("Edit Members", "Each member inside of a Phase will act together in order to modify the GameObject.");

            public static GUIContent content_DeletePhaseButton = new GUIContent("", "Deletes the Phase.");

            public static GUIContent content_disableTransition = new GUIContent("Disable Transition", "Determines if whether the current Transition can be processed or not");

            // GUI Contents
            public static GUIContent content_getValueButton = new GUIContent("Get", "Applies the value found on the object to this Phase Member");

            public static GUIContent content_loopTransition = new GUIContent("Loop", "If enabled, the transition will continuously play until it is manually stopped, or this property gets disabled");
            public static GUIContent content_mainButton_modifyPhases = new GUIContent("Modify Phases", "Allows you to modify the Phases inside of this transition.");
            public static GUIContent content_MovePhaseDown = new GUIContent("", "Moves this Phase one down");
            public static GUIContent content_MovePhaseUp = new GUIContent("", "Moves this Phase one up");
            public static GUIContent content_PhaseAnimationCurve = new GUIContent("Animation Playstyle", "How this Phase will play. Each 'Lerpable' member will behave in this manner, unless you specify a different curve for a specific property");
            public static GUIContent content_phaseDelay = new GUIContent("Delay:", "The total time that elapses before the members in this phase begin their transitions");
            public static GUIContent content_phaseDisabled = new GUIContent("Disabled:", "If true, this entire phase will be skipped");
            public static GUIContent content_phaseDuration = new GUIContent("Duration:", "The total time that this Phase runs for");
            public static GUIContent content_phaseName = new GUIContent("Name:", "The name of this Phase. This is primarily used to help you organize your Phases");
            public static GUIContent content_SettingsBoxTitle_BasicSettings = new GUIContent("Basic Settings", "Allows you to adjust the top-most level of settings for this Transition");
            public static GUIContent content_SettingsBoxTitle_ComponentsOnGameObject = new GUIContent("Components on GameObject", "Lists all of the valid components on the GameObject. ");
            public static GUIContent content_SettingsBoxTitle_MembersOnGameObject = new GUIContent("Members on GameObject", "Displays all of the valid members that a component has. Invalid components will not be listed!");
            public static GUIContent content_SettingsBoxTitle_PhaseOptions = new GUIContent("Phase Options", "All of the options inside of this box affect how the selected Phase behaves");
            public static GUIContent content_SettingsBoxTitle_Phases = new GUIContent("Phases", "Shows the \"Phases\" setting page. ");
            public static GUIContent content_setValueButton = new GUIContent("Set", "Updates the object's values based on this Phase Member");
            public static GUIContent content_targetGameObject = new GUIContent("Target GameObject", "The GameObject that will handle all of the Transitions and Phases");
        }
    }
}