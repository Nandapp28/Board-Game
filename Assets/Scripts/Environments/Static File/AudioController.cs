using UnityEngine;

public static class AudioController
{
    private static AudioSource audioSource;
    public static AudioClip[] audioClips;  // Array untuk menyimpan banyak AudioClip
    public static AudioClip[] soundEffect;

    // Menginisialisasi AudioSource dan AudioClip
    public static void Initialize(AudioSource source, AudioClip[] clips,AudioClip[] SoundEffect)
    {
        audioSource = source;
        audioClips = clips;
        soundEffect = SoundEffect;
    }

    // Memutar musik berdasarkan index
    public static void PlayMusic(int index)
    {
        if (audioSource != null && audioClips != null && audioClips.Length > index)
        {
            audioSource.clip = audioClips[index];  // Menetapkan clip berdasarkan indeks
            audioSource.loop = true;
            audioSource.Play();  // Memulai pemutaran
        }
        else
        {
            Debug.LogWarning("AudioSource atau AudioClip tidak ditemukan atau index tidak valid!");
        }
    }

    // Menghentikan musik
    public static void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public static void PlaySoundEffect(int index)
    {
        if (audioSource != null && soundEffect != null && soundEffect.Length > index)
        {
            audioSource.PlayOneShot(soundEffect[index]);  // Memutar sound effect
        }
        else
        {
            Debug.LogWarning("AudioSource atau SoundEffect tidak ditemukan atau index tidak valid!");
        }
    }
}
