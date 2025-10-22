using System;
using Configuration;
using Core;
using UnityEngine;

namespace Components
{
    public class EnemyController : MonoBehaviour
    {
        [Header("Configuration")] 
        [SerializeField] private EnemyConfiguration config;
        
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Animator animator;
        
        [Header("Components")]
        [SerializeField] EnemyVisibilityDetector visibilityDetector;
        [SerializeField] private EnemyMovement movement;

        private EnemyStateMachine _stateMachine;

        private void Awake()
        {
            ValidateReferences();
            InitializeStateMachine();
        }

        private void Start()
        {
            visibilityDetector.StartDetection();
            
            //Evento para cambio de visibilidad
            visibilityDetector.OnVisibilityChanged += OnVisibilityChanged;
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void ValidateReferences()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
                else
                    Debug.LogError("[EnemyController] No encontro al player");
            }

            if (animator == null)
                animator = GetComponent<Animator>();
            
            if (visibilityDetector == null)
                visibilityDetector = GetComponent<EnemyVisibilityDetector>();
            
            if (movement == null)
                movement = GetComponent<EnemyMovement>();
        }
        
        private void InitializeStateMachine()
        {
            _stateMachine = new EnemyStateMachine(
                transform,
                playerTransform,
                movement,
                animator,
                config);
        }

        private void OnVisibilityChanged(bool isVisible)
        {
            _stateMachine?.OnVisibilityChanged(isVisible);
        }

        private void OnDestroy()
        {
            if (visibilityDetector != null)
            {
                visibilityDetector.OnVisibilityChanged -= OnVisibilityChanged;
            }
        }
    }
}