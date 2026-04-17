using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.3f; // Tiempo aproximado para alcanzar al jugador
    
    private Vector3 currentVelocity = Vector3.zero; // Referencia interna para el motor de suavizado
    private float initialY;
    private float initialZ;

    void Start()
    {
        initialY = transform.position.y;
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Definimos la posición destino (solo X)
        Vector3 targetPosition = new Vector3(target.position.x, initialY, initialZ);

        // Aplicamos SmoothDamp en lugar de Lerp
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref currentVelocity, 
            smoothTime
        );
    }
}