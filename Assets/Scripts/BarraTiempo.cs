using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using System.Collections;

public class BarraTiempo : MonoBehaviour
{
    public static BarraTiempo Instancia { get; private set; }

    public Slider sliderTiempo; 
    public float tiempoMaximo = 30f; 
    
    private float tiempoActual;
    private bool juegoTerminado = false;
    private bool tiempoActivo = false; 

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        tiempoActual = tiempoMaximo;

        if (sliderTiempo != null)
        {
            sliderTiempo.maxValue = tiempoMaximo;
            sliderTiempo.value = tiempoMaximo;
            sliderTiempo.gameObject.SetActive(false); 
        }
    }

    public void IniciarContador()
    {
        tiempoActivo = true;
        if (sliderTiempo != null)
        {
            sliderTiempo.gameObject.SetActive(true); 
        }

        
        if (ComboManager.Instancia != null)
        {
            ComboManager.Instancia.IniciarComboEnPartida();
        }
    }

    void Update()
    {
        if (juegoTerminado || !tiempoActivo) return;

        tiempoActual -= Time.deltaTime;
        
        if (sliderTiempo != null)
        {
            sliderTiempo.value = tiempoActual;
        }

        if (tiempoActual <= 0)
        {
            TerminarJuego();
        }
    }

    public void SumarTiempo(float segundos)
    {
        if (juegoTerminado || !tiempoActivo) return;

        tiempoActual += segundos;

        if (tiempoActual > tiempoMaximo)
        {
            tiempoActual = tiempoMaximo;
        }

        if (sliderTiempo != null)
        {
            sliderTiempo.value = tiempoActual;
        }
    }

    public void RestarTiempo(float segundos)
{
    if (juegoTerminado || !tiempoActivo) return;

    tiempoActual -= segundos;

    
    if (tiempoActual < 0)
    {
        tiempoActual = 0;
    }

    if (sliderTiempo != null)
    {
        sliderTiempo.value = tiempoActual;
    }

    if (tiempoActual <= 0)
    {
        TerminarJuego();
    }
}

   void TerminarJuego()
    {
        juegoTerminado = true;
        tiempoActivo = false;
        Debug.Log("¡Tiempo agotado! Fin de la partida.");

        if (Cursor.Instance != null)
        {
            Cursor.Instance.SetActivo(false);
        }

        

        GameOverAnimacion gameOverScript = FindFirstObjectByType<GameOverAnimacion>();
        if (gameOverScript != null)
        {
            gameOverScript.ActivarGameOver();
        }

        StartCoroutine(DesacelerarTiempo(0.5f));
        StartCoroutine(EsperarYReiniciar(4f)); 
    }

   
    IEnumerator DesacelerarTiempo(float duracionFreno)
    {
        float tiempoPasado = 0f;

        while (tiempoPasado < duracionFreno)
        {
            
            tiempoPasado += Time.unscaledDeltaTime; 
            
           
            float t = tiempoPasado / duracionFreno;

          
            Time.timeScale = Mathf.Lerp(1f, 0f, t);
            
            yield return null;
        }

       
        Time.timeScale = 0f;
    }

   
    IEnumerator EsperarYReiniciar(float segundos)
    {
        yield return new WaitForSecondsRealtime(segundos);

      
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}