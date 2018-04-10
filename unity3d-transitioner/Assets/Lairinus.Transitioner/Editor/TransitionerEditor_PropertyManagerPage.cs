using Lairinus.Transitions.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private void DisplayPhaseMember(int index, SerializedProperty reflectedMemberSingle, SerializedProperty reflectedPhaseMembersListProperty)
        {
            // Displays a single member inside of a Phase
            SerializedProperty rmAvailableMemberTypeEnum = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_availableMemberType);
            SerializedProperty rmMemberName = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_memberName);
            SerializedProperty rmParentComponent = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_parentComponent);
            SerializedProperty rmIsDisabled = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_isDisabled);
            SerializedProperty rmCanBeLerped = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_canBeLerped);
            SerializedProperty rmStringValue = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_memberValueString);
            SerializedProperty rmUseSeparateAnimationCurve = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_useSeparateAnimationCurve);
            SerializedProperty rmAnimationCurve = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_animationCurve);
            SerializedProperty phaseDelay = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_Delay);
            SerializedProperty phaseDuration = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_Duration);
            string typeFromComponent = GetStringTypeFromComponentProperty(rmParentComponent);

            GUILayout.Space(20);
            if (!rmCanBeLerped.boolValue)
                EditorGUILayout.HelpBox(Helper.helpbox_cannotBeLerped.Replace("%%custom%%", typeFromComponent), MessageType.Info);
            EditorGUILayout.BeginVertical(_editorStyles.headerBoxStyle);
            EditorGUILayout.LabelField("Component: - " + typeFromComponent, EditorStyles.largeLabel, GUILayout.Height(25));
            EditorGUILayout.LabelField("Member: - " + rmMemberName.stringValue + " - " + Utility.GetAvailableMemberName(rmAvailableMemberTypeEnum.enumValueIndex), EditorStyles.largeLabel, GUILayout.Height(25));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.PropertyField(rmIsDisabled, new GUIContent("Disabled"));
            EditorGUILayout.Space();
            if (!rmIsDisabled.boolValue)
            {
                ShowProperty(rmStringValue, rmAvailableMemberTypeEnum);
                ShowGetSetButtons(index, rmStringValue);

                EditorGUILayout.Space();
                if (rmCanBeLerped.boolValue && phaseDuration.floatValue > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(rmUseSeparateAnimationCurve, new GUIContent("Use Separate Curve:", "A separate curve will not follow the parent Phase's curve and will act on its' own. It is based off of the parent Phase's delay time and duration"));
                }
                EditorGUILayout.Space();

                if (rmUseSeparateAnimationCurve.boolValue && phaseDuration.floatValue > 0)
                {
                    ShowLerpCurve(new GUIContent("Curve"), rmAnimationCurve, phaseDuration, phaseDelay, 0);
                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
                GUILayout.Space(5);
            }

            DisplayMainButton(new GUIContent("Remove Member", "Removes this member from the Phase"), _editorStyles.lairinusRed, new Action(() => OnHandleClick_RemovePhaseMember(reflectedPhaseMembersListProperty, index)));
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(20);
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

        private void OnHandleClick_RemovePhaseMember(SerializedProperty phaseMemberListProperty, int index)
        {
            if (phaseMemberListProperty == null)
                return;

            if (index > -1 && index < phaseMemberListProperty.arraySize)
                phaseMemberListProperty.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }

        private void ShowGetSetButtons(int index, SerializedProperty reflectedMemberSingleValue)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            try
            {
                Transitioner targetObject = (Transitioner)target;
                if (GUILayout.Button(Helper.content_getValueButton))
                {
                    object value = targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index].GetValue();
                    if (value != null)
                        reflectedMemberSingleValue.stringValue = value.ToString();

                    serializedObject.ApplyModifiedProperties();
                }

                if (GUILayout.Button(Helper.content_setValueButton))
                {
                    PhaseMember member = targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index];
                    Type type = Utility.GetInstance().reverseTypeDictionary[member.serializedMemberType];
                    object value = Utility.GetObject(type, reflectedMemberSingleValue.stringValue);
                    targetObject.phases[_currentlySelectedPhaseIndex].reflectedMembers[index].SetValue(value);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            catch
            {
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowProperty(SerializedProperty stringValueProperty, SerializedProperty availableMemberTypeEnum)
        {
            try
            {
                Utility.AvailableMemberTypes type = (Utility.AvailableMemberTypes)availableMemberTypeEnum.enumValueIndex;
                string valueString = stringValueProperty.stringValue;
                GUIContent valueContent = new GUIContent("Value", "The final value that this member will attempt to lerp to");
                if (valueString == null)
                    return;

                switch (type)
                {
                    case Utility.AvailableMemberTypes.Float:
                        {
                            float value = 0;
                            float.TryParse(valueString, out value);
                            value = EditorGUILayout.FloatField(valueContent, value);
                            break;
                        }

                    case Utility.AvailableMemberTypes.Integer:
                        {
                            int value = 0;
                            int.TryParse(valueString, out value);
                            value = EditorGUILayout.IntField(valueContent, value);
                            break;
                        }

                    case Utility.AvailableMemberTypes.Boolean:
                        {
                            bool value = Utility.GetObject<bool>(valueString);
                            value = EditorGUILayout.Toggle(valueContent, value);
                            stringValueProperty.stringValue = value.ToString();
                            break;
                        }

                    case Utility.AvailableMemberTypes.Color:
                        {
                            Color value = Utility.GetObject<Color>(valueString);
                            value = EditorGUILayout.ColorField(valueContent, value);
                            stringValueProperty.stringValue = value.ToString();
                            break;
                        }

                    case Utility.AvailableMemberTypes.String:
                        {
                            valueString = EditorGUILayout.TextField(valueContent, valueString);
                            stringValueProperty.stringValue = valueString;
                            break;
                        }

                    case Utility.AvailableMemberTypes.Vector2:
                        {
                            Vector2 value = Utility.GetObject<Vector2>(valueString);
                            value = EditorGUILayout.Vector2Field(valueContent, value);
                            stringValueProperty.stringValue = value.ToString();
                            break;
                        }

                    case Utility.AvailableMemberTypes.Vector3:
                        {
                            Vector3 value = Utility.GetObject<Vector3>(valueString);
                            value = EditorGUILayout.Vector3Field(valueContent, value);
                            stringValueProperty.stringValue = value.ToString();
                            break;
                        }

                    case Utility.AvailableMemberTypes.Vector4:
                        {
                            Vector4 value = Utility.GetObject<Vector4>(valueString);
                            value = EditorGUILayout.Vector4Field(valueContent, value);
                            stringValueProperty.stringValue = value.ToString();
                            break;
                        }
                }
                serializedObject.ApplyModifiedProperties();
            }
            catch
            {
            }
        }
    }
}