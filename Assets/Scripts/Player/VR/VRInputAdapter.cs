using UnityEngine;
using UnityEngine.InputSystem;
using Player.Core;

namespace Player.VR
{
    public class VRInputAdapter : MonoBehaviour, IMovementInput
    {
        [Header("Referencias a Input System")]
        //Usamos InputAcrionReference para obligar a usar el asset preconfigurado
        [SerializeField] private InputActionReference moveActionRef;
        [SerializeField] private InputActionReference jumpActionRef;

        public Vector2 MoveInput 
        {
            get 
            {
                //Verificación de nulos estilo AAA para evitar crasheos
                if (moveActionRef != null && moveActionRef.action != null)
                    return moveActionRef.action.ReadValue<Vector2>();
                return Vector2.zero;
            }
        }

        public bool JumpTriggered
        {
            get
            {
                if (jumpActionRef != null && jumpActionRef.action != null)
                    return jumpActionRef.action.WasPressedThisFrame();
                return false;
            }
        }
    }
}