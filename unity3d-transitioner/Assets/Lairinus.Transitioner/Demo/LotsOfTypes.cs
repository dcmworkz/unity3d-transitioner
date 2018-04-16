using System.Reflection;
using UnityEngine;

public class LotsOfTypes : MonoBehaviour
{
    public Vector3 vector3Prop { get; set; }
    public Vector4 vector4pProp = new Vector4();
    public Color newColorProp;
    public float floatValue = 10;
    public bool newBoolProp = false;
    public int intType = 10;
    public string stringValue = "";
}