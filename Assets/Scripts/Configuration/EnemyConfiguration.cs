using UnityEngine;

namespace Configuration
{
    [CreateAssetMenu(fileName = "EnemyConfig", menuName = "WeepingAngel/Enemy Configuration")]
    public class EnemyConfiguration : ScriptableObject
    {
        [Header("Movement Settings")]
        [Tooltip("Velocidad de movimiento del enemigo")]
        public float moveSpeed = 3.5f;
        
        [Tooltip("Rango dentro del cual el enemigo puede activarse")]
        public float activationRange = 15f;

        [Tooltip("Distancia minima de persecucion antes de detenerse")]
        public float stoppingDistance = 1.5f;

        [Header("Detection Settings")]
        [Tooltip("Frecuencia de comprobacion de visibilidad (segundos)")]
        public float visibilityCheckInterval = 0.1f;

        [Tooltip("Margen del frustum para considerar al enemigo visible")]
        public float frustumMargin = 0.1f;

        [Tooltip("Layer mask para raycast de deteccion")]
        public LayerMask detectionLayerMask = -1;
        
        [Header("Animation Settings")]
        [Tooltip("Nombre del parametro de velocidad en el Animator")]
        public string walkSpeedParameter = "Speed";
    }
}