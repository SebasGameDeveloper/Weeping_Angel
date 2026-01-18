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
        // Podríamos añadir un estado "Idle" si quisieras que patrulle, pero de momento usaremos Frozen como base.

        private readonly Transform _enemyTransform;
        private readonly Transform _playerTransform;
        private readonly EnemyConfiguration _config; // Guardamos la config entera para acceder a mascaras y angulos

        private bool _isVisible;    // ¿El jugador ve al enemigo?
        private bool _hasActivated; // ¿El enemigo ya se dio cuenta de que el jugador existe?

        public EnemyStateMachine(
            Transform enemyTransform,
            Transform playerTransform,
            IMovement movement,
            Animator animator,
            EnemyConfiguration config)
        {
            _enemyTransform = enemyTransform;
            _playerTransform = playerTransform;
            _config = config; // Guardar referencia
            
            _frozenState = new FrozenState(movement, animator);
            _pursuingState = new PursuingState(movement, animator, playerTransform);
            
            // Estado inicial: Quieto
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
            
            // Si aun no se ha activado, buscamos al jugador activamente
            if (!_hasActivated)
            {
                if (CheckInitialActivation())
                {
                    _hasActivated = true;
                    // Debug.Log("¡Enemigo Activado! Te ha visto.");
                }
            }
            
            EvaluateStateTransition();
        }

        // Nueva lógica de detección "Realista"
        private bool CheckInitialActivation()
        {
            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);

            // 1. Chequeo de Distancia
            if (distanceToPlayer > _config.activationRange) return false;

            // 2. Chequeo de Ángulo (¿Está frente a mi?)
            Vector3 directionToPlayer = (_playerTransform.position - _enemyTransform.position).normalized;
            float angle = Vector3.Angle(_enemyTransform.forward, directionToPlayer);
            
            // Si el ángulo es mayor a 60 grados (aprox), no lo ve. (Ajusta este valor a tu gusto)
            if (angle > 60f) return false;

            // 3. Raycast (¿Hay una pared en medio?)
            // Lanzamos un rayo desde los ojos (aprox +1.7y) hacia el jugador
            Vector3 origin = _enemyTransform.position + Vector3.up * 1.5f;
            Vector3 target = _playerTransform.position + Vector3.up * 1.5f;
            
            // Usamos la mascara de detección de tu config para chocar con paredes (Default)
            if (Physics.Linecast(origin, target, out RaycastHit hit, _config.detectionLayerMask))
            {
                // Si chocamos con algo que NO es el jugador, hay una pared
                if (hit.transform != _playerTransform) return false;
            }

            return true; // Pasó todas las pruebas: Te ve.
        }
        
        private void EvaluateStateTransition()
        {
            // Si NO se ha activado, se queda congelado/quieto y no hace nada más.
            if (!_hasActivated)
            {
                if (_currentState != _frozenState) ChangeState(_frozenState);
                return;
            }

            // LOGICA WEEPING ANGEL (Solo corre si ya está activado)
            
            float distanceToPlayer = Vector3.Distance(_enemyTransform.position, _playerTransform.position);
            // Opcional: Si el jugador se aleja muchísimo, ¿se desactiva? 
            // De momento lo dejamos activado para siempre una vez te ve.

            if (_isVisible && _currentState != _frozenState)
            {
                // Si el jugador lo mira -> Estatua
                ChangeState(_frozenState);
            }
            else if (!_isVisible && _currentState != _pursuingState)
            {
                // Si el jugador NO lo mira -> Atacar
                ChangeState(_pursuingState);
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