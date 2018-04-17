using Lairinus.Transitions.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
    public partial class UITransitionerEditor : Editor
    {
        private List<Action> _componentActions = new List<Action>();
        private List<PhaseMember> _allReflectedPhaseMembers = new List<PhaseMember>();
        private Component _selectedComponent = null;

        private void DrawPage_PropertySelector()
        {
            if (_currentPage != Pages.PropertySelector)
                return;

            Action openPage = new Action(() => OpenPage(Pages.PropertyManager));
            DisplayMainButton(Helper.content_button_Back, _editorStyles.lairinusRed, openPage, true, null, 20, 20);
            _allReflectedPhaseMembers = _allReflectedPhaseMembers.OrderBy(x => x.memberName).ToList();
            DisplaySettingBox(Helper.content_SettingsBoxTitle_ComponentsOnGameObject, _componentActions.ToArray(), 20);
            if (_selectedComponent != null)
            {
                List<Action> reflectedPhaseMemberDisplayActions = new List<Action>();
                foreach (PhaseMember reflectedPhaseMember in _allReflectedPhaseMembers)
                {
                    reflectedPhaseMemberDisplayActions.Add(new Action(() => DisplayReflectedPhaseMember(reflectedPhaseMember, 20, 10)));
                }

                DisplaySettingBox(Helper.content_SettingsBoxTitle_MembersOnGameObject, reflectedPhaseMemberDisplayActions.ToArray(), 20);
            }
        }

        private void InitializePropertySelectorPage()
        {
            _selectedComponent = null;
            SerializedProperty property = serializedObject.FindProperty(Helper.transitionerProp_targetGameObject);
            GameObject targetGameObject = property.objectReferenceValue as GameObject;
            Component[] components = targetGameObject.GetComponents(typeof(Component));
            _componentActions.Clear();
            foreach (Component c in components)
            {
                if (c.GetType() == typeof(Transitioner))
                    continue;

                Action componentAction = () => DisplayComponentSelectUI(c);
                _componentActions.Add(componentAction);
            }
        }

        private void DisplayReflectedPhaseMember(PhaseMember reflectedPhaseMember, float horizontalSpace = 20, float verticalSpace = 5)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Space(horizontalSpace);
            GUIContent buttonContent = new GUIContent(reflectedPhaseMember.memberName + "\n\n(" + reflectedPhaseMember.serializedMemberType.ToString() + ")", "Add this " + reflectedPhaseMember.serializedMemberType.ToString() + " to your current Phase!");
            if (GUILayout.Button(buttonContent))
                HandleOnClick_WriteSelectedPropertyToPhase(reflectedPhaseMember);

            GUILayout.Space(horizontalSpace);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        private void HandleOnClick_WriteSelectedPropertyToPhase(PhaseMember reflectedPhaseMember)
        {
            if (_currentSelectedPhaseProperty == null)
                return;

            // Set members
            SerializedProperty reflectedPropertiesList = _currentSelectedPhaseProperty.FindPropertyRelative(Helper.phaseProp_reflectedMembers);
            reflectedPropertiesList.arraySize++;
            SerializedProperty reflectedMemberSingle = reflectedPropertiesList.GetArrayElementAtIndex(reflectedPropertiesList.arraySize - 1);
            SerializedProperty rmMemberType = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMmemberProp_memberType);
            SerializedProperty rmMemberSerializedType = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_availableMemberType);
            SerializedProperty rmMemberName = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_memberName);
            SerializedProperty rmParentComponent = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_parentComponent);
            SerializedProperty rmCanBeLerped = reflectedMemberSingle.FindPropertyRelative(Helper.phaseMemberProp_canBeLerped);
            rmMemberType.enumValueIndex = (int)reflectedPhaseMember.memberType;
            rmMemberSerializedType.enumValueIndex = (int)reflectedPhaseMember.serializedMemberType;
            rmMemberName.stringValue = reflectedPhaseMember.memberName;
            rmParentComponent.objectReferenceValue = reflectedPhaseMember.parentComponent;
            rmCanBeLerped.boolValue = Utility.CanBeLerped(reflectedPhaseMember.serializedMemberType);
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
                if (Utility.GetInstance().typesDictionary.ContainsKey(componentFieldInfo.FieldType))
                {
                    PhaseMember reflectedPhaseMember = new PhaseMember(MemberType.Field, reflectedComponent, componentFieldInfo.Name, componentFieldInfo.FieldType);
                    _allReflectedPhaseMembers.Add(reflectedPhaseMember);
                }
            }

            foreach (PropertyInfo componentPropertyInfo in reflectedComponent.GetType().GetProperties())
            {
                if (componentPropertyInfo.CanRead && componentPropertyInfo.CanWrite && Utility.GetInstance().typesDictionary.ContainsKey(componentPropertyInfo.PropertyType))
                {
                    PhaseMember reflectedPhaseMember = new PhaseMember(MemberType.Property, reflectedComponent, componentPropertyInfo.Name, componentPropertyInfo.PropertyType);
                    _allReflectedPhaseMembers.Add(reflectedPhaseMember);
                }
            }
        }
    }
}