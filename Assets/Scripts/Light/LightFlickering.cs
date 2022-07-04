using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    // NOTES :
    // 1. Make an editor > when a preset is selected, the list is not modifiable, unless you chose so;
    // 2. The only preset that always allow you to directly change the profile is -CUSTOM-;
    // 3. Add a warning notes, when GUI is disabled, the preview cannot be seen in the scene view.
    // 4. Add something to save the last custom preset made,
    // so you can go to previous if you want and you can save the one you've made,
    // the notif saying there is a custom preset saved for this object (object, flickering profile)
    // 5. Lights components are not detected when they're placed as children of other object > need to go through every child of child etc...
    // 6. Add components to objects holding light component to subscribe OnValueChanged

    public enum FlickeringPreset
    {
        Unassigned, Custom, LowFlicker, MediumFlicker, HighFlicker, VeryHighFlicker
    }

    [DisallowMultipleComponent]
    public class LightFlickering : MonoBehaviour
    {
        [Header("FLICKERING SETTINGS")]
        [SerializeField] private bool _doesFlicker = false;
        [SerializeField] private FlickeringPreset _flickeringPreset = FlickeringPreset.Custom;
        [SerializeField] private float _flickeringDefaultRate = .05f;
        [Space(10), SerializeField] private List<FlickeringProperty> _flickeringProfile = new();

        private float _currentFlickeringTimer;
        int _index = 0;

        [Space(5), Header("LIGHTS SETTINGS")]
        [SerializeField] private Material _lightOnMaterial;
        [SerializeField] private Material _lightOffMaterial;
        [Space(10), SerializeField] private List<LightData> _lightDatas = new();

        private List<float> _lightMaxIntensityValues = new();

        [Space(20), Header("DEBUG SETTINGS")]
        public bool TestFlickering = false;

        #region Nested classes - Flickering Property & Light Data
        [System.Serializable]
        private class FlickeringProperty
        {
            [HideInInspector] public string Name = "Profile Assigned Property";
            public bool SwitchON = false;
            [Range(0,1)] public float FlickeringDelay = 0;

            public FlickeringProperty(float flickeringDelay, bool switchON)
            {
                FlickeringDelay = flickeringDelay;
                SwitchON = switchON;
            }
        }

        [System.Serializable]
        private class LightData
        {
            public Light Light;
            public bool SwitchON = false;

            public LightData(Light light, bool swicthON)
            {
                Light = light;
                SwitchON = swicthON;
            }
        }
        #endregion

        private void Start() => Init();

        private void LateUpdate() => ProcessFlickeringRateDuration();

        #region Setup
        void Init() => SetToDefault();

        /// <summary> Retrieve all the light components of transform children. </summary>
        private void GetAllLightComponentsInChildren()
        {
            if (!_doesFlicker) { return; }

            if (transform.childCount == 0)
            {
                Debug.LogError("Light would not be found, there is no child.", transform);
                return;
            }

            int amount = 0;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (!transform.GetChild(i).TryGetComponent(out Light light)
                    || !transform.GetChild(i).gameObject.activeInHierarchy)
                {
                    continue;
                }

                LightData newLightData = new(light, true);
                amount++;

                if (_lightDatas.Count < amount)
                {
                    _lightDatas.Add(newLightData);
                    _lightMaxIntensityValues.Add(light.intensity);
                }
            }
        }

        private void MatchLightStateWithFirstFlickeringProfileProperty()
        {
            if (_lightDatas.Count == 0 || _flickeringProfile.Count == 0) { return; }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            bool switchState = _flickeringProfile[0].SwitchON;

            if (_lightDatas[0].Light.enabled != switchState) { _lightDatas[0].Light.enabled = switchState; }
            
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
        #endregion

        /// <summary> Charged to change lights state upon runtime, according to a timer. </summary>
        private void ProcessFlickeringRateDuration()
        {
            if (!_doesFlicker) { return; }

            if (_flickeringProfile.Count == 0) 
            {
                Debug.LogError("No flickering profile found", gameObject);
                return; 
            }

            _currentFlickeringTimer -= Time.deltaTime;

            if (_currentFlickeringTimer <= 0)
            {
                _currentFlickeringTimer = _flickeringDefaultRate;
                _currentFlickeringTimer = _flickeringProfile[_index].FlickeringDelay;
                SwitchLightState();
            }
        }

        /// <summary> Switch lights enabled state and MeshRenderer material to match - Light ON /OFF -. </summary>
        void SwitchLightState()
        {
            if (!_doesFlicker) { return; }

            if (_lightDatas.Count == 0 || _flickeringProfile.Count == 0)
            {
                Debug.LogError("No light found or profile is empty.", transform);
                return;
            }

            bool switchState = _flickeringProfile[_index].SwitchON;

            _index++;
            if (_index >= _flickeringProfile.Count) { _index = 0; }

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

            // Light on or off depending on switchON state.
            for (int i = 0; i < _lightDatas.Count; i++)
            {
                if (_lightDatas[i].Light.enabled != switchState) { _lightDatas[i].Light.enabled = switchState; }

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

        #region Utilities
        /// <summary> Reset important parameters of this script to default. </summary>
        private void SetToDefault()
        {
            if (_index != 0) { _index = 0; }

            if (_currentFlickeringTimer != _flickeringDefaultRate) { _currentFlickeringTimer = _flickeringDefaultRate; }

            MatchLightStateWithFirstFlickeringProfileProperty();
        }
        /// <summary> Replace given MeshRenderer's materiel with given material. </summary>
        private void ModifyMeshRendererMaterial(MeshRenderer meshRenderer, Material material)
        {
            if (!meshRenderer) { return; }

            meshRenderer.sharedMaterial = material;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR
        private void OnValidate()
        {
            GetAllLightComponentsInChildren();
            //SwitchLightState();
            ManageFlickeringPresetsInEditor();

            if (!TestFlickering)
            {
                SetToDefault();
                MatchLightStateWithFirstFlickeringProfileProperty();
            }
        }
        
        [HideInInspector] public FlickeringPreset flickeringPreset;

        /// <summary> Update variables as flickering profile changes. </summary>
        private void ManageFlickeringPresetsInEditor()
        {
            if (flickeringPreset != _flickeringPreset && !Application.isPlaying)
            {
                _flickeringProfile.Clear();
                SetToDefault();
            }

            //Debug.Log("AVANT " + flickeringPreset + " / " + _flickeringPreset);

            flickeringPreset = _flickeringPreset;

            //Debug.Log("APRES " + flickeringPreset + " / " + _flickeringPreset);

            switch (_flickeringPreset)
            {
                case FlickeringPreset.Custom:
                    break;

                case FlickeringPreset.LowFlicker:
                    if (_flickeringProfile.Count == 3) { return; }

                    _flickeringProfile.Add(new FlickeringProperty(0.25f, true));
                    _flickeringProfile.Add(new FlickeringProperty(0.25f, false));
                    _flickeringProfile.Add(new FlickeringProperty(0.25f, true));
                    break;

                case FlickeringPreset.MediumFlicker:
                    if (_flickeringProfile.Count == 3) { return; }

                    _flickeringProfile.Add(new FlickeringProperty(0.15f, true));
                    _flickeringProfile.Add(new FlickeringProperty(0.15f, false));
                    _flickeringProfile.Add(new FlickeringProperty(0.15f, true));
                    break;

                case FlickeringPreset.HighFlicker:
                    if (_flickeringProfile.Count == 3) { return; }

                    _flickeringProfile.Add(new FlickeringProperty(0.05f, true));
                    _flickeringProfile.Add(new FlickeringProperty(0.05f, false));
                    _flickeringProfile.Add(new FlickeringProperty(0.05f, true));
                    break;

                case FlickeringPreset.VeryHighFlicker:
                    if (_flickeringProfile.Count == 3) { return; }

                    _flickeringProfile.Add(new FlickeringProperty(0.015f, true));
                    _flickeringProfile.Add(new FlickeringProperty(0.015f, false));
                    _flickeringProfile.Add(new FlickeringProperty(0.015f, true));
                    break;
            }
        }

        private void TestFlickeringProfile()
        {
            if (UnityEditor.Selection.Contains(gameObject) && TestFlickering)
            {
                ProcessFlickeringRateDuration();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
                Debug.Log("Test flickering");
            }
        }

        private void OnDrawGizmos()
        {

            if (!Application.isEditor || Application.isPlaying) { return; }

            if (!UnityEditor.Selection.Contains(gameObject))
            {
                TestFlickering = false;
                SetToDefault();
                return;
            }

            TestFlickeringProfile();
#endif
        }
        #endregion
    }
}