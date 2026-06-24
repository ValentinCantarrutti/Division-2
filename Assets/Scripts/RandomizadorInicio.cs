using UnityEngine;

public class RandomizadorInicio : MonoBehaviour
{
    [System.Serializable]
    public struct ConfiguracionBoton
    {
        public string nombreReferencia; 
        [Tooltip("El cubo original que pusiste a mano en la escena.")]
        public GameObject cuboOriginalEscena; 
        
        [Header("Opciones de Prefabs")]
        [Tooltip("Lista de posibles cubos que pueden salir acá.")]
        public GameObject[] prefabsPosibles;

        [Header("Opciones de Rotación Z")]
        public bool rotarAleatorio;
        public float[] angulosPermitidos;
    }

    [Header("Configuración de los 3 Botones del Menú")]
    public ConfiguracionBoton[] botonesInicio;

    void Start()
    {
        RandomizarBotones();
    }

    void RandomizarBotones()
    {
        foreach (ConfiguracionBoton boton in botonesInicio)
        {
            if (boton.cuboOriginalEscena == null || boton.prefabsPosibles == null || boton.prefabsPosibles.Length == 0)
            {
                Debug.LogWarning("Falta configurar algún dato en el Randomizador de Inicio.");
                continue;
            }

            
            Vector3 posicionOriginal = boton.cuboOriginalEscena.transform.position;
            Vector3 escalaOriginal = boton.cuboOriginalEscena.transform.localScale;

            
            GameObject prefabElegido = boton.prefabsPosibles[Random.Range(0, boton.prefabsPosibles.Length)];

            
            Quaternion rotacionFinal = Quaternion.identity; 

            if (prefabElegido.GetComponent<BotonesMenu>() != null && boton.rotarAleatorio && boton.angulosPermitidos != null && boton.angulosPermitidos.Length > 0)
            {
                float anguloAzar = boton.angulosPermitidos[Random.Range(0, boton.angulosPermitidos.Length)];
                rotacionFinal = Quaternion.Euler(0f, 0f, anguloAzar);
            }

            
            Rigidbody2D rbPrefab = prefabElegido.GetComponent<Rigidbody2D>();
            bool rbEstabaActivado = false;
            if (rbPrefab != null)
            {
                rbEstabaActivado = rbPrefab.simulated;
                rbPrefab.simulated = false; 
            }

            
            GameObject nuevoCubo = Instantiate(prefabElegido, posicionOriginal, rotacionFinal);
            nuevoCubo.transform.localScale = escalaOriginal;

            
            if (rbPrefab != null)
            {
                rbPrefab.simulated = rbEstabaActivado;
            }

            
            Rigidbody2D rbClon = nuevoCubo.GetComponent<Rigidbody2D>();
            if (rbClon != null)
            {
                rbClon.bodyType = RigidbodyType2D.Kinematic; 
                rbClon.constraints = RigidbodyConstraints2D.FreezeRotation; 
                rbClon.velocity = Vector2.zero;             
                rbClon.angularVelocity = 0f;                
                
                
                rbClon.simulated = true; 
            }

            
            Destroy(boton.cuboOriginalEscena);
        }
    }
}