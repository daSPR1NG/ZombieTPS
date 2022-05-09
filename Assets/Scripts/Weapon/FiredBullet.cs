using UnityEngine;

namespace Khynan_Coding
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class FiredBullet : MonoBehaviour
    {
        [SerializeField] private float _minRandomMass = 0.15f;
        [SerializeField] private float _maxRandomMass = 5f;
        [SerializeField] private float _appliedForce = 2f;
        [SerializeField] private ForceMode _forceMode;

        private Rigidbody _rigidbody;

        private void Awake() => Init();

        private void OnEnable() => ApplyForceMode();

        void Init()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.mass = Random.Range(_minRandomMass, _maxRandomMass);

            SetRotation();
        }

        private void SetRotation()
        {
            transform.LookAt(Helper.GetMainCamera().transform.GetChild(0), Vector3.up);
        }

        private void ApplyForceMode()
        {
            var cameraRight = Helper.GetMainCamera().transform.right;

            _rigidbody.AddRelativeForce(transform.position + cameraRight * _appliedForce, _forceMode);
        }
    }
}