namespace Khynan_Coding
{
    public abstract class CharacterState
    {
        public abstract void Init(StateManager stateManager);
        public abstract void EnterState(StateManager stateManager);
        public abstract void ExitState(StateManager stateManager);

        public virtual void ProcessState(StateManager stateManager)
        {
            Helper.DebugMessage("ProcessState " + stateManager.CurrentState, stateManager.transform);
        }
    }
}