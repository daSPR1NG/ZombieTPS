using UnityEngine;
using UnityEngine.AI;

namespace Khynan_Coding
{
    public enum RelatedControllerAction
    {
        Unassigned, OnDeath, OnJump, OnRoll, OnAttack, OnInteraction, OnMoving, OnWalking, OnRunning, OnIdle //....
    }

    [DisallowMultipleComponent]
    public class DefaultController : StateManager
    {
        private AudioSource _audioSource;
        public AudioSource AudioSource { get => _audioSource; private set => _audioSource = value; }

        #region Public references
        [HideInInspector] public bool IsCharacterMoving = false;
        public StatsManager CharacterStats => GetComponent<StatsManager>();
        public Animator Animator => transform.GetChild(0).GetComponent<Animator>();
        public Transform RendererTransform => transform.GetChild(0).transform;
        #endregion

        protected virtual void Awake() => Init();

        protected override void Update()
        {
            base.Update();
        }

        private void Init()
        {
            AudioSource = GetComponent<AudioSource>();

            NavMeshAgent.stoppingDistance = NavMeshAgent.radius + (NavMeshAgent.radius * .25f);
        }

        public void SetCurrentMSValue(float value)
        {
            if (!CharacterStats.DoesThisStatTypeExists(StatType.MovementSpeed)) { return; }

            CharacterStats.GetStatByType(StatType.MovementSpeed).CurrentValue = value;

            if (!NavMeshAgent) { return; }

            NavMeshAgent.speed = value;
        }

        public void UpdateCharacterNavMeshAgentRotation(NavMeshAgent navMeshAgent, Transform transform, float rotationSpeed)
        {
            if (navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > Mathf.Epsilon)
            {
                Quaternion lookRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    lookRotation,
                    rotationSpeed * Time.deltaTime);
            }
        }

        public void LookAtSomething(Transform target)
        {
            transform.LookAt(target, Vector3.up);
        }
    }
}