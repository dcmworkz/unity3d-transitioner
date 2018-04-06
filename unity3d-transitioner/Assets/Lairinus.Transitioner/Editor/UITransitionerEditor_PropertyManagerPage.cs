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
        private void DrawPropertyManagerPage()
        {
            if (_currentPage != Pages.PropertyManager)
                return;

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
            SerializedProperty rmAvailableMemberTypeEnum = reflectedMemberSingle.FindPropertyRelative("_sf_serializedPropertyType");
            SerializedProperty rmMemberName = reflectedMemberSingle.FindPropertyRelative("_sf_memberName");
            SerializedProperty rmParentComponent = reflectedMemberSingle.FindPropertyRelative("_sf_parentComponent");
            SerializedProperty rmIsDisabled = reflectedMemberSingle.FindPropertyRelative("_sf_isDisabled");
            SerializedProperty rmCanBeLerped = reflectedMemberSingle.FindPropertyRelative("_sf_canBeLerped");
            SerializedProperty rmStringValue = reflectedMemberSingle.FindPropertyRelative("_sf_memberValueString");

            GUILayout.Space(20);
            EditorGUILayout.LabelField(rmParentComponent.objectReferenceValue.GetType().ToString(), EditorStyles.largeLabel, GUILayout.Height(25));
            EditorGUILayout.LabelField(rmMemberName.stringValue + " - " + TransitionerUtility.GetAvailableMemberName(rmAvailableMemberTypeEnum.enumValueIndex), EditorStyles.boldLabel, GUILayout.Height(30));
            if (!rmCanBeLerped.boolValue)
                EditorGUILayout.HelpBox("This value type cannot be lerped. The value will be set at the end of this phase", MessageType.Info);
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(rmIsDisabled, new GUIContent("Disabled"));
            EditorGUILayout.Space();
            ShowProperty(rmStringValue, rmAvailableMemberTypeEnum);
            ShowGetSetButtons(index, rmStringValue);
            EditorGUILayout.Space();
            GUILayout.Space(5);
            DisplayMainButton(new GUIContent("Remove Member", "Removes this member from the Phase"), _editorStyles.lairinusRed, new Action(() => OnHandleClick_RemovePhaseMember(reflectedPhaseMembersListProperty, index)));
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }

        private void ShowGetSetButtons(int index, SerializedProperty reflectedMemberSingleValue)
        {
            try
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                UITransitioner targetObject = (UITransitioner)target;
                if (GUILayout.Button(new GUIContent("Get Value")))
                {
                    object value = targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index].GetValue();
                    if (value != null)
                        reflectedMemberSingleValue.stringValue = value.ToString();

                    serializedObject.ApplyModifiedProperties();
                }

                if (GUILayout.Button(new GUIContent("Set Value")))
                {
                    ReflectedPhaseMember member = targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index];
                    Type type = TransitionerUtility.GetInstance().reverseTypeDictionary[member.serializedMemberType];
                    object value = TransitionerUtility.GetObject(type, reflectedMemberSingleValue.stringValue);
                    targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index].SetValue(value);
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.EndHorizontal();
            }
            catch
            {
            }
        }

        private void ShowProperty(SerializedProperty stringValueProperty, SerializedProperty availableMemberTypeEnum)
        {
            TransitionerUtility.AvailableMemberTypes type = (TransitionerUtility.AvailableMemberTypes)availableMemberTypeEnum.enumValueIndex;
            string valueString = stringValueProperty.stringValue;
            GUIContent valueContent = new GUIContent("Value", "The final value that this member will attempt to lerp to");
            if (valueString == null)
                return;

            switch (type)
            {
                case TransitionerUtility.AvailableMemberTypes.Float:
                    {
                        float value = 0;
                        float.TryParse(valueString, out value);
                        value = EditorGUILayout.FloatField(valueContent, value);
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Integer:
                    {
                        int value = 0;
                        int.TryParse(valueString, out value);
                        value = EditorGUILayout.IntField(valueContent, value);
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Boolean:
                    {
                        bool value = TransitionerUtility.GetObject<bool>(valueString);
                        value = EditorGUILayout.Toggle(valueContent, value);
                        stringValueProperty.stringValue = value.ToString();
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Color:
                    {
                        Color value = TransitionerUtility.GetObject<Color>(valueString);
                        value = EditorGUILayout.ColorField(valueContent, value);
                        stringValueProperty.stringValue = value.ToString();
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.String:
                    {
                        valueString = EditorGUILayout.TextField(valueContent, valueString);
                        stringValueProperty.stringValue = valueString;
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Vector2:
                    {
                        Vector2 value = TransitionerUtility.GetObject<Vector2>(valueString);
                        value = EditorGUILayout.Vector2Field(valueContent, value);
                        stringValueProperty.stringValue = value.ToString();
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Vector3:
                    {
                        Vector3 value = TransitionerUtility.GetObject<Vector3>(valueString);
                        value = EditorGUILayout.Vector3Field(valueContent, value);
                        stringValueProperty.stringValue = value.ToString();
                        break;
                    }

                case TransitionerUtility.AvailableMemberTypes.Vector4:
                    {
                        Vector4 value = TransitionerUtility.GetObject<Vector4>(valueString);
                        value = EditorGUILayout.Vector4Field(valueContent, value);
                        stringValueProperty.stringValue = value.ToString();
                        break;
                    }
            }
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