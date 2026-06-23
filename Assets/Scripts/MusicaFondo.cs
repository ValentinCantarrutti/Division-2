using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    private static MusicaFondo instanciaMusica;

    void Awake()
    {
        // Si ya existe una instancia de la música sonando, destruye la nueva para no duplicar
        if (instanciaMusica != null && instanciaMusica != this)
        {
            Destroy(gameObject);
            return;
        }

        // Si es la primera vez que arranca, se vuelve inmortal
        instanciaMusica = this;
        DontDestroyOnLoad(gameObject);
    }
}