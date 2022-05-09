using UnityEngine;

namespace Khynan_Coding
{
    public class WorldUI : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private Vector3 _offset;

        Camera _mainCamera;
        Transform _target;

        private void OnDisable()
        {
            _target = null;
        }

        void Start() => Init();

        private void LateUpdate() => FollowTarget();

        void Init()
        {
            _mainCamera = Helper.GetMainCamera();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void FollowTarget()
        {
            Vector3 pos = _mainCamera.WorldToScreenPoint(_target.position + _offset);

            if (transform.position != pos) { transform.position = pos; }
        }
    }
}