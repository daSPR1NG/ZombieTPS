namespace Khynan_Coding
{
    public class Character_MovingState : CharacterState
    {
        private DefaultController _controller;
        private IAInteractionHandler _interactionHandler;

        //Store each variable that might be needed for this state, here.
        public override void Init(StateManager stateManager)
        {
            _controller = stateManager.CharacterController;
            _interactionHandler = stateManager.GetComponent<IAInteractionHandler>();
        }

        public override void EnterState(StateManager stateManager)
        {
            Init(stateManager);

            stateManager.CharacterIsMoving = true;

            Helper.DebugMessage("Entering <MOVING> state", stateManager.transform); 
        }

        public override void ExitState(StateManager stateManager)
        {
            stateManager.CharacterIsMoving = false;

            Helper.DebugMessage("Exiting <MOVING> state", stateManager.transform);
        }

        public override void ProcessState(StateManager stateManager)
        {
            base.ProcessState(stateManager);

            UpdateNavMeshAgentRotationWhileMoving(stateManager);
        }

        private void UpdateNavMeshAgentRotationWhileMoving(StateManager stateManager)
        {
            if (_controller.NavMeshAgent.hasPath)
            {
                _controller.UpdateCharacterNavMeshAgentRotation(stateManager.NavMeshAgent, stateManager.transform, 15);
            }
        }
    }
}