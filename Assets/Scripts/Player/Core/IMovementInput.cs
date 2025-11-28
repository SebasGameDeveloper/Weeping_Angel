using UnityEngine;

namespace Player.Core
{
    public interface IMovementInput
    {
        Vector2 MoveInput { get; }
        bool JumpTriggered { get; }
    }
}