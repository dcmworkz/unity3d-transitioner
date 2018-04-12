using System.Collections.Generic;
using UnityEngine;
using Lairinus.Transitions.Internal;
using System.Linq;

namespace Lairinus.Transitions
{
    public class Transitioner : MonoBehaviour
    {
        public bool enableTransition { get { return _sf_disableTransition; } set { _sf_disableTransition = value; } }
        public bool loop { get { return _sf_loop; } set { _sf_loop = value; } }
        public List<Phase> phases { get { return _sf_phases; } }
        public GameObject targetGameObject { get { return _sf_targetGameObject; } set { _sf_targetGameObject = value; } }

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

            bool wasDelayed = _isCurrentlyDelayed;
            UpdateTransition_SetPhase();
            UpdateTransition_UpdateDelay(updateInterval);
            if (!_isCurrentlyDelayed)
            {
                if (wasDelayed)
                    _currentLerpTime = 0;

                UpdateTransition_CheckReset();
                UpdateTransition_SetAndUpdatePhase();
                if (_currentPhase != null)
                    _currentLerpTime += updateInterval / _currentPhase.duration;
            }
        }

        [System.Serializable]
        public class Phase
        {
            public float delay { get { return _sf_delay; } }
            public bool disabled { get { return _sf_disabled; } set { _sf_disabled = value; } }
            public float duration { get { return _sf_duration; } set { _sf_duration = value; } }
            public AnimationCurve lerpPlaystyleType { get { return _sf_lerpPlaystyleType; } }
            public string name { get { return _sf_name; } set { _sf_name = value; } }
            public List<PhaseMember> reflectedMembers { get { return _sf_reflectedMembers; } }

            public void UpdatePhaseTransition(float lerpTime, float actualTime, bool isResetting)
            {
                foreach (PhaseMember phaseMember in reflectedMembers)
                {
                    if (phaseMember != null)
                    {
                        float evaluatedValue = _sf_lerpPlaystyleType.Evaluate(actualTime);
                        phaseMember.UpdatePhaseMember(evaluatedValue, actualTime, isResetting);
                    }
                }
            }

            [SerializeField] private float _sf_delay = 0;
            [SerializeField] private bool _sf_disabled = false;
            [SerializeField] private float _sf_duration = 0;
            [SerializeField] private AnimationCurve _sf_lerpPlaystyleType = new AnimationCurve();
            [SerializeField] private string _sf_name = "New Phase";
            [SerializeField] private List<PhaseMember> _sf_reflectedMembers = new List<PhaseMember>();
        }

        private float _currentLerpTime = 0;
        private Phase _currentPhase = null;
        private int _currentPhaseIndex = 0;
        private bool _isCurrentlyDelayed = false;
        [SerializeField] private bool _sf_disableTransition = false;
        [SerializeField] private bool _sf_loop = false;
        [SerializeField] private List<Phase> _sf_phases = new List<Phase>();
        [SerializeField] private GameObject _sf_targetGameObject = null;
        [SerializeField] private bool _sf_playOnAwake = false;
        private float _totalTimeDelayed = 0;

        private void UpdateTransition_SetPhase()
        {
            _currentPhase = null;
            if (_currentPhaseIndex < _sf_phases.Count)
                _currentPhase = _sf_phases[_currentPhaseIndex];
            else if (_sf_phases.Count > 0)
            {
                _currentPhase = _sf_phases[0];
                _currentPhaseIndex = 0;
            }
        }

        public void StartTransition()
        {
            Utility.GetInstance().StartTransitioner(this);
        }

        public void StopTransition(bool reset = false)
        {
            Utility.GetInstance().StopTransitioner(this);
            if (reset)
                ResetTransition();
        }

        public void ResetTransition()
        {
            _currentLerpTime = 0;
            _currentPhaseIndex = 0;
            _sf_phases.Where(x => x != null).ToList().ForEach(X => X.UpdatePhaseTransition(0, 0, true));
        }

        private void UpdateTransition_UpdateDelay(float updateInterval)
        {
            if (_currentPhase == null)
                return;

            if (_currentPhase.delay > 0 && _totalTimeDelayed < _currentPhase.delay)
            {
                _totalTimeDelayed += updateInterval;
                _isCurrentlyDelayed = true;
            }
            else
            {
                _isCurrentlyDelayed = false;
            }
        }

        private void Awake()
        {
            if (_sf_playOnAwake)
                StartTransition();
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
            if (_currentLerpTime <= 1)
            {
                if (_currentPhase != null)
                    _currentPhase.UpdatePhaseTransition(_currentLerpTime, _currentLerpTime * _currentPhase.duration, false);
            }
            else
            {
                _currentPhase.UpdatePhaseTransition(1, _currentPhase.duration, false);
                _currentLerpTime = 0;
                _isCurrentlyDelayed = false;
                _totalTimeDelayed = 0;
                _currentPhaseIndex++;
            }
        }
    }
}