using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Reflection;
using Lairinus.Transitions.Internal;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private UITransitioner thisObject = null;

        private void DrawPropertyManagerPage()
        {
            if (_currentPage != Pages.PropertyManager)
                return;

            thisObject = (UITransitioner)target;
            Action openPage = new Action(() => OpenPage(Pages.Phases));
            DisplayMainButton(new GUIContent("Back", "Returns to the Phases page"), _editorStyles.lairinusRed, openPage, true, null, 20, 20);

            Action openPropertySelectionPage = new Action(() => OpenPage(Pages.PropertySelector));
            DisplayMainButton(new GUIContent("Add Members", "Adds more fields and/or properties to this phase"), _editorStyles.lairinusGreen, openPropertySelectionPage, true, null, 20, 20);

            DisplayPhaseProperties();
        }

        private void DisplayPhaseProperties()
        {
            if (_currentSelectedPhaseProperty != null)
            {
                List<Action> phasePropertyActions = new List<Action>();
                SerializedProperty reflectedMembersListProperty = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_reflectedMembers");
                for (var a = 0; a < reflectedMembersListProperty.arraySize; a++)
                {
                    int captured = a;
                    SerializedProperty reflectedMemberSingle = reflectedMembersListProperty.GetArrayElementAtIndex(captured);
                    DisplayPhaseMember(captured, reflectedMemberSingle, reflectedMembersListProperty);
                }
            }
        }

        private void DisplayPhaseMember(int index, SerializedProperty reflectedMemberSingle, SerializedProperty reflectedPhaseMembersListProperty)
        {
            SerializedProperty rmMemberType = reflectedMemberSingle.FindPropertyRelative("_sf_memberType");
            SerializedProperty rmMemberSerializedType = reflectedMemberSingle.FindPropertyRelative("_sf_serializedPropertyType");
            SerializedProperty rmMemberName = reflectedMemberSingle.FindPropertyRelative("_sf_memberName");
            SerializedProperty rmParentComponent = reflectedMemberSingle.FindPropertyRelative("_sf_parentComponent");
            SerializedProperty rmIsEnabled = reflectedMemberSingle.FindPropertyRelative("_sf_isEnabled");
            SerializedProperty rmCanBeLerped = reflectedMemberSingle.FindPropertyRelative("sf_canBeLerped");

            GUILayout.Space(20);
            //if (!rmCanBeLerped.boolValue)
            //{
            //    EditorGUILayout.HelpBox("This property cannot be lerped. Instead, this property will be set at the end of the Phase", MessageType.Info);
            //}

            EditorGUILayout.LabelField(rmMemberName.stringValue + " - " + TransitionerUtility.GetAvailableMemberName(rmMemberSerializedType.enumValueIndex));
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(rmIsEnabled, new GUIContent("Is Enabled"));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            GUILayout.Space(20);
            EditorGUILayout.EndHorizontal();
            DisplayMainButton(new GUIContent("Remove Member", "Removes this member from the Phase"), _editorStyles.lairinusRed, new Action(() => OnHandleClick_RemovePhaseMember(reflectedPhaseMembersListProperty, index)));
            serializedObject.ApplyModifiedProperties();
        }

        private void OnHandleClick_RemovePhaseMember(SerializedProperty phaseMemberListProperty, int index)
        {
            if (phaseMemberListProperty == null)
                return;

            if (index > -1 && index < phaseMemberListProperty.arraySize)
                phaseMemberListProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }
    }
}