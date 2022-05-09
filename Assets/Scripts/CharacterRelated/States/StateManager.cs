using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Khynan_Coding
{
    public enum StateName
    {
        Idle, Moving, Interaction, Attack, Death
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class StateManager : MonoBehaviour
    {
        Dictionary<StateName, CharacterState> _states = new();

        public StateManager()
        {
            _states[StateName.Idle] = new Character_IdleState();
            _states[StateName.Moving] = new Character_MovingState();
            _states[StateName.Interaction] = new Character_InteractionState();
            _states[StateName.Attack] = new Character_AttackState();
            _states[StateName.Death] = new Character_DeathState();
        }

        private bool _characterIsMoving = false;
        private bool _characterIsRunning = false;

        private CharacterState _currentState;
        public CharacterState CurrentState { get => _currentState; set => _currentState = value; }

        #region Public references
        public IAInteractionHandler InteractionHandler => GetComponent<IAInteractionHandler>();
        public NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
        public DefaultController CharacterController => GetComponent<DefaultController>();
        public bool CharacterIsMoving { get => _characterIsMoving; set => _characterIsMoving = value; }
        public bool CharacterIsRunning { get => _characterIsRunning; set => _characterIsRunning = value; }
        #endregion

        protected virtual void Update()
        {
            CurrentState.ProcessState(this);
        }

        protected void SetDefaultStateAtStart(CharacterState baseState)
        {
            CurrentState = baseState;
            CurrentState.EnterState(this);
        }

        public void SwitchState(CharacterState newState)
        {
            if (CurrentState != newState)
            {
                CurrentState.ExitState(this);

                CurrentState = newState;
                newState.EnterState(this);
            }
        }

        #region States - Get
        public CharacterState Idle()
        {
            return _states[StateName.Idle];
        }

        public CharacterState Moving()
        {
            return _states[StateName.Moving];
        }

        public CharacterState Interaction()
        {
            return _states[StateName.Interaction];
        }

        public CharacterState Attack()
        {
            return _states[StateName.Attack];
        }

        public CharacterState Death()
        {
            return _states[StateName.Death];
        }
        #endregion
    }
}