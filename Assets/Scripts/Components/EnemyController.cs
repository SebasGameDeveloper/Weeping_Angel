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
        
        // NUEVO: Evento público al que se suscribirán Audio y Susurros
        public event Action OnEnemyActivated; 

        private void Awake()
        {
            ValidateReferences();
            InitializeStateMachine();
        }

        private void Start()
        {
            visibilityDetector.StartDetection();
            visibilityDetector.OnVisibilityChanged += OnVisibilityChanged;
        }

        private void Update()
        {
            _stateMachine?.Update();
        }

        private void ValidateReferences()
        {
            if (playerTransform == null) {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
            if (animator == null) animator = GetComponent<Animator>();
            if (visibilityDetector == null) visibilityDetector = GetComponent<EnemyVisibilityDetector>();
            if (movement == null) movement = GetComponent<EnemyMovement>();
        }
        
        private void InitializeStateMachine()
        {
            // Pasamos "NotifyActivation" como argumento extra al constructor
            _stateMachine = new EnemyStateMachine(
                transform,
                playerTransform,
                movement,
                animator,
                config,
                NotifyActivation // <--- CALLBACK
            );
        }
        
        // Método puente que invoca el evento público
        private void NotifyActivation()
        {
            OnEnemyActivated?.Invoke();
        }

        private void OnVisibilityChanged(bool isVisible)
        {
            _stateMachine?.OnVisibilityChanged(isVisible);
        }

        private void OnDestroy()
        {
            if (visibilityDetector != null) visibilityDetector.OnVisibilityChanged -= OnVisibilityChanged;
        }
    }
}