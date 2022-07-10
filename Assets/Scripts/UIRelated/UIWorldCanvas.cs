using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class UIWorldCanvas : MonoBehaviour
    {
        [SerializeField] private float _rotationFrequency = 0.5f;

        private Transform _playerRef;

        private void OnEnable() => Actions.OnPlayerDeath += HideSelf;
        private void OnDisable() => Actions.OnPlayerDeath -= HideSelf;

        void Start()
        {
            Init();
            InvokeRepeating(nameof(LookAtPlayer), 1, _rotationFrequency);
        }

        void Init()
        {
            _playerRef = GameManager.Instance.ActivePlayer;
        }

        void LookAtPlayer()
        {
            Camera camera = Helper.GetMainCamera();
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward, -(camera.transform.rotation * Vector3.up));
        }

        void HideSelf()
        {
            Helper.HideGO( gameObject );
        }
    }
}