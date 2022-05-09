using UnityEngine;
using UnityEngine.AI;

namespace Khynan_Coding
{
    [DisallowMultipleComponent]
    public class IAInteractionHandler : MonoBehaviour
    {
        // Est-ce que je suis arrivé à portée de la cible ?
        // Si oui > interaction

        public Transform CurrentTarget;
        [SerializeField] private float _interactionRange = 1.25f;
        [SerializeField] private float _interactionMoveTransitionDelay = 0.5f;

        private bool _isTargetTooFar;
        private bool _canMoveTowardsTarget = true;
        private float _currentTransitionTimer = 0;

        private NavMeshAgent _navMeshAgent;
        private IAController _iaController;

        private void Awake() => Initialization();

        private void LateUpdate()
        {
            MoveTowardsTarget();

            if (HasReachedTarget())
            {
                InteractWithTarget();
                Debug.Log("No path left");
            }

            CheckIfTargetIsStillInRange();
        }

        private void Initialization()
        {
            _iaController = GetComponent<IAController>();
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void SetTarget(Transform newTarget)
        {
            CurrentTarget = newTarget;

            Helper.SetAgentStoppingDistance(_navMeshAgent, _interactionRange, 1);
        }

        public void SetInteractorDestination(Vector3 targetPos)
        {
            Helper.SetAgentDestination(_navMeshAgent, targetPos);
            _iaController.SwitchState(_iaController.Moving());
        }

        private void MoveTowardsTarget()
        {
            if (!HasATarget() || !_canMoveTowardsTarget) { return; }

            // Reset path if it reaches its target...
            if (HasReachedTarget()) 
            {
                Helper.ResetAgentDestination(_navMeshAgent);
                return; 
            }

            // ... else set the target destination to move towards it
            SetInteractorDestination(CurrentTarget.position);
        }

        public bool HasATarget()
        {
            return CurrentTarget != null;
        }

        public bool IsTargetDead(StatsManager statsManager)
        {
            return statsManager.IsCharacterDead();
        }

        private void InteractWithTarget()
        {
            GlobalCharacterParameters globalCharacterParameters = CurrentTarget.GetComponent<GlobalCharacterParameters>();

            switch (globalCharacterParameters.CharacterType)
            {
                case CharacterType.Player:
                    _currentTransitionTimer = _interactionMoveTransitionDelay;
                    _isTargetTooFar = false;

                    _iaController.SwitchState(_iaController.Attack());
                    break;
            }
        }

        private bool HasReachedTarget()
        {
            return HasATarget() && Vector3.Distance(transform.position, CurrentTarget.position) <= _navMeshAgent.stoppingDistance;
        }

        private void CheckIfTargetIsStillInRange()
        {
            if (!HasATarget()) { return; }

            float distanceFromTarget = Vector3.Distance(transform.position, CurrentTarget.position);

            if (distanceFromTarget > _navMeshAgent.stoppingDistance)
            {
                _isTargetTooFar = true;
                OnTargetBeingTooFar();
            }
        }

        private void OnTargetBeingTooFar()
        {
            if (!_isTargetTooFar) { return; }

            _canMoveTowardsTarget = false;

            _currentTransitionTimer -= Time.deltaTime;

            if (_iaController.Animator.GetLayerWeight(1) != 0)
            {
                _iaController.Animator.SetLayerWeight(1, Mathf.Lerp(_iaController.Animator.GetLayerWeight(1), 0, Time.deltaTime));
            }

            if (_currentTransitionTimer <= 0)
            {
                _currentTransitionTimer = 0;

                _canMoveTowardsTarget = true;
                _iaController.SwitchState(_iaController.Moving());
            }
        }
    }
}