using UnityEngine;

namespace Interfaces
{
    public interface IVisibilityDetector
    {
        //Evento disparado cuando cambia el estado de visibilidad.
        event System.Action<bool> OnVisibilityChanged;
        
        //Verifica si el enemigo es visible para el jugador
        bool IsVisible { get; }
        
        //Inicia la deteccion de visibilidad
        void StartDetection();
        
        //Detiene la deteccion de visibilidad
        void StopDetection();
    }

    public interface IMovement
    {
        //activa el movimiento hacia el objetivo
        void StartMoving();
        
        //Detiene el movimiento
        void StopMoving();
        
        //Actualiza la posicion del objetivo 
        void UpdateDestination(Vector3 targetPosition);
        
        //Verifica si el enemigo ha llegado al destino
        bool HasReachedDestination { get; }
    }

    public interface IEnemyState
    {
        //llamado al entrar al estado
        void OnEnter();
        
        //Llamado cada frame mientras esta activo
        void OnUpdate();
        
        //Llamado al salir del estado
        void OnExit();
    }
}