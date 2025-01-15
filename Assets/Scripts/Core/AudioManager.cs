using UnityEngine;

public class AudioManagers : MonoBehaviour
{
    // Instance singleton
    public static AudioManagers instance;

    [Header("BackSound")]
    public AudioClip BackSoundMainMenu;
    public AudioClip BackSoundInGame;

    [Header("SoundEffect")]
    public AudioClip ButtonSound;
    public AudioClip DiceSound;
    public AudioClip TransitionPhase;
    private AudioSource audioSource;

    public AudioClip[] audioClips;  // Array untuk menyimpan banyak AudioClip
    public AudioClip[] soundEffect;

    public bool isMusicPlaying = false;

    // Menginisialisasi AudioSource dan AudioClip
    void Start()
    {
        AudioClip[] allaudioclips = new AudioClip[]
        {
            BackSoundMainMenu,BackSoundInGame
        };
        AudioClip[] AllSoundEffect = new AudioClip[]
        {
            ButtonSound,
            DiceSound,
            TransitionPhase
        };
        audioSource = GetComponent<AudioSource>();  // Mendapatkan AudioSource
        AudioManagers.instance.Initialize(audioSource,allaudioclips,AllSoundEffect);
    }
    public void Initialize(AudioSource source, AudioClip[] clips, AudioClip[] SoundEffect)
    {
        audioSource = source;
        audioClips = clips;
        soundEffect = SoundEffect;
    }

    // Memutar musik berdasarkan index
    public void PlayMusic(int index)
    {
        if (audioSource != null && audioClips != null && audioClips.Length > index)
        {
            audioSource.clip = audioClips[index];  // Menetapkan clip berdasarkan indeks
            audioSource.loop = true;
            audioSource.Play();  // Memulai pemutaran
            isMusicPlaying = true;
        }
        else
        {
            Debug.LogWarning("AudioSource atau AudioClip tidak ditemukan atau index tidak valid!");
        }
    }

    // Menghentikan musik
    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            isMusicPlaying = false;
        }
    }

    public void PlaySoundEffect(int index, float volume = 1.0f)
    {
        if (audioSource != null && soundEffect != null && soundEffect.Length > index)
        {
            audioSource.PlayOneShot(soundEffect[index], volume);  // Memutar sound effect dengan volume yang ditentukan
        }
        else
        {
            Debug.LogWarning("AudioSource atau SoundEffect tidak ditemukan atau index tidak valid!");
        }
    }

    private void Awake()
    {
        // Jika instance belum ada, buat instance baru
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Jangan hapus instance saat scene berubah
        }
        else
        {
            // Jika instance sudah ada, hapus instance ini
            Destroy(gameObject);
        }
    }
    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
        else
        {
            Debug.LogWarning("AudioSource tidak ditemukan!");
        }
    }
}