using UnityEngine;

namespace Khynan_Coding
{
    public abstract class PowerUp : ScriptableObject
    {
        [Space(3), Header("DEPENDENCIES")]
        [SerializeField] protected string _name;
        [SerializeField] private Sprite _icon;
        [SerializeField] private float _duration;
        [SerializeField] private float _grabbableDuration;
        [SerializeField] private Vector3 _offsetSpawn;

        [Space(3), Header("GRAB SETTINGS")]
        [SerializeField] private Vector3 _grabFXOffsetSpawn;
        [SerializeField] private GameObject _onGrabFX;
        [SerializeField] private AudioClip _onGrabSFX;

        public abstract void Apply(Transform target);
        public virtual void RemovePowerUpEffect(Transform target) { }
        protected virtual void GrabEffect(Transform target)
        {
            if (!_onGrabFX)
            {
                Debug.LogError("No grab effect assigned !");
                return;
            }

            Instantiate(_onGrabFX, target.position + _grabFXOffsetSpawn, _onGrabFX.transform.rotation);
        }

        public float GetDuration() { return _duration; }
        public float GetGrabbableDuration() { return _grabbableDuration; }
        public AudioClip GetAudioClip() { return _onGrabSFX; }
        public Vector3 GetOffsetWSpawn() { return _offsetSpawn; }
    }
}