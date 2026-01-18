using Player.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.VR
{
    public class VRInputAdapter : MonoBehaviour, IMovementInput
    {
        [Header("XR Inputs")]
        //Referencias directas a las acciones de OpenXR (Left Hand Move, Right Hand Jump)
        [SerializeField] private InputActionProperty moveAction;
        [SerializeField] private InputActionProperty jumpAction;

        public Vector2 MoveInput => moveAction.action != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        
        //Lógica simple para detectar el frame del salto
        public bool JumpTriggered => jumpAction.action != null && jumpAction.action.WasPressedThisFrame();
    }
}