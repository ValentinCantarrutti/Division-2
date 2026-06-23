using UnityEngine;
using System.Collections;
using TMPro;

public class GameOverAnimacion : MonoBehaviour
{
    [Header("Componentes UI")]
    [Tooltip("Arrastrá acá el texto de Game Over (o el panel entero que querés que rebote).")]
    public RectTransform elementoAnimado;

    [Header("Configuración del Rebote (Gelatina)")]
    [Tooltip("Cuánto tiempo dura toda la animación de entrada.")]
    public float duracionEfecto = 0.6f;
    
    [Tooltip("Qué tan grande se hace en su punto máximo antes de rebotar.")]
    public float escalaMaxima = 1.4f;

    [Tooltip("Qué tan elástico es el rebote. A mayor número, más veces rebota antes de quedarse quieto.")]
    public float elasticidad = 3f;

    private void Start()
    {
       
        if (elementoAnimado != null)
        {
            elementoAnimado.localScale = Vector3.zero;
            elementoAnimado.gameObject.SetActive(false);
        }
    }

  
    public void ActivarGameOver()
    {
        if (elementoAnimado == null) return;

        elementoAnimado.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimarEntradaGelatina());
    }

   IEnumerator AnimarEntradaGelatina()
    {
       
        elementoAnimado.localScale = Vector3.zero;

        
        float tiempoSubida = duracionEfecto * 0.4f;
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoSubida)
        {
            tiempoPasado += Time.unscaledDeltaTime;
            
            float t = Mathf.Sin((tiempoPasado / tiempoSubida) * Mathf.PI * 0.5f);
            
           
            float escala = Mathf.Lerp(0f, escalaMaxima, t);
            elementoAnimado.localScale = new Vector3(escala, escala, 1f);
            yield return null;
        }

       
        float tiempoBajada = duracionEfecto * 0.6f;
        tiempoPasado = 0f;

        Vector3 escalaDesde = new Vector3(escalaMaxima, escalaMaxima, 1f);
        Vector3 escalaHacia = Vector3.one; 

        while (tiempoPasado < tiempoBajada)
        {
            tiempoPasado += Time.unscaledDeltaTime;
            float t = tiempoPasado / tiempoBajada;
            
          
            t = Mathf.SmoothStep(0f, 1f, t);

            elementoAnimado.localScale = Vector3.Lerp(escalaDesde, escalaHacia, t);
            yield return null;
        }

      
        elementoAnimado.localScale = Vector3.one;
    }
}