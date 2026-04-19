using UnityEngine;

public class Meta : MonoBehaviour
{
    [Header ("Panel")]
    [SerializeField] private GameObject panelGanaste;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detecta si el objeto que entró es el Jugador
        if (collision.CompareTag("Player"))
        {
            FinalizarNivel();
        }
    }

    private void FinalizarNivel()
    {
        if (panelGanaste != null)
        {
            panelGanaste.SetActive(true);
            Time.timeScale = 0f; // Detiene el movimiento y físicas
            
            // Si quieres que el ratón aparezca para clicar el botón:
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}