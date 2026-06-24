using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    private static MusicaFondo instanciaMusica;

    void Awake()
    {
       
        if (instanciaMusica != null && instanciaMusica != this)
        {
            Destroy(gameObject);
            return;
        }

       
        instanciaMusica = this;
        DontDestroyOnLoad(gameObject);
    }
}