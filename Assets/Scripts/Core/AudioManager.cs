using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("BackSound")]
    public AudioClip BackSound;

    [Header("SoundEffect")]
    public AudioClip ButtonSound;
    public AudioClip DiceSound;
    private AudioSource audioSource;


    void Start()
    {
        AudioClip[] allaudioclips = new AudioClip[]
        {
            BackSound,
        };
        AudioClip[] AllSoundEffect = new AudioClip[]
        {
            ButtonSound,
            DiceSound
        };
        audioSource = GetComponent<AudioSource>();  // Mendapatkan AudioSource
        AudioController.Initialize(audioSource,allaudioclips,AllSoundEffect);
    }
}
