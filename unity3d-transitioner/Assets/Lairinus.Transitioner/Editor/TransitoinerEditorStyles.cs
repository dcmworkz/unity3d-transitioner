using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "TransitionerData", menuName = "Lairinus/TransitionerEditorStyles", order = 1)]
public class TransitionerEditorStyles : ScriptableObject
{
    public GUISkin editorSkin = null;
    public GUIStyle upArrowButtonStyle = null;
    public GUIStyle downArrowButtonStyle = null;
}