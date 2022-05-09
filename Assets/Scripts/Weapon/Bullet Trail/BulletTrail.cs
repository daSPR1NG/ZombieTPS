using UnityEngine;

namespace Khynan_Coding
{
    [RequireComponent(typeof(TrailRenderer))]
    [DisallowMultipleComponent]
    public class BulletTrail : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private TrailConfig trailConfig;

        private Vector3 _startPosition;
        private Vector3 _destination;
        private float _timer;

        private TrailRenderer _trailRenderer;

        void Start() => Init();

        void FixedUpdate() => MoveToShotLocation();

        void Init()
        {
            _trailRenderer = GetComponent<TrailRenderer>();
            trailConfig.Apply(_trailRenderer);

            _timer = 0;
        }

        public void Setup(Vector3 startPosition, Vector3 destination)
        {
            _startPosition = startPosition;
            _destination = destination;
        }

        private void MoveToShotLocation()
        {
            if (_timer >= 1)
            {
                _timer = 1;
                transform.position = _destination;
                Destroy(gameObject);
            }

            _timer += Time.fixedDeltaTime / _trailRenderer.time;
            transform.position = Vector3.Lerp(_startPosition, _destination, _timer);
        }
    }
}