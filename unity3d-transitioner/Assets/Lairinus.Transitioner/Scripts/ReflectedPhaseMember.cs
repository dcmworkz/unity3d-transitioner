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
            _sf_type = type;
            _typeString = _sf_type.ToString();
        }

        private string _typeString = "";
        [SerializeField] private MemberType _sf_memberType = MemberType.Field;
        [SerializeField] private TransitionerUtility.AvailableMemberTypes _sf_serializedPropertyType = new TransitionerUtility.AvailableMemberTypes();
        [SerializeField] private Component _sf_parentComponent = null;
        [SerializeField] private string _sf_memberName = "";
        [SerializeField] private string _sf_memberValueString = "";
        [SerializeField] private Type _sf_type = null;
        public Component parentComponent { get { return _sf_parentComponent; } }
        public string memberName { get { return _sf_memberName; } }
        public PropertyInfo property { get; private set; }

        public FieldInfo field { get; private set; }
        public MemberType memberType { get { return _sf_memberType; } }
        public Type type { get { return _sf_type; } }

        public object GetMemberValue(SerializedPropertyType type)
        {
            switch (_typeString)
            {
            }

            return null;
        }
    }
}