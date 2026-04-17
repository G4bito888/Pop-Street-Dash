using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // Lógica de Singleton para evitar duplicados al volver al menú
        if (instance == null)
        {
            instance = this;
            // Hace que el objeto sobreviva al cambiar de escena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si ya existe un manager, destruye el nuevo para no solapar música
            Destroy(gameObject);
        }
    }
}