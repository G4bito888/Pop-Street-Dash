using UnityEngine;

public class Meta : MonoBehaviour
{
    [SerializeField] private GameObject panelGanaste;
    [SerializeField] private string nombreDeEsteNivel = "Nivel1"; 

    private void OnTriggerEnter2D(Collider2D collision)
{
    if (collision.CompareTag("Player"))
    {
        GameManager.nivelesCompletados.Add(nombreDeEsteNivel);
        FinalizarNivel();
    }
}

    private void FinalizarNivel()
    {
        if (panelGanaste != null)
        {
            panelGanaste.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}