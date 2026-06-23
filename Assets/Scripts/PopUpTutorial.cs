using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTutorial : MonoBehaviour
{
    [Header("Componentes de la UI")]
    [Tooltip("Arrastrá acá el objeto Panel/Imagen que funciona como cartel pop-up.")]
    public GameObject panelPopUp;

    [Tooltip("Arrastrá acá el botón del signo de pregunta del menú.")]
    public Button botonTutorial; 

    [Tooltip("Arrastrá acá el componente Image que está ADENTRO del panel y muestra el dibujo.")]
    public Image contenedorAnimacion;

    [Header("Frames de las Animaciones")]
    [Tooltip("Arrastrá acá los sprites de la PRIMERA imagen.")]
    public Sprite[] animacion1;
    
    [Tooltip("Arrastrá acá los sprites de la SEGUNDA imagen.")]
    public Sprite[] animacion2;

    [Header("Configuración de Tiempos")]
    [Tooltip("Qué tan rápido se desplaza y escala el cartel.")]
    public float velocidadAnimacionMenu = 4f;

    [Tooltip("Tiempo en segundos entre cada frame del GIF (ej: 0.15).")]
    public float tiempoPorFrame = 0.15f;

    // Variables internas para guardar el camino
    private Vector3 posicionCentroDestino;
    private Vector3 posicionBotonOrigen;

    void Start()
    {
        // Guardamos la posición central ideal que ya configuraste en tu Canvas
        if (panelPopUp != null)
        {
            posicionCentroDestino = panelPopUp.transform.position;
        }

        // Sistema de apertura automática la primera vez
        if (PlayerPrefs.GetInt("TutorialHecho", 0) == 0)
        {
            if (panelPopUp != null)
            {
                panelPopUp.SetActive(true);
                if (botonTutorial != null) botonTutorial.interactable = false;
                StartCoroutine(SecuenciaPopUp());
            }
        }
        else
        {
            if (panelPopUp != null) panelSalirInmediato();
            if (botonTutorial != null) botonTutorial.interactable = true;
        }
    }

    // Función que se asigna en el OnClick del botón de la esquina
    public void MostrarTutorialPorBoton()
    {
        if (panelPopUp != null)
        {
            StopAllCoroutines(); 
            panelPopUp.SetActive(true);
            StartCoroutine(SecuenciaPopUp());
        }
    }

    private IEnumerator SecuenciaPopUp()
    {
        if (botonTutorial != null) botonTutorial.interactable = false;

        // Detectamos dónde está el botón en la pantalla para usarlo como punto de salida
        if (botonTutorial != null)
        {
            posicionBotonOrigen = botonTutorial.transform.position;
        }
        else
        {
            posicionBotonOrigen = posicionCentroDestino;
        }

        // ==========================================
        // 🚀 ENTRADA: NACE DESDE EL BOTÓN HACIA EL CENTRO
        // ==========================================
        panelPopUp.transform.position = posicionBotonOrigen;
        panelPopUp.transform.localScale = Vector3.zero;
        
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadAnimacionMenu;
            
            // Va del botón al centro mientras se agranda a 1.15 para el efecto "pop"
            panelPopUp.transform.position = Vector3.Lerp(posicionBotonOrigen, posicionCentroDestino, t);
            float escalaActual = Mathf.Lerp(0f, 1.15f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadAnimacionMenu * 1.5f);
            // Se asienta en su escala normal (1.0)
            float escalaActual = Mathf.Lerp(1.15f, 1.0f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }
        
        panelPopUp.transform.position = posicionCentroDestino;
        panelPopUp.transform.localScale = Vector3.one;

        // ==========================================
        // 🎬 REPRODUCCIÓN DE LAS TUS ANIMACIONES
        // ==========================================
        if (contenedorAnimacion != null)
        {
            for (int vuelta = 0; vuelta < 2; vuelta++)
            {
                for (int i = 0; i < animacion1.Length; i++)
                {
                    contenedorAnimacion.sprite = animacion1[i];
                    yield return new WaitForSeconds(tiempoPorFrame);
                }
            }

            for (int vuelta = 0; vuelta < 2; vuelta++)
            {
                for (int i = 0; i < animacion2.Length; i++)
                {
                    contenedorAnimacion.sprite = animacion2[i];
                    yield return new WaitForSeconds(tiempoPorFrame);
                }
            }
        }

        // ==========================================
        // 📉 SALIDA: SE ENCOGE Y VUELVE AL BOTÓN
        // ==========================================
        t = 0f;
        // Impulso hacia afuera antes de irse
        while (t < 1f)
        {
            t += Time.deltaTime * (velocidadAnimacionMenu * 2f);
            float escalaActual = Mathf.Lerp(1.0f, 1.12f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        // Viaja del centro directo al botón achicándose a 0
        while (t < 1f)
        {
            t += Time.deltaTime * velocidadAnimacionMenu;
            
            panelPopUp.transform.position = Vector3.Lerp(posicionCentroDestino, posicionBotonOrigen, t);
            float escalaActual = Mathf.Lerp(1.12f, 0f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        panelSalirInmediato();

        PlayerPrefs.SetInt("TutorialHecho", 1);
        PlayerPrefs.Save();

        if (botonTutorial != null) botonTutorial.interactable = true;
    }

    private void panelSalirInmediato()
    {
        panelPopUp.transform.position = posicionCentroDestino;
        panelPopUp.transform.localScale = new Vector3(0f, 0f, 1f);
        panelPopUp.SetActive(false);
    }
}