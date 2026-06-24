using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI; 

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instancia { get; private set; }

    [Header("UI de Combo Actual")]
    [Tooltip("Arrastrá acá el texto de TextMeshPro que va a mostrar el combo de la jugada (ej: X2).")]
    public TextMeshProUGUI textoCombo;

    [Header("UI de Máximo Combo")]
    [Tooltip("Arrastrá acá el texto de TextMeshPro que va a mostrar el Max Combo histórico.")]
    public TextMeshProUGUI textoMaxCombo;

    [Header("UI de Soporte")]
    [Tooltip("Arrastrá acá el botón que abre el tutorial (Signo de pregunta).")]
    public Button botonTutorial; 

    private int comboActual = 0;
    private int maxComboHistorico = 0; 
    private Coroutine corrutinaAnimacion;
    private bool partidaActiva = false; 

    void Awake()
    {
        
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);

        
        maxComboHistorico = PlayerPrefs.GetInt("MaxComboGuardado", 0);
    }

    void Start()
    {
        
        if (textoCombo != null) textoCombo.gameObject.SetActive(false);
        
        
        ActualizarTextoRecord();

        
        if (botonTutorial != null) botonTutorial.interactable = true;
    }

    public void IniciarComboEnPartida()
    {
        partidaActiva = true;
        comboActual = 0;
        
        if (textoCombo != null)
        {
            textoCombo.gameObject.SetActive(true);
            textoCombo.text = "X0"; 
        }

        ActualizarTextoRecord();

        
        if (botonTutorial != null) 
        {
            botonTutorial.interactable = false;
        }
    }

    public void IncrementarCombo()
    {
        if (!partidaActiva) IniciarComboEnPartida();

        comboActual++;

       
        if (comboActual > maxComboHistorico)
        {
            maxComboHistorico = comboActual;
            PlayerPrefs.SetInt("MaxComboGuardado", maxComboHistorico);
            PlayerPrefs.Save(); 
        }

        ActualizarUI();

       
        if (corrutinaAnimacion != null) StopCoroutine(corrutinaAnimacion);
        corrutinaAnimacion = StartCoroutine(EfectoPopTexto());
    }

    public void ResetearCombo()
    {
        if (!partidaActiva) return;

        comboActual = 0;
        ActualizarUI();
    }

    public void DesactivarComboPostPartida()
    {
        partidaActiva = false;
        comboActual = 0;
        
        if (textoCombo != null) pointerHide();

        ActualizarTextoRecord(); 

        
        if (botonTutorial != null) 
        {
            botonTutorial.interactable = true;
        }
    }

    void ActualizarUI()
    {
        if (textoCombo != null && partidaActiva)
        {
            textoCombo.text = $"X{comboActual}";
        }

        ActualizarTextoRecord();
    }

    void ActualizarTextoRecord()
    {
        if (textoMaxCombo != null)
        {
            if (!textoMaxCombo.gameObject.activeSelf) 
            {
                textoMaxCombo.gameObject.SetActive(true);
            }
            
            textoMaxCombo.text = $"MaxCombo: <b>{maxComboHistorico}</b>";
        }
    }

    private void pointerHide()
    {
        if (textoCombo != null) textoCombo.gameObject.SetActive(false);
    }

    IEnumerator EfectoPopTexto()
    {
        Vector3 escalaOriginal = Vector3.one;
        Vector3 escalaMax = Vector3.one * 1.3f;

        float tiempo = 0f;
        while (tiempo < 0.05f)
        {
            tiempo += Time.unscaledDeltaTime;
            textoCombo.transform.localScale = Vector3.Lerp(escalaOriginal, escalaMax, tiempo / 0.05f);
            yield return null;
        }

        tiempo = 0f;
        while (tiempo < 0.1f)
        {
            tiempo += Time.unscaledDeltaTime;
            textoCombo.transform.localScale = Vector3.Lerp(escalaMax, escalaOriginal, tiempo / 0.1f);
            yield return null;
        }

        textoCombo.transform.localScale = escalaOriginal;
    }
}