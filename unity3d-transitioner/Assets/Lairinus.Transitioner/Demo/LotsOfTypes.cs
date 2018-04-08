using System.Reflection;
using UnityEngine;

public class LotsOfTypes : MonoBehaviour
{
    public Vector3 vector3Prop { get; set; }
    public Vector4 vector4pProp = new Vector4();
    public Color newColorProp;
    public bool newBoolProp = false;

    public void Start()
    {
        foreach (FieldInfo componentFieldInfo in this.GetType().GetFields())
        {
            if (componentFieldInfo.Name == "vector4pProp")
            {
                Vector4 prop = Vector4.one;
                object obj = prop;
                componentFieldInfo.SetValue(this, obj);
                Debug.Log(vector4pProp.ToString());
            }
        }
    }
}