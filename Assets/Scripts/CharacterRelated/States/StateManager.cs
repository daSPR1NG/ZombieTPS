using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Khynan_Coding
{
    public enum StateType
    {
        Idle, Moving, Interaction, Attack, Death
    }

    [RequireComponent(typeof(NavMeshAgent))]
    public class StateManager : MonoBehaviour
    {
        readonly Dictionary<StateType, CharacterState> _states = new();

        public StateManager()
        {
            _states[StateType.Idle] = new Character_IdleState();
            _states[StateType.Moving] = new Character_MovingState();
            _states[StateType.Interaction] = new Character_InteractionState();
            _states[StateType.Attack] = new Character_AttackState();
            _states[StateType.Death] = new Character_DeathState();
        }

        private bool _characterIsMoving = false;
        private bool _characterIsRunning = false;

        private CharacterState _currentState;
        public CharacterState CurrentState { get => _currentState; set => _currentState = value; }

        #region Public references
        public IAInteractionHandler InteractionHandler => GetComponent<IAInteractionHandler>();
        public NavMeshAgent NavMeshAgent => GetComponent<NavMeshAgent>();
        public DefaultController DefaultController => GetComponent<DefaultController>();
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
            return _states[StateType.Idle];
        }

        public CharacterState Moving()
        {
            return _states[StateType.Moving];
        }

        public CharacterState Interaction()
        {
            return _states[StateType.Interaction];
        }

        public CharacterState Attack()
        {
            return _states[StateType.Attack];
        }

        public CharacterState Death()
        {
            return _states[StateType.Death];
        }
        #endregion
    }
}