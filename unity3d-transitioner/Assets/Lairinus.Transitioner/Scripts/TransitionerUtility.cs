using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Lairinus.Transitions.Internal
{
    [ExecuteInEditMode]
    public class TransitionerUtility : MonoBehaviour
    {
        private Dictionary<Type, AvailableMemberTypes> _typesDictionary = new Dictionary<Type, AvailableMemberTypes>();
        public Dictionary<Type, AvailableMemberTypes> typesDictionary { get { return _typesDictionary; } }
        private Dictionary<AvailableMemberTypes, Type> _reverseTypeDictionary = new Dictionary<AvailableMemberTypes, Type>();
        public Dictionary<AvailableMemberTypes, Type> reverseTypeDictionary { get { return _reverseTypeDictionary; } }

        public enum AvailableMemberTypes
        {
            Integer = 0,
            Boolean = 1,
            Float = 2,
            String = 3,
            Color = 4,
            ObjectReference = 5,
            LayerMask = 6,
            Enum = 7,
            Vector2 = 8,
            Vector3 = 9,
            Vector4 = 10,
            ArraySize = 12,
            Quaternion = 17,
            FixedBufferSize = 19,
            Vector2Int = 20,
            Vector3Int = 21,
        }

        public static string GetAvailableMemberName(int availableMemberType)
        {
            return Enum.GetName(typeof(AvailableMemberTypes), availableMemberType);
        }

        private static TransitionerUtility _instance = null;

        public static TransitionerUtility GetInstance()
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TransitionerUtility>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("Lairinus.TransitionerUtility");
                    _instance = go.AddComponent<TransitionerUtility>();
                }
                _instance.PopulateTypeDictionary();
            }

            return _instance;
        }

        public static bool CanBeLerped(AvailableMemberTypes type)
        {
            switch (type)
            {
                case AvailableMemberTypes.Boolean:
                case AvailableMemberTypes.Enum:
                    return false;

                default:
                    return true;
            }
        }

        private void PopulateTypeDictionary()
        {
            _typesDictionary = new Dictionary<Type, AvailableMemberTypes>();
            _typesDictionary.Add(typeof(int), AvailableMemberTypes.Integer);
            _typesDictionary.Add(typeof(bool), AvailableMemberTypes.Boolean);
            _typesDictionary.Add(typeof(float), AvailableMemberTypes.Float);
            _typesDictionary.Add(typeof(string), AvailableMemberTypes.String);
            _typesDictionary.Add(typeof(Color), AvailableMemberTypes.Color);
            _typesDictionary.Add(typeof(Vector2), AvailableMemberTypes.Vector2);
            _typesDictionary.Add(typeof(Vector3), AvailableMemberTypes.Vector3);
            _typesDictionary.Add(typeof(Vector4), AvailableMemberTypes.Vector4);

            _reverseTypeDictionary.Add(AvailableMemberTypes.Integer, typeof(int));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Boolean, typeof(bool));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Float, typeof(float));
            _reverseTypeDictionary.Add(AvailableMemberTypes.String, typeof(string));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Color, typeof(Color));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Vector2, typeof(Vector2));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Vector3, typeof(Vector3));
            _reverseTypeDictionary.Add(AvailableMemberTypes.Vector4, typeof(Vector4));
        }

        public static T GetObject<T>(string str)
        {
            Type type = typeof(T);
            if (!GetInstance().typesDictionary.ContainsKey(type))
                return (T)new object();

            AvailableMemberTypes memberType = GetInstance().typesDictionary[type];
            switch (memberType)
            {
                case AvailableMemberTypes.Vector2:
                    return (T)Convert.ChangeType(ConvertStringToVector2(str), type);

                case AvailableMemberTypes.Vector3:
                    return (T)Convert.ChangeType(ConvertStringToVector3(str), type);

                case AvailableMemberTypes.Vector4:
                    return (T)Convert.ChangeType(ConvertStringToVector4(str), type);

                case AvailableMemberTypes.String:
                    return (T)Convert.ChangeType(str, type);

                case AvailableMemberTypes.Integer:
                    return (T)Convert.ChangeType(int.Parse(str), type);

                case AvailableMemberTypes.Float:
                    return (T)Convert.ChangeType(float.Parse(str), type);

                case AvailableMemberTypes.Boolean:
                    {
                        bool tryparse = false;
                        bool.TryParse(str, out tryparse);
                        return (T)Convert.ChangeType(tryparse, type);
                    }

                case AvailableMemberTypes.Color:
                    return (T)Convert.ChangeType(ConvertStringToColor(str), type);

                default:
                    return (T)Convert.ChangeType(new object(), type);
            }
        }

        private static Vector3 ConvertStringToVector3(string sVector)
        {
            if (sVector == null || sVector == "")
                return new Vector3();

            // Remove the parentheses
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            // split the items
            string[] sArray = sVector.Split(',');

            // store as a Vector3
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));

            return result;
        }

        private static Color ConvertStringToColor(string sColor)
        {
            if (sColor == null || sColor == "")
                return Color.white;

            // expected string input is "RGBA(0.000,0.000,0.000,0.000)"
            if (sColor.StartsWith("RGBA(") && sColor.EndsWith(")"))
            {
                sColor = sColor.Substring(5, sColor.Length - 6);
            }
            string[] sArray = sColor.Split(',');
            Color result = new Color(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]),
                float.Parse(sArray[3]));

            return result;
        }

        private static Vector4 ConvertStringToVector4(string sVector)
        {
            if (sVector == null || sVector == "")
                return new Vector4();

            // expected string input is "(0,0,0)"
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            string[] sArray = sVector.Split(',');
            Vector4 result = new Vector4(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]),
                float.Parse(sArray[3]));

            return result;
        }

        private static Vector2 ConvertStringToVector2(string sVector)
        {
            if (sVector == null || sVector == "")
                return new Vector2();

            // expected string input is "(0,0)"
            if (sVector.StartsWith("(") && sVector.EndsWith(")"))
            {
                sVector = sVector.Substring(1, sVector.Length - 2);
            }

            string[] sArray = sVector.Split(',');
            Vector2 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]));

            return result;
        }

        private void Start()
        {
        }
    }
}