using UnityEngine;
using System.Collections;
using TMPro; 

public class Spawnercubos : MonoBehaviour
{
    public GameObject cuboPrefab;

    public Transform[] spawnPoints;
    public int cantidadPorRonda = 3;

    public float delayEntreSpawns = 0.3f;
    public float fuerzaSalto = 3f;

    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad constante de giro. Valores entre 90 y 180 suelen quedar muy bien.")]
    public float velocidadRotacion = 120f; 
    [Tooltip("Si está activo, algunos cubos girarán a la izquierda y otros a la derecha de forma aleatoria.")]
    public bool direccionAleatoria = true;

    [Header("UI & Delay Inicial")]
    [Tooltip("El texto del título que se va a difuminar al empezar.")]
    public TextMeshProUGUI textoTitulo; 
    [Tooltip("Tiempo de espera en segundos antes de que aparezca el primer cubo.")]
    public float delayInicialInicial = 2f;

    private bool sistemaActivo = false;
    private bool spawning = false;
    private bool esPrimeraRonda = true; 

    void Update()
    {
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

    IEnumerator SpawnRonda()
    {
        spawning = true;


        if (esPrimeraRonda)
        {
            esPrimeraRonda = false;


            if (textoTitulo != null)
            {
                StartCoroutine(DesvanecerTexto(delayInicialInicial));
            }


            yield return new WaitForSeconds(delayInicialInicial);
        }

        for (int i = 0; i < cantidadPorRonda; i++)
        {
            Transform sp = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject c = Instantiate(cuboPrefab, sp.position, Quaternion.identity);


            Collider2D colliderPadre = c.GetComponent<Collider2D>();
            Collider2D[] collidersHijos = c.GetComponentsInChildren<Collider2D>();

            if (colliderPadre != null && collidersHijos.Length > 0)
            {
                foreach (Collider2D colHijo in collidersHijos)
                {
                    if (colHijo != colliderPadre)
                    {
                        Physics2D.IgnoreCollision(colliderPadre, colHijo, true);
                    }
                }
            }


            Rigidbody2D rb = c.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.freezeRotation = false;
                rb.velocity = Vector2.zero;
                
                float giro = velocidadRotacion;
                if (direccionAleatoria)
                {
                    giro *= Random.Range(0, 2) == 0 ? 1f : -1f;
                }
                
                rb.angularVelocity = giro;
                rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
            }

            Destroy(c, Random.Range(5f, 10f));

            yield return new WaitForSeconds(delayEntreSpawns);
        }

        spawning = false;
    }


    IEnumerator DesvanecerTexto(float tiempoTotal)
    {
        float tiempoPasado = 0f;
        Color colorOriginal = textoTitulo.color;

        while (tiempoPasado < tiempoTotal)
        {
            tiempoPasado += Time.deltaTime;
            

            float alpha = Mathf.Lerp(1f, 0f, tiempoPasado / tiempoTotal);
            

            textoTitulo.color = new Color(colorOriginal.r, colorOriginal.g, colorOriginal.b, alpha);
            
            yield return null; // Espera al siguiente frame
        }

   
        textoTitulo.gameObject.SetActive(false);
    }
}