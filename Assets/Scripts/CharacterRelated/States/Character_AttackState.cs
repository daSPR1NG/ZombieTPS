using UnityEngine;

namespace Khynan_Coding
{
    public class Character_AttackState : CharacterState
    {
        DefaultController _controller;
        IAInteractionHandler _interactionHandler;
        CombatSystem _combatSystem;

        //Store each variable that might be needed for this state, here.
        public override void Init(StateManager stateManager)
        {
            _controller = stateManager.DefaultController;
            _interactionHandler = stateManager.GetComponent<IAInteractionHandler>();
            _combatSystem = stateManager.GetComponent<CombatSystem>();
        }

        public override void EnterState(StateManager stateManager)
        {
            Init(stateManager);

            Helper.ResetAgentDestination(_controller.NavMeshAgent);
            _controller.LookAtSomething(_interactionHandler.CurrentTarget);

            Helper.DebugMessage("Entering <ATTACK> state", stateManager.transform);
        }

        public override void ExitState(StateManager stateManager)
        {
            AnimatorHelper.SetAnimatorBoolean(_controller.Animator, "Attack", false);
            _combatSystem.ResetAttackState();

            Helper.DebugMessage("Exiting <ATTACK> state", stateManager.transform);
        }

        public override void ProcessState(StateManager stateManager)
        {
            base.ProcessState(stateManager);

            _combatSystem.SetupAttack(_interactionHandler.CurrentTarget);

            Debug.Log(stateManager.name + " is Attacking.");
        }
    }
}