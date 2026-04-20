using UnityEngine;
using UnityEngine.UI;

public class IconoNivel : MonoBehaviour
{
    [SerializeField] private string nombreDelNivel = "Nivel1";
    [SerializeField] private Sprite spriteCompletado;
    
    private Image imagenUI;
    private Sprite spriteOriginal;

    void Awake()
    {
        imagenUI = GetComponent<Image>();
        spriteOriginal = imagenUI.sprite;
    }

    void OnEnable()
    {
        // Verificación en la clase estática
        if (GameManager.nivelesCompletados.Contains(nombreDelNivel))
        {
            imagenUI.sprite = spriteCompletado;
        }
        else
        {
            imagenUI.sprite = spriteOriginal;
        }
    }
}