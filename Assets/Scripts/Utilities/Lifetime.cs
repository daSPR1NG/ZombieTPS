using System;
using System.Collections;
using UnityEngine;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class Lifetime : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [Tooltip("Delay after which the vfx is hidden or destroyed.")]
        [SerializeField] private float lifetime = 1f;

        [Tooltip("Determines wether if you want to destroy or hide the vfx that has been created.")]
        [SerializeField] private bool needsToBeDestroyed = false;

        private float _currentLifetimeValue = 0;
        
        private void OnEnable() => Initialize();

        private void Update() => ProcessLifetimeTimer(needsToBeDestroyed);

        private void Initialize()
        {
            _currentLifetimeValue = lifetime;
        }

        private void ProcessLifetimeTimer(bool doDestroy)
        {
            _currentLifetimeValue -= Time.deltaTime;

            if (!(_currentLifetimeValue <= 0)) { return; }

            _currentLifetimeValue = 0;

            if (doDestroy)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
        }

        public void SetLifeTime(float delta)
        {
            lifetime = delta;
        }
        
        public void SetDestroyState(bool value)
        {
            needsToBeDestroyed = value;
        }
    }
}