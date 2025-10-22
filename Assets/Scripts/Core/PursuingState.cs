using Interfaces;
using UnityEngine;

namespace Core
{
    public class PursuingState : IEnemyState
    {
        private readonly IMovement _movement;
        private readonly Animator _animator;
        private readonly Transform _playerTransform;
        
        public PursuingState(IMovement movement, Animator animator, Transform playerTransform)
        {
            _movement = movement;
            _animator = animator;
            _playerTransform = playerTransform;
        }
        
        public void OnEnter()
        {
            _movement.StartMoving();
            if (_animator != null)
            {
                _animator.speed = 1f;
                //Pendiente de verificacion porque no estoy seguro de requerir otro parametro :S
            }
        }
        public void OnUpdate()
        {
            _movement.UpdateDestination(_playerTransform.position);
        }
        
        public void OnExit()
        {
            _movement.StopMoving();
        }
    }
}