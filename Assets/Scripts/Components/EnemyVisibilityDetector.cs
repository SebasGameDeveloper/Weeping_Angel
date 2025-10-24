using System;
using System.Collections;
using System.Collections.Generic;
using Configuration;
using Interfaces;
using UnityEngine;

namespace Components
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class EnemyVisibilityDetector : MonoBehaviour, IVisibilityDetector
    {
        //Usare un metodo de frustum culling  y raycast para detectar si el enemigo es visible para la camara principal
        [SerializeField] private EnemyConfiguration config;

        public event System.Action<bool> OnVisibilityChanged;

        private Camera _mainCamera;
        private Transform _cameraTransform;
        private SkinnedMeshRenderer _renderer;
        private Coroutine _detectionCoroutine;
        private bool _isVisible;
        private bool _wasVisible;

        public bool IsVisible => _isVisible;

        private void Awake()
        {
            _renderer = GetComponent<SkinnedMeshRenderer>();
            _mainCamera = Camera.main;

            if (_mainCamera != null)
            {
                _cameraTransform = _mainCamera.transform;
            }
            else
            {
                Debug.LogError("[EnemyVisibilityDetector] No CAMERA ;(");
            }
        }

        public void StartDetection()
        {
            if (_detectionCoroutine == null)
            {
                _detectionCoroutine = StartCoroutine(DetectionRoutine());
            }
        }
        
        public void StopDetection()
        {
            if (_detectionCoroutine != null)
            {
                StopCoroutine(_detectionCoroutine);
                _detectionCoroutine = null;
            }
        }
        
        //Verificar visibilidad de intervalos
        private IEnumerator DetectionRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(config.visibilityCheckInterval);

            while (true)
            {
                _isVisible = CheckVisibility();

                if (_isVisible != _wasVisible)
                {
                    OnVisibilityChanged?.Invoke(_isVisible);
                    _wasVisible = _isVisible;
                }

                yield return wait;
            }
        }

        private bool CheckVisibility() //MEtodo de verificacion O.o
        {
            if (_mainCamera == null || _renderer == null)
                return false;
            
            if(!IsInFrustum())
                return false;
            
            return IsVisibleRaycast();
        }

        private bool IsInFrustum()
        {
            Vector3 viewportPoint = _mainCamera.WorldToViewportPoint(_renderer.bounds.center);
            
            bool inFrustum = viewportPoint.z > 0 &&
                             viewportPoint.x >= 0 -config.frustumMargin &&
                             viewportPoint.x <= 1f + config.frustumMargin &&
                             viewportPoint.y >= -config.frustumMargin &&
                             viewportPoint.y <= 1f + config.frustumMargin;

            return inFrustum;
        }

        private bool IsVisibleRaycast()
        {
            Vector3 directionToEnemy = _renderer.bounds.center - _cameraTransform.position;
            float distanceToEnemy = directionToEnemy.magnitude;
            
            Ray ray = new Ray(_cameraTransform.position, directionToEnemy.normalized);

            if (Physics.Raycast(ray, out RaycastHit hit, distanceToEnemy, config.detectionLayerMask))
            {
                return hit.transform == transform || hit.transform.IsChildOf(transform);
            }
            return false;
        }

        private void OnDisable()
        {
            StopDetection();
        }
    }
}