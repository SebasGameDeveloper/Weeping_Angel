using System;
using Configuration;
using Interfaces;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour, IMovement
    {
        [SerializeField] private EnemyConfiguration config;
        
        private UnityEngine.AI.NavMeshAgent _navMeshAgent;
        private Animator _animator;
        //Necesito control de animaciones para mejorar el movimiento del enemigo O.o Test1 
        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        public bool HasReachedDestination =>
        !_navMeshAgent.pathPending &&
        _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance &&
        !_navMeshAgent.hasPath;

        private void Awake()
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            _animator = GetComponent<Animator>();
            ConfigureNavMeshAgent();
        }

        private void Update()
        {
            UpdateAnimationSpeed();
        }

        private void UpdateAnimationSpeed()
        {
            if (_animator == null || _navMeshAgent == null) return;
            
            float normalizedSpeed = _navMeshAgent.velocity.magnitude / _navMeshAgent.speed;
            _animator.SetFloat(SpeedHash, normalizedSpeed);
            
            if (_navMeshAgent.velocity.magnitude < 0.1f)
            {
                _animator.SetFloat(SpeedHash, 0f);
            }
        }

        private void ConfigureNavMeshAgent()
        {
            if (_navMeshAgent != null && config != null)
            {
                _navMeshAgent.speed = config.moveSpeed;
                _navMeshAgent.stoppingDistance = config.stoppingDistance;
                _navMeshAgent.autoBraking = true; //fuerzo la config 
                _navMeshAgent.isStopped = true;

                _navMeshAgent.acceleration = 999f;
                _navMeshAgent.angularSpeed = 999f;
            }
        }
        
        public void StartMoving()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.velocity = Vector3.zero;
            }
        }

        public void StopMoving()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true;
                //Velocidad del animator a 0
                _navMeshAgent.velocity = Vector3.zero;
                _navMeshAgent.ResetPath();
                
                if (_animator != null)
                { 
                    _animator.SetFloat(SpeedHash, 0f);
                }
            }
        }

        public void UpdateDestination(Vector3 targetPosition)
        {
            if (_navMeshAgent != null && _navMeshAgent.isActiveAndEnabled && !_navMeshAgent.isStopped)
            {
                _navMeshAgent.SetDestination(targetPosition);
            }
        }
    }
}