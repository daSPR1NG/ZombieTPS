namespace Khynan_Coding
{
    public class Character_InteractionState : CharacterState
    {
        PlayerInteractionHandler _playerInteractionHandler;

        //Store each variable that might be needed for this state, here.
        public override void Init(StateManager stateManager)
        {
            _playerInteractionHandler = stateManager.GetComponent<PlayerInteractionHandler>();
        }

        public override void EnterState(StateManager stateManager)
        {
            Init(stateManager);

            _playerInteractionHandler.IsInteracting = true;

            Helper.DebugMessage("Entering <INTERACTION> state", stateManager.transform);
        }

        public override void ExitState(StateManager stateManager)
        {
            _playerInteractionHandler.IsInteracting = false;

            Helper.DebugMessage("Exiting <INTERACTION> state", stateManager.transform);
        }

        public override void ProcessState(StateManager stateManager)
        {
            base.ProcessState(stateManager);
        }
    }
}