using UnityEngine;

namespace Khynan_Coding
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class FiredBullet : MonoBehaviour
    {
        [Header("PHYSICS FORCE SETTINGS")]
        [SerializeField] private float _minRandomMass = 0.15f;
        [SerializeField] private float _maxRandomMass = 5f;
        [SerializeField] private float _appliedForce = 2f;
        [SerializeField] private ForceMode _forceMode;

        [Space(2), Header("ROTATION SETTINGS")]
        [SerializeField] private float _randomMinXRotation = -90;
        [SerializeField] private float _randomMaxXRotation = 90;
        [SerializeField] private float _rotationForceOnLifetime = 90;
        int _randomValue;

        private Rigidbody _rigidbody;

        private void Awake() => Init();

        private void OnEnable()
        {
            ApplyForceMode();
            ApplyRandomRotationOnSpawn();
        }

        private void LateUpdate() => ApplyRotationDuringLifetime();

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
            var cameraForward = Helper.GetMainCamera().transform.forward;

            _rigidbody.AddRelativeForce(transform.right * _appliedForce/* + cameraRight * _appliedForce*/, _forceMode);
        }

        private void ApplyRandomRotationOnSpawn()
        {
            _randomValue = Random.Range(1, 3);
            float randomXRotation = Random.Range(_randomMinXRotation, _randomMaxXRotation);

            transform.eulerAngles = new Vector3(
                randomXRotation, 
                transform.rotation.y, 
                transform.rotation.z);
        }

        private void ApplyRotationDuringLifetime()
        {
            float forceValue = _rotationForceOnLifetime * Time.deltaTime;

            switch (_randomValue)
            {
                case 1:
                    transform.Rotate(forceValue, 0, 0, Space.Self);
                    break;
                case 2:
                    transform.Rotate(0, forceValue, 0, Space.Self);
                    break;
                case 3:
                    transform.Rotate(0, 0, forceValue, Space.Self);
                    break;
            }
        }
    }
}