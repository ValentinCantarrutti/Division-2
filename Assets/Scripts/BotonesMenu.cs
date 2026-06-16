using UnityEngine;

public class BotonesMenu : MonoBehaviour
{
    public GameObject mitadIzquierda;
    public GameObject mitadDerecha;

    [Header("Configuración del Corte")]
    [Tooltip("Qué tan vertical debe ser el corte. 1 es perfectamente vertical, 0 es cualquier diagonal hacia abajo.")]
    [Range(0f, 1f)]
    public float precisionVertical = 0.7f; 

    private Collider2D col;
    private bool cortado = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    void Update()
{
    if (cortado) return;

    if (Cursor.Instance != null && Cursor.Instance.Points.Count >= 2)
    {
        Vector3 puntoA = Cursor.Instance.Points[^2];
        Vector3 puntoB = Cursor.Instance.Points[^1];

        Vector2 direccionTajo = puntoB - puntoA;
        float distancia = direccionTajo.magnitude;

        if (distancia > 0)
        {
            RaycastHit2D[] hits = Physics2D.LinecastAll(puntoA, puntoB);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider == col)
                {
                    
                    Vector2 abajoLocalDelCubo = -transform.up; 
                    
                    float componenteHaciaAbajo = Vector2.Dot(direccionTajo.normalized, abajoLocalDelCubo);

                    if (componenteHaciaAbajo > precisionVertical)
                    {
                        Cortar();
                        break; 
                    }
                }
            }
        }
    }
}

    void Cortar()
    {
        if (cortado) return;
        cortado = true;

        mitadIzquierda.transform.SetParent(null);
        mitadDerecha.transform.SetParent(null);

        Rigidbody2D rbIzq = mitadIzquierda.GetComponent<Rigidbody2D>();
        Rigidbody2D rbDer = mitadDerecha.GetComponent<Rigidbody2D>();

        rbIzq.bodyType = RigidbodyType2D.Dynamic;
        rbDer.bodyType = RigidbodyType2D.Dynamic;

        rbIzq.AddForce(Vector2.left * 2f, ForceMode2D.Impulse);
        rbDer.AddForce(Vector2.right * 2f, ForceMode2D.Impulse);

        rbIzq.AddTorque(100f);
        rbDer.AddTorque(-100f);

        Destroy(mitadIzquierda, 5f);
        Destroy(mitadDerecha, 5f);
        Destroy(gameObject);
    }
}