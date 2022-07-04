using UnityEngine;

namespace Khynan_Coding
{
    public class Character_DeathState : CharacterState
    {
        float _deathAnimationDuration;

        DefaultController _controller;
        IAInteractionHandler _interactionHandler;
        StatsManager _statsManager;
        CapsuleCollider _capsuleCollider;
        
        //Store each variable that might be needed for this state, here.
        public override void Init(StateManager stateManager)
        {
            _controller = stateManager.DefaultController;
            _interactionHandler = _controller.InteractionHandler;
            _statsManager = _controller.CharacterStats;

            _capsuleCollider = stateManager.GetComponent<CapsuleCollider>();
            _capsuleCollider.enabled = false;

            _deathAnimationDuration = AnimatorHelper.GetAnimationLength(_controller.Animator, 3) + 0.15f;
        }

        public override void EnterState(StateManager stateManager)
        {
            Init(stateManager);

            _interactionHandler.CurrentTarget = null;
            Helper.ResetAgentDestination(_controller.NavMeshAgent);

            AnimatorHelper.HandleThisAnimation(_controller.Animator, "IsDead", true, 1, 1);

            DisableColliders(stateManager.transform);

            //Helper.DebugMessage("Entering <DEATH> state", stateManager.transform);
        }

        public override void ExitState(StateManager stateManager)
        {
            //Helper.DebugMessage("Exiting <DEATH> state", stateManager.transform);
        }

        public override void ProcessState(StateManager stateManager)
        {
            base.ProcessState(stateManager);

            HandleDeath();

            //Debug.Log(stateManager.name + " is Dead.");
        }

        private void HandleDeath()
        {
            _deathAnimationDuration -= Time.deltaTime;

            if (_deathAnimationDuration <= 0)
            {
                _deathAnimationDuration = 0;
                _statsManager.InstantiateDeathFX();
            }
        }

        private void DisableColliders(Transform transform)
        {
            Collider selfCollider = transform.GetComponent<Collider>();
            selfCollider.enabled = false;

            foreach (Collider c in transform.GetComponentsInChildren<Collider>())
            {
                c.enabled = false;
            }
        }
    }
}