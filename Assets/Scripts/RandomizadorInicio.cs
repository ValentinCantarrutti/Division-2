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

            // 1. Guardamos los datos de posición y escala originales
            Vector3 posicionOriginal = boton.cuboOriginalEscena.transform.position;
            Vector3 escalaOriginal = boton.cuboOriginalEscena.transform.localScale;

            // 2. Elegimos un prefab al azar
            GameObject prefabElegido = boton.prefabsPosibles[Random.Range(0, boton.prefabsPosibles.Length)];

            // 3. Calculamos la rotación inteligente
            Quaternion rotacionFinal = Quaternion.identity; // Por defecto: 0 grados en todo (perfectamente derecho)

            // 🚀 CLAVE: Si es el cubo de corte (BotonesMenu), le permitimos rotar. 
            // Si es el Cuadradosimple, ignoramos la rotación y se queda en 0 grados.
            if (prefabElegido.GetComponent<BotonesMenu>() != null && boton.rotarAleatorio && boton.angulosPermitidos != null && boton.angulosPermitidos.Length > 0)
            {
                float anguloAzar = boton.angulosPermitidos[Random.Range(0, boton.angulosPermitidos.Length)];
                rotacionFinal = Quaternion.Euler(0f, 0f, anguloAzar);
            }

            // 4. Clonamos el cubo
            GameObject nuevoCubo = Instantiate(prefabElegido, posicionOriginal, rotacionFinal);
            nuevoCubo.transform.localScale = escalaOriginal;

            // 🛡️ CONTROL FÍSICO LIMPIO PARA EL MENÚ
            Rigidbody2D rb = nuevoCubo.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic; 
                rb.velocity = Vector2.zero;             
                rb.angularVelocity = 0f;                
            }

            // 5. Destruimos el cubo viejo de la escena
            Destroy(boton.cuboOriginalEscena);
        }
    }
}