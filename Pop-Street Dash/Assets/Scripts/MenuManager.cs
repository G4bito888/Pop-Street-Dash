using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelOpciones;
    
    [Header("Input System")]
    [SerializeField] private Player_Inputs playerInput; // Arrastra el objeto con el componente PlayerInput aquí

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject fondoOscurecido;

void Update()
{
    // Si usas el nuevo Input System:
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        ToggleOpciones();
    }
}
    private void OnEnable()
    {
        // Se suscribe a la acción "Esc" definida en tu Action Map
        if (playerInput != null)
        {
            playerInput.Player.Esc.performed += OnEscTriggered;
        }
    }

    private void OnDisable()
    {
        // Desuscripción obligatoria para evitar Memory Leaks
        if (playerInput != null)
        {
            playerInput.Player.Esc.performed -= OnEscTriggered;
        }
    }

    // Callback disparado por el Input System
    private void OnEscTriggered(InputAction.CallbackContext context)
    {
        ToggleOpciones();
    }

    // Método principal para abrir/cerrar (Toggle)
    public void ToggleOpciones()
{
    if (panelOpciones != null)
    {
        bool estaActivado = !panelOpciones.activeSelf;
        
        panelOpciones.SetActive(estaActivado);
        
        // Activamos el fondo oscuro al mismo tiempo
        if (fondoOscurecido != null)
        {
            fondoOscurecido.SetActive(estaActivado);
        }

        Time.timeScale = estaActivado ? 0f : 1f;
    }
}

    // --- Métodos para Botones Físicos (UI) ---

    public void CambiarEscena(string nombreEscena)
    {
        // Importante: Asegurar que el tiempo vuelva a 1 antes de cambiar de escena
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado.");
    }

    
}