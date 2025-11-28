using System; // <--- NECESARIO
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
        private readonly EnemyConfiguration _config;

        private bool _isVisible; 
        private bool _hasActivated;
        
        private readonly Action _onActivateCallback; 

        public EnemyStateMachine(
            Transform enemyTransform,
            Transform playerTransform,
            IMovement movement,
            Animator animator,
            EnemyConfiguration config,
            Action onActivateCallback)
        {
            _enemyTransform = enemyTransform;
            _playerTransform = playerTransform;
            _config = config; 
            _onActivateCallback = onActivateCallback;
            
            _frozenState = new FrozenState(movement, animator);
            _pursuingState = new PursuingState(movement, animator, playerTransform);
            
            ChangeState(_frozenState);
        }
        
        public void OnVisibilityChanged(bool isVisible)
        {
            _isVisible = isVisible;
            EvaluateStateTransition();
        }

        public void Update()
        {
            _currentState?.OnUpdate();
            
            if (!_hasActivated)
            {
                if (CheckInitialActivation())
                {
                    _hasActivated = true;
                    _onActivateCallback?.Invoke();
                    Debug.Log("[StateMachine] Enemigo activado por primera vez.");
                }
            }
            
            EvaluateStateTransition();
        }
        
        private bool CheckInitialActivation()
        {
            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);
            if (distanceToPlayer > _config.activationRange) return false;
            Vector3 directionToPlayer = (_playerTransform.position - _enemyTransform.position).normalized;
            float angle = Vector3.Angle(_enemyTransform.forward, directionToPlayer);
            if (angle > 60f) return false;
            Vector3 origin = _enemyTransform.position + Vector3.up * 1.5f;
            Vector3 target = _playerTransform.position + Vector3.up * 1.5f;
            if (Physics.Linecast(origin, target, out RaycastHit hit, _config.detectionLayerMask))
            {
                if (hit.transform != _playerTransform) return false;
            }
            return true; 
        }

        private void EvaluateStateTransition()
        {
            if (!_hasActivated) { if (_currentState != _frozenState) ChangeState(_frozenState); return; }
            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);
            if (_isVisible && _currentState != _frozenState) ChangeState(_frozenState);
            else if (!_isVisible && _currentState != _pursuingState) ChangeState(_pursuingState);
        }

        private void ChangeState(IEnemyState newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }
    }
}