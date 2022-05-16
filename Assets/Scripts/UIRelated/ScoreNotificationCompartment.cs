using TMPro;
using UnityEngine;

namespace Khynan_Coding
{
    public class ScoreNotificationCompartment : MonoBehaviour
    {
        [Header("DEPENDENCIES")]
        [SerializeField] private TMP_Text _notificationContentText;
        [SerializeField] private float _displayDuration;
        private float _currentDisplayTimer;

        private Animator _animator;

        void Start() => Init();

        void Update() => ProcessDisplayTimer();

        void Init()
        {
            _animator = GetComponent<Animator>();

            _currentDisplayTimer = _displayDuration;
        }

        public void SetContentText(string text)
        {
            _notificationContentText.SetText(text);
        }

        private void ProcessDisplayTimer()
        {
            _currentDisplayTimer -= Time.deltaTime;

            if (_currentDisplayTimer <= 0)
            {
                _currentDisplayTimer = 0;

                _animator.Play("ScoreNotification_FadeOut");
            }
        }

        // Called directly in an animation ScoreNotification_FadeOut
        public void DestroyCompartment()
        {
            Destroy(gameObject);
        }
    }
}