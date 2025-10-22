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

        public bool HasReachedDestination =>
        !_navMeshAgent.pathPending &&
        _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;

        private void Awake()
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            ConfigureNavMeshAgent();
        }
        
        private void ConfigureNavMeshAgent()
        {
            if (_navMeshAgent != null && config != null)
            {
                _navMeshAgent.speed = config.moveSpeed;
                _navMeshAgent.stoppingDistance = config.stoppingDistance;
                _navMeshAgent.isStopped = true;
            }
        }
        
        public void StartMoving()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = false;
            }
        }

        public void StopMoving()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = true;
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