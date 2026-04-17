using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject PanelOpciones;
    // Carga la escena por su nombre exacto en el Build Settings
    public void CambiarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // Carga la escena por su número de índice
    public void CambiarEscenaPorIndice(int indiceEscena)
    {
        SceneManager.LoadScene(indiceEscena);
    }
    public void MostrarOpciones(bool estado)
    {
        PanelOpciones.SetActive(estado);
    }
    public void SalirDelJuego()
    {
        Application.Quit();
        Debug.Log("El juego se ha cerrado"); // Solo visible en el editor
    }
    
}