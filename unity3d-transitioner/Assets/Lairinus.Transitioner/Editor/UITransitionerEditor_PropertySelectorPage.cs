using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using UnityEditorInternal;
using Lairinus.Transitions.Internal;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private List<Action> _componentActions = new List<Action>();
        private List<ReflectedPhaseMember> _allReflectedPhaseMembers = new List<ReflectedPhaseMember>();
        private Component _selectedComponent = null;

        private void DrawPage_PropertySelector()
        {
            if (_currentPage != Pages.PropertySelector)
                return;

            Action openPage = new Action(() => OpenPage(Pages.PropertyManager));
            DisplayMainButton(new GUIContent("Back", "Returns to the Property Manager page"), _editorStyles.lairinusRed, openPage, true, null, 20, 20);
            _allReflectedPhaseMembers = _allReflectedPhaseMembers.OrderBy(x => x.memberName).ToList();
            DisplaySettingBox("Components on GameObject", _componentActions.ToArray(), 20);
            if (_selectedComponent != null)
            {
                List<Action> reflectedPhaseMemberDisplayActions = new List<Action>();
                foreach (ReflectedPhaseMember reflectedPhaseMember in _allReflectedPhaseMembers)
                {
                    reflectedPhaseMemberDisplayActions.Add(new Action(() => DisplayReflectedPhaseMember(reflectedPhaseMember, 20, 10)));
                }

                DisplaySettingBox("Values on GameObject", reflectedPhaseMemberDisplayActions.ToArray(), 20);
            }
        }

        private void InitializePropertySelectorPage()
        {
            _selectedComponent = null;
            UITransitioner uiTransition = (UITransitioner)target;
            Component[] components = uiTransition.gameObject.GetComponents(typeof(Component));
            _componentActions.Clear();
            foreach (Component c in components)
            {
                if (c.GetType() == typeof(UITransitioner))
                    continue;

                Action componentAction = () => DisplayComponentSelectUI(c);
                _componentActions.Add(componentAction);
            }
        }

        private void DisplayReflectedPhaseMember(ReflectedPhaseMember reflectedPhaseMember, float horizontalSpace = 20, float verticalSpace = 5)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(horizontalSpace);
            GUIContent buttonContent = new GUIContent(reflectedPhaseMember.memberName + "\n\n(" + reflectedPhaseMember.serializedMemberType.ToString() + ")", "Add this " + reflectedPhaseMember.serializedMemberType.ToString() + " to your current Phase!");
            if (GUILayout.Button(buttonContent))
            {
                HandleOnClick_WriteSelectedPropertyToPhase(reflectedPhaseMember);
            }
            GUILayout.Space(horizontalSpace);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void HandleOnClick_WriteSelectedPropertyToPhase(ReflectedPhaseMember reflectedPhaseMember)
        {
            if (_currentSelectedPhaseProperty == null)
                return;

            // Set members
            SerializedProperty reflectedPropertiesList = _currentSelectedPhaseProperty.FindPropertyRelative("_sf_reflectedMembers");
            reflectedPropertiesList.arraySize++;
            SerializedProperty reflectedMemberSingle = reflectedPropertiesList.GetArrayElementAtIndex(reflectedPropertiesList.arraySize - 1);
            SerializedProperty rmMemberType = reflectedMemberSingle.FindPropertyRelative("_sf_memberType");
            SerializedProperty rmMemberSerializedType = reflectedMemberSingle.FindPropertyRelative("_sf_serializedPropertyType");
            SerializedProperty rmMemberName = reflectedMemberSingle.FindPropertyRelative("_sf_memberName");
            SerializedProperty rmParentComponent = reflectedMemberSingle.FindPropertyRelative("_sf_parentComponent");
            SerializedProperty rmCanBeLerped = reflectedMemberSingle.FindPropertyRelative("_sf_canBeLerped");
            rmMemberType.enumValueIndex = (int)reflectedPhaseMember.memberType;
            rmMemberSerializedType.enumValueIndex = (int)reflectedPhaseMember.serializedMemberType;
            rmMemberName.stringValue = reflectedPhaseMember.memberName;
            rmParentComponent.objectReferenceValue = reflectedPhaseMember.parentComponent;
            rmCanBeLerped.boolValue = TransitionerUtility.CanBeLerped(reflectedPhaseMember.serializedMemberType);
            serializedObject.ApplyModifiedProperties();
            OpenPage(Pages.PropertyManager);
        }

        private void DisplayComponentSelectUI(Component reflectedComponent)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUIContent guiContent = new GUIContent(reflectedComponent.GetType().FullName);
            if (GUILayout.Button(guiContent))
            {
                HandleOnClick_SelectComponent(reflectedComponent);
            }
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void HandleOnClick_SelectComponent(Component reflectedComponent)
        {
            if (reflectedComponent == null)
                return;

            _selectedComponent = reflectedComponent;
            _allReflectedPhaseMembers.Clear();

            foreach (FieldInfo componentFieldInfo in reflectedComponent.GetType().GetFields())
            {
                if (TransitionerUtility.GetInstance().typesDictionary.ContainsKey(componentFieldInfo.FieldType))
                {
                    ReflectedPhaseMember reflectedPhaseMember = new ReflectedPhaseMember(MemberType.Field, reflectedComponent, componentFieldInfo.Name, componentFieldInfo.FieldType);
                    _allReflectedPhaseMembers.Add(reflectedPhaseMember);
                }
            }

            foreach (PropertyInfo componentPropertyInfo in reflectedComponent.GetType().GetProperties())
            {
                if (componentPropertyInfo.CanRead && componentPropertyInfo.CanWrite && TransitionerUtility.GetInstance().typesDictionary.ContainsKey(componentPropertyInfo.PropertyType))
                {
                    ReflectedPhaseMember reflectedPhaseMember = new ReflectedPhaseMember(MemberType.Property, reflectedComponent, componentPropertyInfo.Name, componentPropertyInfo.PropertyType);
                    _allReflectedPhaseMembers.Add(reflectedPhaseMember);
                }
            }
        }
    }
}