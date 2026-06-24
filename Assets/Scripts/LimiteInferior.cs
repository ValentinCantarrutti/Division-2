using UnityEngine;

public class LimiteInferior : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        bool tieneTagCubo = collision.CompareTag("Cubo");
        bool esCuboSimple = collision.GetComponent<Cuadradosimple>() != null;
        bool esBotonCorte = collision.GetComponent<BotonesMenu>() != null;

        if (tieneTagCubo || esCuboSimple || esBotonCorte)
        {
            
            if (BarraTiempo.Instancia != null)
            {
                BarraTiempo.Instancia.RestarTiempo(2f);
                Debug.Log($"¡Un cubo cayó al vacío! Se restaron 2 segundos. Nombre: {collision.gameObject.name}");
            }

           
            if (ComboManager.Instancia != null)
            {
                ComboManager.Instancia.ResetearCombo(); 
            }

           
            if (Spawnercubos.Instancia != null)
            {
                Spawnercubos.Instancia.RegistrarCuboRoto(); 
            }

            
            Destroy(collision.gameObject);
        }
    }
}