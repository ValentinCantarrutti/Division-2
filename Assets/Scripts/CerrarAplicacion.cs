using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Necesario para reiniciar la escena actual

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
        
        // Nos aseguramos de que el juego arranque a velocidad normal
        Time.timeScale = 1f;
    }

    // 🚪 FUNCIÓN 1: Abre la ventana, ralentiza el juego y desactiva el cursor
    public void AbrirCartelSalida()
    {
        if (panelSalir != null)
        {
            StopAllCoroutines();
            panelSalir.SetActive(true);
            
            if (Cursor.Instance != null)
            {
                Cursor.Instance.SetActivo(false);
            }

            // 🔥 NUEVO: Validar si ya estamos en el Menú (viendo si el slider de tiempo está activo o no)
            ConfigurarBotonMenuSegunEstado();

            StartCoroutine(SecuenciaEntrada());
            StartCoroutine(RalentizarJuego(true)); 
        }
    }

    // ❌ FUNCIÓN 2: Cierra la ventana, reactiva el juego y devuelve el cursor
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

    // ⚠️ FUNCIÓN 3: Cierra el juego de verdad
    public void ConfirmarSalirDelJuego()
    {
        Debug.Log("Saliendo de la aplicación...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // 🏠 🔥 FUNCIÓN 4: Reinicia la escena exactamente igual que el Game Over
    public void VolverAlMenuPrincipal()
    {
        // Devolvemos el tiempo a la normalidad antes de recargar para que no empiece pausado
        Time.timeScale = 1f;
        
        // 🔥 Clave: Recarga la escena actual desde cero, igual que hace tu BarraTiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 🛠️ 🔥 FUNCIÓN AUXILIAR: Como es una sola escena, chequeamos si la barra de tiempo está visible.
    // Si la barra no está activa, significa que el jugador YA está en el menú principal.
    private void ConfigurarBotonMenuSegunEstado()
    {
        if (botonVolverAlMenu != null && BarraTiempo.Instancia != null)
        {
            // Si el slider de tiempo NO está activo en la pantalla, significa que ya estamos parados en el menú
            if (!BarraTiempo.Instancia.sliderTiempo.gameObject.activeSelf)
            {
                botonVolverAlMenu.interactable = false; // Se pone gris
            }
            else
            {
                botonVolverAlMenu.interactable = true;  // Se puede clickear en plena partida
            }
        }
    }

    // ==========================================
    // EFECTO SLOW-MOTION GRADUAL (0.5 SEGUNDOS)
    // ==========================================
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

    // ==========================================
    // ANIMACIONES DEL CARTEL
    // ==========================================

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

        if (botonSalir != null) botonSalir.interactable = true;
    }
}