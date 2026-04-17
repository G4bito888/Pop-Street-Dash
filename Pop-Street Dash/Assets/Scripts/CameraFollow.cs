using UnityEngine;

public class CameraDeadZone : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform target;

    [Header("Configuración de Suavizado")]
    [SerializeField] private float smoothTimeX = 0.1f;
    [SerializeField] private float smoothTimeY = 0.3f;

    [Header("Límites de la Zona Muerta (Relativos)")]
    [SerializeField] private float upperLimit = 2f; // Distancia hacia arriba antes de mover
    [SerializeField] private float lowerLimit = 2f; // Distancia hacia abajo antes de mover

    [Header("Restricciones de Mundo")]
    [SerializeField] private float minY = -25f;

    private Vector3 velocity = Vector3.zero;
    private float targetY;
    private float initialZ;

    void Start()
    {
        initialZ = transform.position.z;
        targetY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- Lógica Vertical Relativa ---
        float currentCamY = transform.position.y;
        float playerY = target.position.y;

        // Si el jugador sube más allá del límite superior de la cámara
        if (playerY > currentCamY + upperLimit)
        {
            targetY = playerY - upperLimit;
        }
        // Si el jugador baja más allá del límite inferior de la cámara
        else if (playerY < currentCamY - lowerLimit)
        {
            targetY = playerY + lowerLimit;
        }

        // Aplicamos el límite del suelo (-25)
        float finalTargetY = Mathf.Max(targetY, minY);

        // --- Movimiento Final ---
        float posX = Mathf.SmoothDamp(transform.position.x, target.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, finalTargetY, ref velocity.y, smoothTimeY);

        transform.position = new Vector3(posX, posY, initialZ);
    }
}