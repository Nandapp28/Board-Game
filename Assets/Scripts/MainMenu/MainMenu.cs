using UnityEngine;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject playerPlatform;
    [SerializeField] private float animationDuration = 0.5f;
     [SerializeField] private Ease openEase = Ease.OutBounce;  // Variabel baru untuk easing saat membuka
    [SerializeField] private Ease closeEase = Ease.InBounce;  // Variabel baru untuk easing saat menutup

    private RectTransform playerPlatformRect;

    private void Awake()
    {
        InitializePlayerPlatform();
    }

    private void InitializePlayerPlatform()
    {
        if (playerPlatform == null)
        {
            Debug.LogError("Player Platform tidak diatur di Inspector.");
            return;
        }

        playerPlatformRect = playerPlatform.GetComponent<RectTransform>();
        if (playerPlatformRect == null)
        {
            Debug.LogError("Komponen RectTransform tidak ditemukan pada Player Platform.");
            return;
        }

        playerPlatformRect.localScale = Vector3.zero;
        playerPlatform.SetActive(false);
    }

    public void OnClickGameMode()
    {
        if (playerPlatformRect == null)
        {
            Debug.LogError("Animasi tidak dapat dijalankan karena RectTransform tidak diinisialisasi.");
            return;
        }

        ActivateAndAnimatePlayerPlatform();
    }

    private void ActivateAndAnimatePlayerPlatform()
    {
        playerPlatform.SetActive(true);
        playerPlatformRect.DOScale(Vector3.one, animationDuration).SetEase(openEase);
    }

    public void onClickClosePlayerPlatform(){
        playerPlatformRect.DOScale(Vector3.zero, animationDuration).SetEase(closeEase)
                            .OnComplete(()=> playerPlatform.SetActive(false));
    }
}
