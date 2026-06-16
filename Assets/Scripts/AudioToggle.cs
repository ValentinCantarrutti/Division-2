using UnityEngine;

public class AudioToggle : MonoBehaviour
{
    public AudioSource musica;
    private bool muted = false;

    public void ToggleAudio()
    {
        muted = !muted;
        musica.mute = muted;
    }
}