using UnityEngine;

public class Cuadradosimple : MonoBehaviour
{
    [Header("Efecto de Destrucción")]
    [Tooltip("Arrastrá acá el Prefab 'PartesCuadrado' que creamos en el Paso 1.")]
    public GameObject prefabPartes;

    [Header("Efecto de Audio")]
    [Tooltip("Arrastrá acá el sonido (.mp3, .wav) que querés que suene al romper el cuadrado.")]
    public AudioClip sonidoRotura;
    [Range(0f, 1f)]
    public float volumenSonido = 0.8f;

    private bool yaSeRompio = false;

    private void OnMouseDown()
    {
        // 🛑 NUEVO: Si el juego está en pausa o ralentizándose (timeScale menor a 1), ignorar el click
        if (Time.timeScale < 1f) return;

        if (yaSeRompio) return;
        yaSeRompio = true;

        // 🔥 Reproduce el sonido de rotura en el espacio 2D
        if (sonidoRotura != null)
        {
            Vector3 posicionOido = Camera.main != null ? Camera.main.transform.position : transform.position;
            posicionOido.z = 0f; // Asegura que no se desfase en el eje Z
            AudioSource.PlayClipAtPoint(sonidoRotura, posicionOido, volumenSonido);
        }

        // Lógica del juego y tiempos
        if (BarraTiempo.Instancia != null) BarraTiempo.Instancia.SumarTiempo(2f);
        if (Spawnercubos.Instancia != null) Spawnercubos.Instancia.RegistrarCuboRoto();

        // Candado del combo para la partida
        if (BarraTiempo.Instancia != null && BarraTiempo.Instancia.sliderTiempo.gameObject.activeSelf)
        {
            if (ComboManager.Instancia != null)
            {
                ComboManager.Instancia.IncrementarCombo();
            }
        }

        // 🔥 APARECEN LOS PEDAZOS EXPLOTANDO
        if (prefabPartes != null)
        {
            // Clonamos las partes en la posición y rotación actual del cubo entero
            GameObject contenedorPartes = Instantiate(prefabPartes, transform.position, transform.rotation);

            // Buscamos los Rigidbody2D de los 4 pedacitos hijos
            Rigidbody2D[] rbHijos = contenedorPartes.GetComponentsInChildren<Rigidbody2D>();

            foreach (Rigidbody2D rb in rbHijos)
            {
                // Dirección base hacia afuera (diagonal)
                Vector2 direccionExplosion = rb.transform.localPosition.normalized;
                
                // Fuerza base de la explosión
                float fuerzaImpulso = 3f;

                // 🚀 CLAVE: Si el objeto hijo contiene la palabra "Arriba" en su nombre,
                // le alteramos la dirección agregándole fuerza extra hacia arriba (Vector2.up)
                if (rb.gameObject.name.Contains("Arriba"))
                {
                    // Sumamos un empuje vertical extra (ej: 1.5f más hacia arriba)
                    direccionExplosion += Vector2.up * 1.5f;
                    fuerzaImpulso = 3.5f; // Un toque más de fuerza general para vencer la gravedad inicial
                }

                // Aplicamos el impulso final modificado y un giro aleatorio
                rb.AddForce(direccionExplosion.normalized * fuerzaImpulso, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-180f, 180f));
            }

            // Limpieza: destruye el contenedor de pedazos en 4 segundos
            Destroy(contenedorPartes, 4f);
        }

        // Desaparece el bloque entero original
        Destroy(gameObject);
    }
}