using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Khynan_Coding
{
    public class LightTool : MonoBehaviour
    {
        [Header("FLICKERING SETTINGS")]
        [SerializeField] private bool _doesFlicker = false;
        [SerializeField] private float _flickeringRate = .05f;
        [Tooltip("Values alternate between 0 & 1 => 0 means lights are off, 1 lights are on.")]
        [SerializeField] private List<FlickeringProperty> _flickeringProfile = new();
        private float _currentFlickeringTimer;
        int _index = 0;

        [Header("LIGHTS SETTINGS")]
        [SerializeField] private Material _lightOnMaterial;
        [SerializeField] private Material _lightOffMaterial;
        [SerializeField] private List<Light> _lights = new();
        [SerializeField] private List<float> _lightMaxIntensityValues = new();

        // DEBUG SECTION
        private Coroutine _testCoroutine = null;
        private bool _testFlickering = false;

        [System.Serializable]
        private class FlickeringProperty
        {
            [Range(0,1)] public float FlickeringDelay = 0;
            public bool SwitchON = false;
        }

        private void Start() => Init();

        private void LateUpdate() => ProcessFlickeringRateDuration();

        void Init()
        {
            _currentFlickeringTimer = _flickeringRate;
            _index = 0;
        }

        private void ProcessFlickeringRateDuration()
        {
            if (!_doesFlicker) { return; }

            _currentFlickeringTimer -= Time.deltaTime;

            if (_currentFlickeringTimer <= 0)
            {
                _currentFlickeringTimer = _flickeringRate;
                _currentFlickeringTimer = _flickeringProfile[_index].FlickeringDelay;
                SwitchLight();
            }
        }
        
        void SwitchLight()
        {
            if (_lights.Count == 0)
            {
                Debug.LogError("No light found or profile is empty.", transform);
                return;
            }

            bool switchState = _flickeringProfile[_index].SwitchON;

            _index++;
            if (_index >= _flickeringProfile.Count) { _index = 0; }

            // Light on or off depending on switchON state.
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].enabled = switchState;

                MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

                switch (switchState)
                {
                    case true:
                        ModifyMeshRendererMaterial(meshRenderer, _lightOnMaterial);
                        break;
                    case false:
                        ModifyMeshRendererMaterial(meshRenderer, _lightOffMaterial);
                        break;
                }
            }
        }

        void ModifyMeshRendererMaterial(MeshRenderer meshRenderer, Material material)
        {
            meshRenderer.sharedMaterial = material;
        }

        private void GetAllLightsInObject()
        {
            if (transform.childCount == 0)
            {
                Debug.LogError("Light would not be found, there is no child.", transform);
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).TryGetComponent(out Light light)) { continue; }

                if (!_lightMaxIntensityValues.Contains(light.intensity)) 
                {
                    _lightMaxIntensityValues.Add(light.intensity);
                }

                if (!_lights.Contains(light))
                {
                    _lights.Add(light);
                }
            }
        }

        private void SetupLightsThatFlicker()
        {
            if (_lights.Count == 0) { return; }

            for (int i = 0; i < _lights.Count; i++)
            {
                if (_doesFlicker)
                {
                    _lights[i].lightmapBakeType = LightmapBakeType.Realtime;
                    _lights[i].bounceIntensity = 0;
                    continue;
                }

                _lights[i].lightmapBakeType = LightmapBakeType.Mixed;
                _lights[i].bounceIntensity = 1;
            }
        }

        private void OnValidate()
        {
            GetAllLightsInObject();
            SetupLightsThatFlicker();
        }
    }
}