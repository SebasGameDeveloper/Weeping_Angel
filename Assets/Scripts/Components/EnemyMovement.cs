using System;
using Configuration;
using Interfaces;
using UnityEngine;
using UnityEngine.AI;

namespace Components
{
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class EnemyMovement : MonoBehaviour, IMovement
    {
        [SerializeField] private EnemyConfiguration config;

        private UnityEngine.AI.NavMeshAgent _navMeshAgent;
        private Animator _animator;

        private bool isMoving;

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

            if (isMoving && _navMeshAgent != null && !_navMeshAgent.isStopped)
            {
                RotateTowardsMovement();
            }
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


        private void RotateTowardsMovement()
        {
            if (_navMeshAgent.velocity.sqrMagnitude > 0.1f)
            {
                Vector3 direction = _navMeshAgent.velocity.normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * 8f
                );
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
                _navMeshAgent.angularSpeed = 120f;

                //Bloqueo de rrotacion 
                _navMeshAgent.updateRotation = false;
                _navMeshAgent.updateUpAxis = false;

                //NavMesh nuevas implementaciones PRUEBAS
                _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                //_navMeshAgent.avoidancePriority = 50;
                _navMeshAgent.avoidancePriority = config.avoidancePriority;
                //_navMeshAgent.radius = 0.5f;
                _navMeshAgent.radius = config.avoidanceRadius;

                //AutoPath
                _navMeshAgent.autoTraverseOffMeshLink = true;
                _navMeshAgent.autoRepath = true;

                //Areas navegables
                _navMeshAgent.areaMask = NavMesh.AllAreas;
            }
        }

        public void StartMoving()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.isStopped = false;
                _navMeshAgent.velocity = Vector3.zero;
                isMoving = true;
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

                isMoving = false;

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

        private void OnDrawGizmos()
        {
            if (_navMeshAgent != null && isMoving)
            {
                // Dibujar línea de dirección
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, _navMeshAgent.velocity.normalized * 2f);

                // Dibujar path
                if (_navMeshAgent.hasPath)
                {
                    Gizmos.color = Color.yellow;
                    Vector3[] corners = _navMeshAgent.path.corners;
                    for (int i = 0; i < corners.Length - 1; i++)
                    {
                        Gizmos.DrawLine(corners[i], corners[i + 1]);
                    }
                }
            }
        }
    }
}