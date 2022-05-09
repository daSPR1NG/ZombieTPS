using UnityEngine;

namespace Khynan_Coding
{
    public class FreezeRotation : MonoBehaviour
    {
        [Header("X Rotation")]
        [SerializeField] private bool freezeX = false;
        [SerializeField] private float minX = -.5f;
        [SerializeField] private float maxX = .5f;

        [Header("Y Rotation")]
        [SerializeField] private bool freezeY = false;
        [SerializeField] private float minY = -.5f;
        [SerializeField] private float maxY = .5f;

        [Header("Z Rotation")]
        [SerializeField] private bool freezeZ = false;
        [SerializeField] private float minZ = -.5f;
        [SerializeField] private float maxZ = .5f;

        void LateUpdate() => FreezeRotationAxis();

        private void FreezeRotationAxis()
        {
            if (freezeX)
            {
                transform.localEulerAngles = new Vector3(
                    Mathf.Clamp(transform.localEulerAngles.x, minX, maxX), 
                    transform.localEulerAngles.y, 
                    transform.localEulerAngles.z);
            }

            if (freezeY)
            {
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    Mathf.Clamp(transform.localEulerAngles.y, minY, maxY),
                    transform.localEulerAngles.z);
            }

            if (freezeZ)
            {
                transform.localEulerAngles = new Vector3(
                    transform.localEulerAngles.x,
                    transform.localEulerAngles.y,
                    Mathf.Clamp(transform.localEulerAngles.z, minZ, maxZ));
            }
        }
    }
}