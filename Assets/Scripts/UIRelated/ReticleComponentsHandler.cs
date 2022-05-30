using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Khynan_Coding
{
    public class ReticleComponentsHandler : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private bool _hidesCrosshairOnStart = false;

        [Header("SPACING SETTINGS")]
        [Range(0, 100)][SerializeField] private float _spacing = 0f;
        [SerializeField] private float _spacingIncreaseSpeedMultiplier = 5f;
        [SerializeField] private float _spacingDecreasingSpeedMultiplier = 5f;
        [SerializeField] private float _spacingLimit = 40f;
        private float _startingSpacing;

        [Header("SIZE SETTINGS")]
        [Range(0, 40)][SerializeField] private float _width = 0f;
        [Range(0, 40)][SerializeField] private float _height = 0f;

        List<RectTransform> _rectTransforms = new();

        private Transform _upperComponent;
        private Transform _rightComponent;
        private Transform _lowerComponent;
        private Transform _leftComponent;

        private void OnEnable() => Actions.OnShooting += IncreaseSpacing;
        private void OnDisable()
        {
            Actions.OnShooting -= IncreaseSpacing;

            ResetSpacingValue();
        }

        void Start() => Init();

        private void LateUpdate() => DecreaseSpacing();

        void Init()
        {
            AssignComponents();

            SetComponentsSize();
            SetComponentsPosition();

            _startingSpacing = _spacing;

            if (_hidesCrosshairOnStart) { gameObject.SetActive(false); } 
        }

        #region Components settings
        private void AssignComponents()
        {
            _upperComponent = transform.GetChild(0);
            RectTransform upperRectTransform = _upperComponent.GetComponent<RectTransform>();
            if (!_rectTransforms.Contains(upperRectTransform)) { _rectTransforms.Add(upperRectTransform); }

            _rightComponent = transform.GetChild(1);
            RectTransform rightRectTransform = _rightComponent.GetComponent<RectTransform>();
            if (!_rectTransforms.Contains(rightRectTransform)) { _rectTransforms.Add(rightRectTransform); }

            _lowerComponent = transform.GetChild(2);
            RectTransform lowerRectTransform = _lowerComponent.GetComponent<RectTransform>();
            if (!_rectTransforms.Contains(lowerRectTransform)) { _rectTransforms.Add(lowerRectTransform); }

            _leftComponent = transform.GetChild(3);
            RectTransform leftRectTransform = _leftComponent.GetComponent<RectTransform>();
            if (!_rectTransforms.Contains(leftRectTransform)) { _rectTransforms.Add(leftRectTransform); }
        }

        private void SetComponentsSize()
        {
            if (_rectTransforms.Count == 0) { return; }

            Vector2 deltaSize = new(_height, _width);

            for (int i = 0; i < _rectTransforms.Count; i++)
            {
                if (_rectTransforms[i].sizeDelta == deltaSize) { continue; }

                _rectTransforms[i].sizeDelta = deltaSize;
            }
        }

        private void SetComponentsPosition()
        {
            _upperComponent.localPosition = new(0, _spacing);
            _rightComponent.localPosition = new(_spacing, 0);
            _lowerComponent.localPosition = new(0, -_spacing);
            _leftComponent.localPosition = new(-_spacing, 0);
        }

        public void SetComponentsColor(Color color)
        {
            if (_rectTransforms.Count == 0) { return; }

            Image reticleComponentImage;

            for (int i = 0; i < _rectTransforms.Count; i++)
            {
                reticleComponentImage = _rectTransforms[i].GetComponent<Image>();

                if (reticleComponentImage.color == color) { continue; }

                reticleComponentImage.color = color;
            }
        }
        #endregion

        private void IncreaseSpacing(Weapon weapon)
        {
            if (_spacing >= _spacingLimit)
            {
                _spacing = _spacingLimit;
                return;
            }

            _spacing += (weapon.GetImpulseForce() * _spacingIncreaseSpeedMultiplier);
            _spacing = Mathf.Clamp(_spacing, _startingSpacing, _spacingLimit);

            SetComponentsPosition();
        }

        private void DecreaseSpacing()
        {
            if (_spacing <= _startingSpacing) 
            {
                _spacing = _startingSpacing;
                return; 
            }

            _spacing -= Time.deltaTime * _spacingDecreasingSpeedMultiplier;
            _spacing = Mathf.Clamp(_spacing, _startingSpacing, _spacingLimit);

            SetComponentsPosition();
        }

        private void ResetSpacingValue()
        {
            _spacing = _startingSpacing;

            SetComponentsPosition();
        }
        private void OnValidate()
        {
            AssignComponents();

            SetComponentsSize();
            SetComponentsPosition();
        }
    }
}