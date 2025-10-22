using Configuration;
using Interfaces;
using UnityEngine;

namespace Core
{
    public class EnemyStateMachine
    {
        private IEnemyState _currentState;
        private readonly FrozenState _frozenState;
        private readonly PursuingState _pursuingState;

        private readonly Transform _enemyTransform;
        private readonly Transform _playerTransform;
        private readonly float _activationRange;

        private bool _isVisible;

        public EnemyStateMachine(
            Transform enemyTransform,
            Transform playerTransform,
            IMovement movement,
            Animator animator,
            EnemyConfiguration config) //Scriptable object
        {
            _enemyTransform = enemyTransform;
            _playerTransform = playerTransform;
            _activationRange = config.activationRange;
            
            //Iniciamos ESTADOS :)
            _frozenState = new FrozenState(movement, animator);
            _pursuingState = new PursuingState(movement, animator, playerTransform);
            
            ChangeState(_frozenState);
        }
        
        //Actualiza estado de visibilidad y evalua transiciones 
        public void OnVisibilityChanged(bool isVisible)
        {
            _isVisible = isVisible;
            EvaluateStateTransition();
        }

        public void Update()
        {
            _currentState?.OnUpdate();  
            EvaluateStateTransition();
        }
        
        //Evalua si cambiar de estado en base a las condiciones actuales 
        private void EvaluateStateTransition()
        {
            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);
            bool inRange = distanceToPlayer <= _activationRange;

            if (_isVisible && _currentState != _frozenState)
            {
                ChangeState(_frozenState);
            }
            else if (!_isVisible && inRange && _currentState != _pursuingState)
            {
                ChangeState(_pursuingState);
            }
            else if (!inRange && _currentState != _frozenState)
            {
                ChangeState(_frozenState);
            }
        }

        private void ChangeState(IEnemyState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }
    }
}