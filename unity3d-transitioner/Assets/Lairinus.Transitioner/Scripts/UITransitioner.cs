using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitioner : MonoBehaviour
{
    [SerializeField] private GameObject _sf_targetGameObject = null;
    [SerializeField] private bool _sf_enableTransition = false;
    [SerializeField] private bool _sf_loop = false;
    public GameObject targetGameObject { get { return _sf_targetGameObject; } set { _sf_targetGameObject = value; } }
    public bool enableTransition { get { return _sf_enableTransition; } set { _sf_enableTransition = value; } }
    public bool loop { get { return _sf_loop; } set { _sf_loop = value; } }
    [SerializeField] private List<Phase> _sf_phases = new List<Phase>();

    [System.Serializable]
    public class Phase
    {
        [SerializeField] private bool _sf_enabled = true;
        [SerializeField] private string _sf_name = "New Phase";
        [SerializeField] private float _sf_delay = 0;
        [SerializeField] private float _sf_duration = 0;
        [SerializeField] private AnimationCurve _sf_lerpPlaystyleType = new AnimationCurve();
        public float duration { get { return _sf_duration; } set { _sf_duration = value; } }
        public float delay { get { return _sf_delay; } }
        public string name { get { return _sf_name; } set { _sf_name = value; } }
        public bool enabled { get { return _sf_enabled; } set { _sf_enabled = value; } }
        public AnimationCurve lerpPlaystyleType { get { return _sf_lerpPlaystyleType; } }
    }
}