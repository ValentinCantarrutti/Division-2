using UnityEngine;

public class CerrarAplicacion : MonoBehaviour
{

    public void SalirDelJuego()
    {

        Debug.Log("Saliendo de la aplicación...");


        Application.Quit();

        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}