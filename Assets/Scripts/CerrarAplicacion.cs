using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class CerrarAplicacion : MonoBehaviour
{
    [Header("Componentes de la UI")]
    [Tooltip("Arrastrá acá el cartel pop-up (en tu caso, el CartelPausa).")]
    public GameObject panelSalir;

    [Tooltip("Arrastrá acá el botón de la esquina que abre esta ventana.")]
    public Button botonSalir;

    [Tooltip("Arrastrá acá el botón de 'Volver al Menú' que está adentro del cartel.")]
    public Button botonVolverAlMenu;

    [Header("Configuración de Animación")]
    [Tooltip("Qué tan rápido se agranda al aparecer y se achica al cerrar.")]
    public float velocidadAnimacion = 5f;

    void Start()
    {
        if (panelSalir != null) panelSalir.SetActive(false);
        if (botonSalir != null) botonSalir.interactable = true;
        
       
        Time.timeScale = 1f;
    }

    void Update()
    {
       
        if (botonSalir != null)
        {
            
            if (PopUpTutorial.TutorialActivo)
            {
                if (botonSalir.interactable) botonSalir.interactable = false;
            }
            else
            {
               
                if (!botonSalir.interactable && (panelSalir == null || !panelSalir.activeSelf))
                {
                    botonSalir.interactable = true;
                }
            }
        }
    }

    
    public void AbrirCartelSalida()
    {
        
        if (PopUpTutorial.TutorialActivo) return;

        if (panelSalir != null)
        {
            StopAllCoroutines();
            panelSalir.SetActive(true);
            
            if (Cursor.Instance != null)
            {
                Cursor.Instance.SetActivo(false);
            }

            
            ConfigurarBotonMenuSegunEstado();

            StartCoroutine(SecuenciaEntrada());
            StartCoroutine(RalentizarJuego(true)); 
        }
    }

    
    public void CancelarSalida()
    {
        if (panelSalir != null)
        {
            StopAllCoroutines();
            
            if (Cursor.Instance != null)
            {
                Cursor.Instance.SetActivo(true);
            }

            StartCoroutine(SecuenciaSalida());
            StartCoroutine(RalentizarJuego(false)); 
        }
    }

    
    public void ConfirmarSalirDelJuego()
    {
        Debug.Log("Saliendo de la aplicación...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    
    public void VolverAlMenuPrincipal()
    {
       
        Time.timeScale = 1f;
        
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    
    private void ConfigurarBotonMenuSegunEstado()
    {
        if (botonVolverAlMenu != null && BarraTiempo.Instancia != null)
        {
            
            if (!BarraTiempo.Instancia.sliderTiempo.gameObject.activeSelf)
            {
                botonVolverAlMenu.interactable = false; 
            }
            else
            {
                botonVolverAlMenu.interactable = true;  
            }
        }
    }

    
    private IEnumerator RalentizarJuego(bool pausar)
    {
        float tiempoTranscurrido = 0f;
        float duracion = 0.5f; 
        float escalaInicial = Time.timeScale;
        float escalaFinal = pausar ? 0f : 1f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.unscaledDeltaTime; 
            Time.timeScale = Mathf.Lerp(escalaInicial, escalaFinal, tiempoTranscurrido / duracion);
            yield return null;
        }

        Time.timeScale = escalaFinal; 
    }



    private IEnumerator SecuenciaEntrada()
    {
        if (botonSalir != null) botonSalir.interactable = false;

        panelSalir.transform.localScale = new Vector3(0f, 0f, 1f);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * velocidadAnimacion;
            float escalaActual = Mathf.Lerp(0f, 1.15f, t);
            panelSalir.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * (velocidadAnimacion * 1.5f);
            float escalaActual = Mathf.Lerp(1.15f, 1.0f, t);
            panelSalir.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        panelSalir.transform.localScale = Vector3.one;
    }

    private IEnumerator SecuenciaSalida()
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * (velocidadAnimacion * 2f);
            float escalaActual = Mathf.Lerp(1.0f, 1.12f, t);
            panelSalir.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * velocidadAnimacion;
            float escalaActual = Mathf.Lerp(1.12f, 0f, t);
            panelSalir.transform.localScale = new Vector3(escalaActual, escalaActual, 1f);
            yield return null;
        }

        panelSalir.transform.localScale = new Vector3(0f, 0f, 1f);
        panelSalir.SetActive(false);

        if (botonSalir != null && !PopUpTutorial.TutorialActivo) botonSalir.interactable = true;
    }
}