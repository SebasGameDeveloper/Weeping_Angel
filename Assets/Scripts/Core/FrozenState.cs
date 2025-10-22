using Interfaces;
using UnityEngine;

namespace Core
{
    public class FrozenState : IEnemyState
    {
        private readonly IMovement _movement;
        private readonly Animator _animator;
        
        public FrozenState(IMovement movement, Animator animator)
        {
            _movement = movement;
            _animator = animator;
        }

        public void OnEnter()
        {
            _movement.StopMoving();
            if (_animator != null)
            {
                _animator.speed = 0f;
                _animator.Play("Walk", 0, 0);
            }
        }
        public void OnUpdate()
        {
            //Enemigo permanece congelado! <------ OJO :)
        }
        
        public void OnExit()
        {
            if (_animator != null)
            {
                _animator.speed = 1f;
            }
        }
    }
}