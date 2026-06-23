using UnityEngine;

public class LimiteInferior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Buscamos exactamente tu Tag "Cubo" (en singular)
        bool tieneTagCubo = collision.CompareTag("Cubo");
        bool esCuboSimple = collision.GetComponent<Cuadradosimple>() != null;
        bool esBotonCorte = collision.GetComponent<BotonesMenu>() != null;

        if (tieneTagCubo || esCuboSimple || esBotonCorte)
        {
            // 2. Restamos 2 segundos al tiempo general
            if (BarraTiempo.Instancia != null)
            {
                BarraTiempo.Instancia.RestarTiempo(2f);
                Debug.Log($"¡Un cubo cayó al vacío! Se restaron 2 segundos. Nombre: {collision.gameObject.name}");
            }

            // 🔥 CONFIGURADO: Llama exactamente a tu función ResetearCombo()
            if (ComboManager.Instancia != null)
            {
                ComboManager.Instancia.ResetearCombo(); 
            }

            // 4. Avisamos al Spawner para que libere el lugar
            if (Spawnercubos.Instancia != null)
            {
                Spawnercubos.Instancia.RegistrarCuboRoto(); 
            }

            // 5. Lo destruimos
            Destroy(collision.gameObject);
        }
    }
}