using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using UnityEditor;
using Lairinus.Transitions.Internal;

namespace Lairinus.Transitions
{
    public enum MemberType
    {
        Property,
        Field
    }

    [System.Serializable]
    public class ReflectedPhaseMember
    {
        public ReflectedPhaseMember(MemberType memberType, Component _parentComponent, string _memberName, Type type)
        {
            _sf_memberType = memberType;
            _sf_parentComponent = _parentComponent;
            _sf_memberName = _memberName;
            if (TransitionerUtility.GetInstance().typesDictionary.ContainsKey(type))
                _sf_serializedPropertyType = TransitionerUtility.GetInstance().typesDictionary[type];
        }

        [SerializeField] private MemberType _sf_memberType = MemberType.Field;
        [SerializeField] private TransitionerUtility.AvailableMemberTypes _sf_serializedPropertyType = new TransitionerUtility.AvailableMemberTypes();
        [SerializeField] private Component _sf_parentComponent = null;
        [SerializeField] private string _sf_memberName = "";
        [SerializeField] private string _sf_memberValueString = "";
        [SerializeField] private bool _sf_isEnabled = true;
        [SerializeField] private bool _sf_canBeLerped = false;
        public Component parentComponent { get { return _sf_parentComponent; } }
        public string memberName { get { return _sf_memberName; } }
        public PropertyInfo property { get; private set; }
        public TransitionerUtility.AvailableMemberTypes serializedMemberType { get { return _sf_serializedPropertyType; } }
        public bool isEnabled { get { return _sf_isEnabled; } }

        public FieldInfo field { get; private set; }
        public MemberType memberType { get { return _sf_memberType; } }

        public object GetMemberValue(SerializedPropertyType type)
        {
            return null;
        }
    }
}