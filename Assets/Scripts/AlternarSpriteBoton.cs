using UnityEngine;
using UnityEngine.UI; 

public class AlternarSpriteBoton : MonoBehaviour
{
    private Image imagenBoton;
    
    public Sprite spriteOriginal; 
    public Sprite spriteAlternativo; 

    private bool usandoOriginal = true;

    void Start()
    {
        imagenBoton = GetComponent<Image>();
        
       
        if (imagenBoton != null && spriteOriginal != null)
        {
            imagenBoton.sprite = spriteOriginal;
        }
    }

   
    public void AlternarSprite()
    {
        if (imagenBoton == null || spriteOriginal == null || spriteAlternativo == null) return;

        if (usandoOriginal)
        {
            imagenBoton.sprite = spriteAlternativo;
        }
        else
        {
            imagenBoton.sprite = spriteOriginal;
        }

       
        usandoOriginal = !usandoOriginal;
    }
}