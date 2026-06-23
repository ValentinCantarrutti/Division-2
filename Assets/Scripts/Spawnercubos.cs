using UnityEngine;
using System.Collections;
using TMPro;

public class Spawnercubos : MonoBehaviour
{
    public static Spawnercubos Instancia { get; private set; }

    [Header("Prefabs de Cubos")]
    public GameObject[] cubosPrefabs;
    public Transform[] spawnPoints;
    public int cantidadPorRonda = 3;

    [Header("Ritmo y Dificultad (Velocidad Fija)")]
    [Tooltip("La velocidad constante que querés como objetivo (ej: 0.5). A mayor número, más rápido salen.")]
    public float velocidadBase100Percent = 0.5f;
    
    [Tooltip("Cuánto aumenta la velocidad de spawn por cada cubo destruido.")]
    public float incrementoPorCuboRoto = 0.05f;
    
    private float velocidadActual;
    private float delayEntreSpawns;

    [Header("Configuración de Salto (Rango Aleatorio)")]
    [Tooltip("Fuerza mínima de salto para los cubos (ej: 8).")]
    public float fuerzaSaltoMin = 8f;
    [Tooltip("Fuerza máxima de salto para los cubos (ej: 12).")]
    public float fuerzaSaltoMax = 12f;

    [Header("Configuración de Rotación")]
    public float velocidadRotacion = 120f;
    public bool direccionAleatoria = true;

    [Header("UI & Delay Inicial")]
    public TextMeshProUGUI textoTitulo;
    public TextMeshProUGUI textoSecundario; 
    public TextMeshProUGUI textoMaxComboMenu; 
    public float delayInicialInicial = 2f;
    
    [Tooltip("Arrastrá acá el objeto que tiene el script de la Barra de Tiempo.")]
    public BarraTiempo barraDeTiempo; 

    [Header("Audio de Transición Inicial")]
    [Tooltip("Sonido que se reproduce al romper los botones del inicio, mientras el texto se desvanece.")]
    public AudioClip sonidoTransicionInicio;
    [Range(0f, 1f)]
    public float volumenTransicion = 0.8f;

    private bool sistemaActivo = false;
    private bool spawning = false;
    private bool esPrimeraRonda = true;

    void Awake()
    {
        if (Instancia == null) Instancia = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        velocidadActual = velocidadBase100Percent * 0.75f;
        CalcularDelay();

        if (textoMaxComboMenu != null)
        {
            int maxComboGuardado = PlayerPrefs.GetInt("MaxComboGuardado", 0);
            textoMaxComboMenu.text = $"Max Combo: {maxComboGuardado}";
            textoMaxComboMenu.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        // En tu escena única, asumimos que usás tags para limpiar bloques antiguos al reiniciar
        int cubos = GameObject.FindGameObjectsWithTag("Cubo").Length;

        if (!sistemaActivo && cubos == 0)
        {
            sistemaActivo = true;
            StartCoroutine(SpawnRonda());
        }

        if (sistemaActivo && cubos == 0 && !spawning)
        {
            StartCoroutine(SpawnRonda());
        }
    }

    public void RegistrarCuboRoto()
    {
        velocidadActual += incrementoPorCuboRoto;
        CalcularDelay();
    }

    void CalcularDelay()
    {
        delayEntreSpawns = 1f / velocidadActual;
    }

    IEnumerator SpawnRonda()
    {
        spawning = true;

        if (esPrimeraRonda)
        {
            esPrimeraRonda = false;

            if (sonidoTransicionInicio != null)
            {
                Vector3 posicionOido = Camera.main != null ? Camera.main.transform.position : transform.position;
                posicionOido.z = 0f;
                AudioSource.PlayClipAtPoint(sonidoTransicionInicio, posicionOido, volumenTransicion);
            }

            if (textoTitulo != null) StartCoroutine(DesvanecerTexto(textoTitulo, delayInicialInicial));
            if (textoSecundario != null) StartCoroutine(DesvanecerTexto(textoSecundario, delayInicialInicial));
            if (textoMaxComboMenu != null) StartCoroutine(DesvanecerTexto(textoMaxComboMenu, delayInicialInicial));

            yield return new WaitForSeconds(delayInicialInicial);

            if (barraDeTiempo != null)
            {
                barraDeTiempo.IniciarContador();
            }
        }

        if (cubosPrefabs == null || cubosPrefabs.Length == 0)
        {
            spawning = false;
            yield break;
        }

        for (int i = 0; i < cantidadPorRonda; i++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject cuboElegido = cubosPrefabs[Random.Range(0, cubosPrefabs.Length)];

            // 1. Instanciamos en posición, rotación por defecto
            GameObject c = Instantiate(cuboElegido, sp.position, Quaternion.identity);

            // ... (Lógica de colisiones hijos igual) ...
             Collider2D colliderPadre = c.GetComponent<Collider2D>();
            Collider2D[] collidersHijos = c.GetComponentsInChildren<Collider2D>();
            if (colliderPadre != null && collidersHijos.Length > 0)
            {
                foreach (Collider2D colHijo in collidersHijos)
                {
                    if (colHijo != colliderPadre) Physics2D.IgnoreCollision(colliderPadre, colHijo, true);
                }
            }

            Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 🔥 SOLUCIÓN: Primero apagamos Constraints para que respete el giro
                rb.freezeRotation = false;

                // 🔥 NUEVO: Aplicamos rotación aleatoria al Transform (0, 90, 180, 270)
                float[] angulosPosibles = { 0f, 90f, 180f, 270f };
                float anguloAleatorioZ = angulosPosibles[Random.Range(0, angulosPosibles.Length)];
                c.transform.rotation = Quaternion.Euler(0f, 0f, anguloAleatorioZ);

                rb.velocity = Vector2.zero;
               
                float giro = velocidadRotacion;
                if (direccionAleatoria) giro *= Random.Range(0, 2) == 0 ? 1f : -1f;
                rb.angularVelocity = giro;

                // Salto vertical puro (Vector2.up)
                float fuerzaSaltoAleatoria = Random.Range(fuerzaSaltoMin, fuerzaSaltoMax);
                rb.AddForce(new Vector2(0f, fuerzaSaltoAleatoria), ForceMode2D.Impulse);
            }

            Destroy(c, Random.Range(5f, 10f));
            yield return new WaitForSeconds(delayEntreSpawns);
        }

        spawning = false;
    }

    IEnumerator DesvanecerTexto(TextMeshProUGUI texto, float tiempoTotal)
    {
        float tiempoPasado = 0f;
        Color colorOriginal = texto.color;
        while (tiempoPasado < tiempoTotal)
        {
            tiempoPasado += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, tiempoPasado / tiempoTotal);
            texto.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            yield return null; 
        }
        texto.gameObject.SetActive(false);
    }
}