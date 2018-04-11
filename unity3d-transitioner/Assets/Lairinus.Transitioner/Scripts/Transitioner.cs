using System.Collections.Generic;
using UnityEngine;

namespace Lairinus.Transitions
{
    public class Transitioner : MonoBehaviour
    {
        [SerializeField] private GameObject _sf_targetGameObject = null;
        [SerializeField] private bool _sf_disableTransition = false;
        [SerializeField] private bool _sf_loop = false;
        public GameObject targetGameObject { get { return _sf_targetGameObject; } set { _sf_targetGameObject = value; } }
        public bool enableTransition { get { return _sf_disableTransition; } set { _sf_disableTransition = value; } }
        public bool loop { get { return _sf_loop; } set { _sf_loop = value; } }
        [SerializeField] private List<Phase> _sf_phases = new List<Phase>();
        public List<Phase> phases { get { return _sf_phases; } }

        [System.Serializable]
        public class Phase
        {
            [SerializeField] private bool _sf_disabled = false;
            [SerializeField] private string _sf_name = "New Phase";
            [SerializeField] private float _sf_delay = 0;
            [SerializeField] private float _sf_duration = 0;
            [SerializeField] private AnimationCurve _sf_lerpPlaystyleType = new AnimationCurve();
            [SerializeField] private List<PhaseMember> _sf_reflectedMembers = new List<PhaseMember>();
            public float duration { get { return _sf_duration; } set { _sf_duration = value; } }
            public float delay { get { return _sf_delay; } }
            public string name { get { return _sf_name; } set { _sf_name = value; } }
            public bool disabled { get { return _sf_disabled; } set { _sf_disabled = value; } }
            public AnimationCurve lerpPlaystyleType { get { return _sf_lerpPlaystyleType; } }
            public List<PhaseMember> reflectedMembers { get { return _sf_reflectedMembers; } }

            public void UpdatePhaseTransition(float lerpTime, float actualTime)
            {
                foreach (PhaseMember phaseMember in reflectedMembers)
                {
                    if (phaseMember != null)
                    {
                        float evaluatedValue = _sf_lerpPlaystyleType.Evaluate(actualTime);
                        phaseMember.UpdatePhaseMember(evaluatedValue, actualTime);
                    }
                }
            }
        }

        private float _currentLerpTime = 0;
        private int _currentPhaseIndex = 0;
        private Phase _currentPhase = null;

        public bool IsFinishedPlaying()
        {
            if (_currentPhaseIndex >= _sf_phases.Count && _currentLerpTime >= 1)
                return true;
            return false;
        }

        public void UpdateTransition(float updateInterval)
        {
            if (IsFinishedPlaying() && !_sf_loop)
                return;

            UpdateTransition_CheckReset();
            UpdateTransition_SetAndUpdatePhase();

            if (_currentPhase != null)
                _currentLerpTime += updateInterval / _currentPhase.duration;
        }

        private void UpdateTransition_CheckReset()
        {
            if (IsFinishedPlaying() && _sf_loop)
            {
                _currentLerpTime = 0;
                _currentPhaseIndex = 0;
            }
        }

        private void UpdateTransition_SetAndUpdatePhase()
        {
            _currentPhase = null;
            if (_currentPhaseIndex < _sf_phases.Count)
                _currentPhase = _sf_phases[_currentPhaseIndex];
            else if (_sf_phases.Count > 0)
                _currentPhase = _sf_phases[_sf_phases.Count - 1];

            if (_currentLerpTime <= 1)
            {
                if (_currentPhase != null)
                    _currentPhase.UpdatePhaseTransition(_currentLerpTime, _currentLerpTime * _currentPhase.duration);
            }
            else
            {
                _currentPhase.UpdatePhaseTransition(1, _currentPhase.duration);
                _currentLerpTime = 0;
                _currentPhaseIndex++;
            }
        }

        private void Update()
        {
            UpdateTransition(Time.deltaTime);
        }
    }
}