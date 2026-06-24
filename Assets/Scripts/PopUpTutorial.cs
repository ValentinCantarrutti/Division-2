using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopUpTutorial : MonoBehaviour
{

    public static bool TutorialActivo { get; private set; } = false;

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


    private Vector3 posicionCentroDestino;
    private Vector3 posicionBotonOrigen;
    private CanvasGroup canvasGroup;
    

    private CerrarAplicacion scriptPausa;

    void Start()
    {
        if (panelPopUp != null)
        {
            posicionCentroDestino = panelPopUp.transform.position;
            

            canvasGroup = panelPopUp.GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = panelPopUp.AddComponent<CanvasGroup>();
        }


        scriptPausa = Object.FindFirstObjectByType<CerrarAplicacion>();


        if (PlayerPrefs.GetInt("TutorialHecho", 0) == 0)
        {
            if (panelPopUp != null)
            {
                panelPopUp.SetActive(true);
                StartCoroutine(SecuenciaPopUp());
            }
        }
        else
        {
            if (panelPopUp != null) panelSalirInmediato();
            if (botonTutorial != null) botonTutorial.interactable = true;
        }
    }

    void Update()
    {

        if (botonTutorial != null)
        {
            if (TutorialActivo) return;

            if (scriptPausa != null && scriptPausa.panelSalir != null && scriptPausa.panelSalir.activeSelf)
            {
                if (botonTutorial.interactable) botonTutorial.interactable = false;
            }
            else
            {
                if (!botonTutorial.interactable) botonTutorial.interactable = true;
            }
        }
    }


    public void MostrarTutorialPorBoton()
    {
        if (scriptPausa != null && scriptPausa.panelSalir != null && scriptPausa.panelSalir.activeSelf) return;

        if (panelPopUp != null)
        {
            StopAllCoroutines(); 
            panelPopUp.SetActive(true);
            StartCoroutine(SecuenciaPopUp());
        }
    }

    private IEnumerator SecuenciaPopUp()
    {

        TutorialActivo = true;

        if (botonTutorial != null) botonTutorial.interactable = false;


        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        if (botonTutorial != null) posicionBotonOrigen = botonTutorial.transform.position;
        else posicionBotonOrigen = posicionCentroDestino;


        StartCoroutine(RalentizarJuegoGradual());


        panelPopUp.transform.position = posicionBotonOrigen;
        panelPopUp.transform.localScale = Vector3.zero;
        
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * velocidadAnimacionMenu;
            panelPopUp.transform.position = Vector3.Lerp(posicionBotonOrigen, posicionCentroDestino, t);
            float escalaActual = Mathf.Lerp(0f, 1.15f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * (velocidadAnimacionMenu * 1.5f);
            float escalaActual = Mathf.Lerp(1.15f, 1.0f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }
        
        panelPopUp.transform.position = posicionCentroDestino;
        panelPopUp.transform.localScale = Vector3.one;


        if (contenedorAnimacion != null)
        {
            for (int vuelta = 0; vuelta < 2; vuelta++)
            {
                for (int i = 0; i < animacion1.Length; i++)
                {
                    contenedorAnimacion.sprite = animacion1[i];
                    yield return new WaitForSecondsRealtime(tiempoPorFrame);
                }
            }

            for (int vuelta = 0; vuelta < 2; vuelta++)
            {
                for (int i = 0; i < animacion2.Length; i++)
                {
                    contenedorAnimacion.sprite = animacion2[i];
                    yield return new WaitForSecondsRealtime(tiempoPorFrame);
                }
            }
        }


        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * (velocidadAnimacionMenu * 2f);
            float escalaActual = Mathf.Lerp(1.0f, 1.12f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * velocidadAnimacionMenu;
            panelPopUp.transform.position = Vector3.Lerp(posicionCentroDestino, posicionBotonOrigen, t);
            float escalaActual = Mathf.Lerp(1.12f, 0f, t);
            panelPopUp.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        panelSalirInmediato();

        PlayerPrefs.SetInt("TutorialHecho", 1);
        PlayerPrefs.Save();

        if (botonTutorial != null)
        {
            bool pausaAbierta = scriptPausa != null && scriptPausa.panelSalir != null && scriptPausa.panelSalir.activeSelf;
            botonTutorial.interactable = !pausaAbierta;
        }
    }


    private IEnumerator RalentizarJuegoGradual()
    {
        float tiempoTranscurrido = 0f;
        float duracion = 0.5f;
        float escalaInicial = Time.timeScale;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(escalaInicial, 0f, tiempoTranscurrido / duracion);
            yield return null;
        }

        Time.timeScale = 0f; // Aseguramos el freno total al terminar los 0.5s
    }

    private void panelSalirInmediato()
    {
        panelPopUp.transform.position = posicionCentroDestino;
        panelPopUp.transform.localScale = new Vector3(0f, 0f, 1f);
        panelPopUp.SetActive(false);
        

        Time.timeScale = 1f;
        

        TutorialActivo = false;
    }
}