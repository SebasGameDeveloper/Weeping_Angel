using Player.Core;
using UnityEngine;

namespace Player.Physics
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerLocomotion : MonoBehaviour
    {
        [Header("Configuración Física")]
        [SerializeField] private float moveSpeed = 4.5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float jumpHeight = 1.5f; //Mejor usar altura que fuerza bruta

        private CharacterController _controller;
        private Vector3 _velocity;
        
        //Inyección de dependencia (puede ser asignada por inspector o buscada)
        private IMovementInput _inputProvider; 

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            //En una arquitectura pura, esto se inyectaría desde un Composition Root o Zenject.
            //Para simplificar aquí, buscamos el componente en el mismo objeto.
            _inputProvider = GetComponent<IMovementInput>();
        }

        private void Update()
        {
            HandleGravity();
            HandleMovement();
        }

        private void HandleMovement()
        {
            if (_inputProvider == null) return;

            Vector2 input = _inputProvider.MoveInput;
            
            //En VR, el "frente" es hacia donde mira la CÁMARA, no el cuerpo.
            //Necesitamos la referencia de la cámara para movernos en la dirección correcta.
            Transform cameraTransform = Camera.main.transform; 
            
            //Proyectamos la dirección de la cámara en el plano XZ (piso) para no volar hacia arriba/abajo
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            Vector3 moveDirection = (forward * input.y + right * input.x).normalized;

            _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        private void HandleGravity()
        {
            if (_controller.isGrounded)
            {
                if (_velocity.y < 0) _velocity.y = -2f; //Pequeña fuerza hacia abajo para asegurar contacto

                if (_inputProvider != null && _inputProvider.JumpTriggered)
                {
                    _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}