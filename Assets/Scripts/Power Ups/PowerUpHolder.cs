using UnityEngine;
using System.Collections.Generic;

namespace Khynan_Coding
{
    public class PowerUpHolder : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private CharacterType _characterType = CharacterType.Unassigned;
        [SerializeField] private PowerUp _powerUp;
        [Space(3), SerializeField] private List<ParticleSystemRenderer> _particleRenderers = new();

        private readonly List<ParticleSystemRenderMode> _particleFetchedRenderers = new();

        private float _internGrabbableDuration;
        private AudioSource _audioSource;
        private Animator _animator;

        private void LateUpdate() => ProcessGrabbableDuration();

        private void Start() => Init();

        private void Init()
        {
            _animator = GetComponent<Animator>();
            _animator.speed = 0;

            _internGrabbableDuration = _powerUp.GetGrabbableDuration();
        }

        private void OnTriggerEnter(Collider other)
        {
            GlobalCharacterParameters globalCharacterParameters = other.GetComponent<GlobalCharacterParameters>();

            if (!globalCharacterParameters || globalCharacterParameters.CharacterType != _characterType)
            {
                return;
            }

            _powerUp.Apply(other.transform);
            AudioHelper.PlaySound( _audioSource, _powerUp.GetAudioClip(), _audioSource.volume );

            Destroy(gameObject);
        }

        private void ProcessGrabbableDuration()
        {
            _internGrabbableDuration -= Time.deltaTime;

            if (_internGrabbableDuration / _powerUp.GetGrabbableDuration() <= .6f 
                        && _internGrabbableDuration / _powerUp.GetGrabbableDuration() > .4f) { _animator.speed = 1.5f; }

            if (_internGrabbableDuration / _powerUp.GetGrabbableDuration() <= .4f
                        && _internGrabbableDuration / _powerUp.GetGrabbableDuration() > .5f) { _animator.speed = 3; }

            if (_internGrabbableDuration / _powerUp.GetGrabbableDuration() <= .25f) { _animator.speed = 5; }

            if (_internGrabbableDuration <= 0)
            {
                _internGrabbableDuration = 0;

                Destroy(gameObject);
            }
        }

        // Called in an Animation Event
        public void HandleParticleEffectsDisplayState(int toggle)
        {
            if (_particleRenderers.Count == 0) { return; }

            for (int i = 0; i < _particleRenderers.Count; i++)
            {
               if (_particleFetchedRenderers.Count != _particleRenderers.Count) _particleFetchedRenderers.Add(_particleRenderers[i].renderMode);
            }

            for (int i = 0; i < _particleRenderers.Count; i++)
            {
                switch (toggle)
                {
                    case 0:
                        _particleRenderers[i].renderMode = ParticleSystemRenderMode.None;
                        //Debug.Log("None");
                        break;
                    case 1:
                        _particleRenderers[i].renderMode = _particleFetchedRenderers[i];
                        //Debug.Log(_particleFetchedRenderers[i]);
                        break;
                }
            }
        }

        public void SetAudioSource( AudioSource audioSource ) { _audioSource = audioSource; }

        public PowerUp GetPowerUp() { return _powerUp; }
    }
}