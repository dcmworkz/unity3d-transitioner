using UnityEngine;

[CreateAssetMenu(fileName = "TransitionerEditorStyles", menuName = "Lairinus/TransitionerEditorStyles")]
public class TransitionerEditorStyles : ScriptableObject
{
    public GUISkin editorSkin = null;
    public GUIStyle upArrowButtonStyle = null;
    public GUIStyle downArrowButtonStyle = null;
    public GUIStyle deleteButtonStyle = null;
    public GUIStyle phaseSelectedButtonStyle = null;
    public Color lairinusRed;
    public Color lairinusGreen;
    public Color lairinusBlue;
    public Color lairinusPurple;
}