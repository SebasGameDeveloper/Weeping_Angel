using Interfaces;
using UnityEngine;

namespace Core
{
    public class FrozenState : IEnemyState
    {
        private readonly IMovement _movement;
        private readonly Animator _animator;
        
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        
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
                _animator.SetFloat(SpeedHash, 0f);
                //_animator.Play("Walk", 0, 0);
                _animator.SetFloat(SpeedHash, 0f);
            }
        }
        public void OnUpdate()
        {
            //Enemigo permanece congelado! <------ OJO :)
            if (_animator != null && _animator.speed != 0f)
            {
                _animator.speed = 0f;
                _animator.SetFloat(SpeedHash, 0f);
            }
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