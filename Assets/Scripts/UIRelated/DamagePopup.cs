using UnityEngine;
using TMPro;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public enum PopupType
    {
        Unassigned, Damage, Heal, Critic, Invulnerable
    }

    [System.Serializable]
    public class PopupTextSetting
    {
        [Header("TEXT SETTINGS")]
        public string Name;
        [Range(0,1)] public float FontSize = 1;
        public PopupType Type = PopupType.Unassigned;
        public Color Color = Color.white;
        public bool IsBold = false;
        public bool IsUpperCase = false;

        [Header("POSITION SETTINGS")]
        public Vector3 Offset = Vector3.zero;
    }

    public class DamagePopup : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float maxYDistance = 2f;
        [SerializeField] private float minXDistance = -2f;
        [SerializeField] private float maxXDistance = 2f;
        public List<PopupTextSetting> PopupTextSettings = new();

        private float _randomYDestination;
        private float _randomXDestination;

        private TMP_Text _damageValueText;

        public static DamagePopup CreateDamagePopup(GameObject prefab, Transform pivot, float value, PopupType type)
        {
            if (!prefab) 
            {
                Debug.LogError("No damage popup prefab set.");
                return null; 
            }

            // Create damage popup.
            GameObject popupInstance = Instantiate(prefab);

            DamagePopup damagePopup = popupInstance.GetComponent<DamagePopup>();
            damagePopup.Set(pivot, value, type);

            return damagePopup;
        }

        private void Awake() => Initialize();

        private void Start() => LookAtPlayerCamera();

        private void Update() => FloatingMovement();

        private void LateUpdate() => LookAtPlayerCamera();

        private void Initialize()
        {
            _damageValueText = transform.GetChild(0).GetComponentInChildren<TMP_Text>();

            LookAtPlayerCamera();

            _randomYDestination = Random.Range(0, maxYDistance);
            _randomXDestination = Random.Range(minXDistance, maxXDistance);
        }

        private void Set(Transform pivot, float damageValue, PopupType type)
        {
            SetPosition(pivot, GetPopupTextSetting(type));

            // Set text value & look according to the popup type.
            switch (type)
            {
                case PopupType.Damage:
                    SetDamageText(damageValue.ToString(), GetPopupTextSetting(PopupType.Damage));
                    break;
                case PopupType.Heal:
                    SetDamageText(damageValue.ToString(), GetPopupTextSetting(PopupType.Heal));
                    break;
                case PopupType.Critic:
                    SetDamageText(damageValue.ToString(), GetPopupTextSetting(PopupType.Critic));
                    break;
                case PopupType.Invulnerable:
                    SetDamageText("Invulnerable", GetPopupTextSetting(PopupType.Invulnerable));
                    break;
            }
        }

        private void SetPosition(Transform pivot, PopupTextSetting popupTextSetting)
        {
            Vector3 spawnPosition;

            // Create nma reference to get its height.
            UnityEngine.AI.NavMeshAgent navMeshAgent = pivot.GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (popupTextSetting.Offset != Vector3.zero)
            {
                spawnPosition = new(
                    pivot.position.x + popupTextSetting.Offset.x, 
                    pivot.position.y + popupTextSetting.Offset.y, 
                    pivot.position.z + popupTextSetting.Offset.z);
            }
            else
            {
                // Default spawn pos Y + height
                spawnPosition = new(pivot.position.x, pivot.position.y + navMeshAgent.height, pivot.position.z);
            }

            transform.position = spawnPosition;
        }

        private void SetDamageText(string stringValue, PopupTextSetting popupTextSetting)
        {
            if (popupTextSetting.IsUpperCase) { _damageValueText.SetText(stringValue.ToUpper()); }
            else { _damageValueText.SetText(stringValue); }

            _damageValueText.fontSize = popupTextSetting.FontSize;
            _damageValueText.fontStyle = popupTextSetting.IsBold ? FontStyles.Bold : FontStyles.Normal;
            _damageValueText.color = popupTextSetting.Color;
        }

        private void LookAtPlayerCamera()
        {
            transform.LookAt(
                transform.position + Helper.GetMainCamera().transform.rotation * Vector3.forward, Helper.GetMainCamera().transform.rotation * Vector3.up);
        }

        private void FloatingMovement()
        {
            transform.position += new Vector3(_randomXDestination, _randomYDestination, 0) * Time.deltaTime * speed;
        }

        private PopupTextSetting GetPopupTextSetting(PopupType popupType)
        {
            if (PopupTextSettings.Count == 0) { return null; }

            for (int i = 0; i < PopupTextSettings.Count; i++)
            {
                if (PopupTextSettings[i].Type == popupType)
                {
                    return PopupTextSettings[i];
                }
            }

            return null;
        }
    }
}