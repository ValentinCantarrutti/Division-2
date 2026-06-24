using UnityEngine;

public class Cuadradosimple : MonoBehaviour
{
    [Header("Efecto de Destrucción")]
    [Tooltip("Arrastrá acá el Prefab 'PartesCuadrado' que creamos.")]
    public GameObject prefabPartes;

    [Header("Efecto de Audio")]
    [Tooltip("Arrastrá acá el sonido (.mp3, .wav) que querés que suene al romper el cuadrado.")]
    public AudioClip sonidoRotura;
    [Range(0f, 1f)]
    public float volumenSonido = 0.8f;

    private bool yaSeRompio = false;

    private void OnMouseDown()
    {
        
        if (PopUpTutorial.TutorialActivo) return;

        
        if (Time.timeScale < 1f) return;

        if (yaSeRompio) return;
        yaSeRompio = true;

        if (sonidoRotura != null)
        {
            Vector3 posicionOido = Camera.main != null ? Camera.main.transform.position : transform.position;
            posicionOido.z = 0f; 
            AudioSource.PlayClipAtPoint(sonidoRotura, posicionOido, volumenSonido);
        }

        if (BarraTiempo.Instancia != null) BarraTiempo.Instancia.SumarTiempo(2f);
        if (Spawnercubos.Instancia != null) Spawnercubos.Instancia.RegistrarCuboRoto();

        if (BarraTiempo.Instancia != null && BarraTiempo.Instancia.sliderTiempo.gameObject.activeSelf)
        {
            if (ComboManager.Instancia != null)
            {
                ComboManager.Instancia.IncrementarCombo();
            }
        }

        if (prefabPartes != null)
        {
            GameObject contenedorPartes = Instantiate(prefabPartes, transform.position, transform.rotation);
            Rigidbody2D[] rbHijos = contenedorPartes.GetComponentsInChildren<Rigidbody2D>();

            foreach (Rigidbody2D rb in rbHijos)
            {
                Vector2 direccionExplosion = rb.transform.localPosition.normalized;
                float fuerzaImpulso = 3f;

                if (rb.gameObject.name.Contains("Arriba"))
                {
                    direccionExplosion += Vector2.up * 1.5f;
                    fuerzaImpulso = 3.5f; 
                }

                rb.AddForce(direccionExplosion.normalized * fuerzaImpulso, ForceMode2D.Impulse);
                rb.AddTorque(Random.Range(-180f, 180f));
            }

            Destroy(contenedorPartes, 4f);
        }

        Destroy(gameObject);
    }
}