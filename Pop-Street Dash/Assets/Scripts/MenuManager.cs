using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelOpciones;
    
    [Header("Input System")]
    [SerializeField] private Player_Inputs playerInput; 

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject fondoOscurecido;

void Update()
{
    if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        ToggleOpciones();
    }
}
    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.Player.Esc.performed += OnEscTriggered;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.Player.Esc.performed -= OnEscTriggered;
        }
    }

    private void OnEscTriggered(InputAction.CallbackContext context)
    {
        ToggleOpciones();
    }

    public void ToggleOpciones()
{
    if (panelOpciones != null)
    {
        bool estaActivado = !panelOpciones.activeSelf;
        
        panelOpciones.SetActive(estaActivado);
        
        if (fondoOscurecido != null)
        {
            fondoOscurecido.SetActive(estaActivado);
        }

        Time.timeScale = estaActivado ? 0f : 1f;
    }
}


    public void CambiarEscena(string nombreEscena)
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(nombreEscena);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado.");
    }

    
}