using Lairinus.Transitions.Internal;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lairinus.Transitions
{
    public enum MemberType
    {
        Property,
        Field
    }

    [System.Serializable]
    public class PhaseMember
    {
        public PhaseMember(MemberType memberType, Component _parentComponent, string _memberName, Type type)
        {
            _sf_memberType = memberType;
            _sf_parentComponent = _parentComponent;
            _sf_memberName = _memberName;
            if (Utility.GetInstance().typesDictionary.ContainsKey(type))
                _sf_serializedPropertyType = Utility.GetInstance().typesDictionary[type];
        }

        [SerializeField] private MemberType _sf_memberType = MemberType.Field;
        [SerializeField] private Utility.AvailableMemberTypes _sf_serializedPropertyType = new Utility.AvailableMemberTypes();
        [SerializeField] private Component _sf_parentComponent = null;
        [SerializeField] private string _sf_memberName = "";
        [SerializeField] private string _sf_memberValueString = "";
        [SerializeField] private bool _sf_isDisabled = false;
        [SerializeField] private bool _sf_canBeLerped = false;
        [SerializeField] private bool _sf_useSeparateAnimationCurve = false;
        [SerializeField] private AnimationCurve _sf_animationCurve = new AnimationCurve();
        public bool canBeLerped { get { return _sf_canBeLerped; } }
        public bool useSeparateAnimationCurve { get { return _sf_useSeparateAnimationCurve; } }
        protected string memberValueString { get { return _sf_memberValueString; } }
        public Component parentComponent { get { return _sf_parentComponent; } }
        public string memberName { get { return _sf_memberName; } }
        public PropertyInfo property { get; private set; }
        public AnimationCurve separateAnimationCurve { get { return _sf_animationCurve; } }
        public FieldInfo field { get; private set; }
        public Utility.AvailableMemberTypes serializedMemberType { get { return _sf_serializedPropertyType; } }
        public bool isDisabled { get { return _sf_isDisabled; } }
        private object _startValue = null;
        private object _resetValue = null;

        public void SetValue(object value)
        {
            try
            {
                if (field == null && property == null)
                    AssignReflectedMemberFields();

                if (_sf_memberType == MemberType.Field && field != null)
                    field.SetValue(_sf_parentComponent, value);
                else if (_sf_memberType == MemberType.Property && property != null)
                    property.SetValue(_sf_parentComponent, value, null);
            }
            catch
            {
            }
        }

        public object GetValue()
        {
            AssignReflectedMemberFields();
            if (_sf_memberType == MemberType.Field && field != null)
                return field.GetValue(_sf_parentComponent);
            else if (_sf_memberType == MemberType.Property && property != null)
                return property.GetValue(_sf_parentComponent, null);
            else return new object();
        }

        public void UpdatePhaseMember(float curveTime, float actualTime, bool isResetting)
        {
            if (_sf_isDisabled)
                return;

            if (isResetting)
                _startValue = _resetValue;

            if (_startValue == null)
                _startValue = GetValue();

            if (_resetValue == null)
                _resetValue = _startValue;

            float lerpValueFloat = curveTime;
            if (_sf_useSeparateAnimationCurve)
                lerpValueFloat = _sf_animationCurve.Evaluate(actualTime);

            if (_sf_canBeLerped)
            {
                string currentValue = _startValue.ToString();
                string finalValue = _sf_memberValueString;
                object lerpedValue = Utility.GetLerpedValue(currentValue, finalValue, _sf_serializedPropertyType, lerpValueFloat);
                SetValue(lerpedValue);
                if (lerpValueFloat >= 1)
                    _startValue = null;
            }
            else if (lerpValueFloat >= 1)
            {
                object finalValue = Utility.GetObject(Utility.GetInstance().reverseTypeDictionary[_sf_serializedPropertyType], _sf_memberValueString);
                SetValue(finalValue);
                _startValue = null;
            }
        }

        private void AssignReflectedMemberFields()
        {
            if (_sf_parentComponent == null)
                return;

            if (_sf_memberType == MemberType.Property)
            {
                if (property == null)
                    property = _sf_parentComponent.GetType().GetProperty(_sf_memberName);
            }

            if (_sf_memberType == MemberType.Field)
            {
                if (field == null)
                    field = _sf_parentComponent.GetType().GetField(_sf_memberName);
            }
        }

        public MemberType memberType { get { return _sf_memberType; } }

        public object GetMemberValue(SerializedPropertyType type)
        {
            return null;
        }
    }
}