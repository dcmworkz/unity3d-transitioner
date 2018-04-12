using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Lairinus.Transitions.Internal
{
    [ExecuteInEditMode]
    public class Utility : MonoBehaviour
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

        public static object GetLerpedValue(string currentValueString, string finalValueString, AvailableMemberTypes memberType, float lerpedTime)
        {
            switch (memberType)
            {
                case AvailableMemberTypes.Color:
                    {
                        Color final = new Color();
                        Color currentValueColor = GetObject<Color>(currentValueString);
                        Color finalValueColor = GetObject<Color>(finalValueString);
                        final = Color.Lerp(currentValueColor, finalValueColor, lerpedTime);
                        return final;
                    }

                case AvailableMemberTypes.Float:
                    {
                        float final = 0;
                        float currentValueFloat = float.Parse(currentValueString);
                        float finalValueFloat = float.Parse(finalValueString);
                        final = Mathf.Lerp(currentValueFloat, finalValueFloat, lerpedTime);
                        return final;
                    }

                case AvailableMemberTypes.Vector2:
                    {
                        Vector2 final = new Vector2();
                        Vector2 currentValueVector2 = GetObject<Vector2>(currentValueString);
                        Vector2 finalValueVector2 = GetObject<Vector2>(currentValueString);
                        final = Vector2.Lerp(currentValueVector2, finalValueVector2, lerpedTime);
                        return final;
                    }

                case AvailableMemberTypes.Vector3:
                    {
                        Vector3 final = new Vector3();
                        Vector3 currentValueVector3 = GetObject<Vector3>(currentValueString);
                        Vector3 finalValueVector3 = GetObject<Vector3>(currentValueString);
                        final = Vector3.Lerp(currentValueVector3, finalValueVector3, lerpedTime);
                        return final;
                    }

                case AvailableMemberTypes.Vector4:
                    {
                        Vector4 final = new Vector4();
                        Vector4 currentValueVector4 = GetObject<Vector4>(currentValueString);
                        Vector4 finalValueVector4 = GetObject<Vector4>(currentValueString);
                        final = Vector4.Lerp(currentValueVector4, finalValueVector4, lerpedTime);
                        return final;
                    }

                case AvailableMemberTypes.Integer:
                    {
                        int final = 0;
                        float currentInt = GetObject<int>(currentValueString);
                        float finalInt = GetObject<int>(currentValueString);
                        final = Mathf.RoundToInt(Mathf.Lerp(currentInt, finalInt, lerpedTime));
                        return final;
                    }

                default:
                    return new object();
            }
        }

        public static string GetAvailableMemberName(int availableMemberType)
        {
            return Enum.GetName(typeof(AvailableMemberTypes), availableMemberType);
        }

        private static Utility _instance = null;

        public static Utility GetInstance()
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Utility>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("Lairinus.TransitionerUtility");
                    _instance = go.AddComponent<Utility>();
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

        public static object GetObject(Type _type, string str)
        {
            try
            {
                Type type = _type;
                if (!GetInstance().typesDictionary.ContainsKey(type))
                    return new object();

                AvailableMemberTypes memberType = GetInstance().typesDictionary[type];
                switch (memberType)
                {
                    case AvailableMemberTypes.Vector2:
                        return ConvertStringToVector2(str);

                    case AvailableMemberTypes.Vector3:
                        return ConvertStringToVector3(str);

                    case AvailableMemberTypes.Vector4:
                        return ConvertStringToVector4(str);

                    case AvailableMemberTypes.String:
                        return str;

                    case AvailableMemberTypes.Integer:
                        return int.Parse(str);

                    case AvailableMemberTypes.Float:
                        return float.Parse(str);

                    case AvailableMemberTypes.Boolean:
                        {
                            bool tryparse = false;
                            bool.TryParse(str, out tryparse);
                            return tryparse;
                        }

                    case AvailableMemberTypes.Color:
                        return ConvertStringToColor(str);

                    default:
                        return new object();
                }
            }
            catch
            {
                return new object();
            }
        }

        private static Vector3 ConvertStringToVector3(string sVector)
        {
            try
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
            catch
            {
                return new Vector3();
            }
        }

        private static Color ConvertStringToColor(string sColor)
        {
            try
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
            catch
            {
                return Color.white;
            }
        }

        private static Vector4 ConvertStringToVector4(string sVector)
        {
            try
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
            catch
            {
                return new Vector4();
            }
        }

        private static Vector2 ConvertStringToVector2(string sVector)
        {
            try
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
            catch
            {
                return new Vector2();
            }
        }

        private static Dictionary<int, Transitioner> _transitionsToStart = new Dictionary<int, Transitioner>();
        private static Dictionary<int, Transitioner> _transitionsToStop = new Dictionary<int, Transitioner>();
        private static Dictionary<int, Transitioner> _transitionsCurrentlyRunning = new Dictionary<int, Transitioner>();

        public void StartTransitioner(Transitioner transitioner)
        {
            if (transitioner == null)
                return;

            int key = transitioner.GetInstanceID();
            if (!_transitionsToStart.ContainsKey(key) && !_transitionsToStop.ContainsKey(key) && !_transitionsCurrentlyRunning.ContainsKey(key))
                _transitionsToStart.Add(key, transitioner);
        }

        public void StopTransitioner(Transitioner transitioner)
        {
            if (transitioner == null)
                return;

            int key = transitioner.GetInstanceID();
            if (_transitionsToStart.ContainsKey(key))
                _transitionsToStart.Remove(key);
            else if (!_transitionsToStop.ContainsKey(key))
                _transitionsToStop.Add(key, transitioner);
        }

        private void Update()
        {
            List<int> transitionsToStop = _transitionsToStop.Keys.ToList();
            foreach (int transitionToStop in transitionsToStop)
            {
                if (_transitionsCurrentlyRunning.ContainsKey(transitionToStop))
                    _transitionsCurrentlyRunning.Remove(transitionToStop);
            }
            _transitionsToStop.Clear();

            foreach (KeyValuePair<int, Transitioner> kvp in _transitionsToStart)
            {
                if (kvp.Value != null && !_transitionsCurrentlyRunning.ContainsKey(kvp.Key))
                    _transitionsCurrentlyRunning.Add(kvp.Key, kvp.Value);
            }
            _transitionsToStart.Clear();

            float deltaTime = Time.deltaTime;
            List<Transitioner> runningTransitions = _transitionsCurrentlyRunning.Values.ToList();
            runningTransitions.ForEach(x => x.UpdateTransition(deltaTime));
        }
    }
}