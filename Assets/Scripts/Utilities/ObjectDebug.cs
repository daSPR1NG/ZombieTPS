using UnityEngine;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class ObjectDebug : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField] private bool _isDebuggingAPosition = false;
        [SerializeField] private bool _shouldBeRounded = false;

        [Header("LOOK SETTINGS")]
        [SerializeField] private float _positionDebugRadiusValue = 1.0f;
        [SerializeField] private Color _debugDrawColor = Color.red;
        [SerializeField] private Vector3 _positionOffset = Vector3.zero;

        [Header("COLLIDER SETTINGS")]
        [SerializeField] private bool _isDebuggingAColliderRadius = false;
        [SerializeField] private bool _isASphereCollider = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = _debugDrawColor;

            DebugPosition();
            DebugColliderRadius();
        }

        private void DebugPosition()
        {
            if (!_isDebuggingAPosition) { return; }

            if (_shouldBeRounded)
            {
                Gizmos.DrawWireSphere(
                    new Vector3(transform.position.x + _positionOffset.x, transform.position.y + _positionOffset.y, transform.position.z + _positionOffset.z),
                    _positionDebugRadiusValue);
                return;
            }

            Gizmos.DrawWireCube(
                    new Vector3(transform.position.x + _positionOffset.x, transform.position.y + _positionOffset.y, transform.position.z + _positionOffset.z),
                    new Vector3(_positionDebugRadiusValue, _positionDebugRadiusValue, _positionDebugRadiusValue));
        }

        private void DebugColliderRadius()
        {
            if (!_isDebuggingAColliderRadius) { return; }

            // Sphere Collider
            if (TryGetComponent(out SphereCollider sphereCollider))
            {
                Gizmos.DrawWireSphere(new Vector3(
                            transform.position.x + _positionOffset.x,
                            transform.position.y + _positionOffset.y,
                            transform.position.z + _positionOffset.z),
                        sphereCollider.radius);

                //Debug.Log("Its a sphere collider.");
            }
            // Box Collider
            else if (TryGetComponent(out BoxCollider boxCollider))
            {
                Gizmos.DrawWireCube(new Vector3(
                        transform.position.x + _positionOffset.x,
                        transform.position.y + _positionOffset.y,
                        transform.position.z + _positionOffset.z),
                    new Vector3(boxCollider.bounds.size.x, boxCollider.bounds.size.y, boxCollider.bounds.size.z));

                //Debug.Log("Its a box collider.");
            }
            // Capsule Collider
            else if (TryGetComponent(out CapsuleCollider capsuleCollider))
            {
                Gizmos.DrawWireCube(new Vector3(
                        transform.position.x + _positionOffset.x,
                        transform.position.y + _positionOffset.y,
                        transform.position.z + _positionOffset.z),
                    new Vector3(capsuleCollider.radius, capsuleCollider.height, capsuleCollider.radius));

                //Debug.Log("Its a capsule collider.");
            }
        }
    }
}