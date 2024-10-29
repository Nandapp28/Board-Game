using DG.Tweening;
using UnityEngine;
using UnityEngine.UI; // Tambahkan ini untuk bekerja dengan UI

public class ActionCardAnimation : MonoBehaviour
{
    [Header("Kamera dan Posisi")]
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Jarak offset kartu dari kamera
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Skala target kartu
    
    [Header("Durasi Animasi")]
    public float animationDuration = 0.5f; // Durasi animasi umum
    public float activeDuration = 0.5f; // Durasi animasi aktif
    
    [Header("Gerakan dan Rotasi")]
    public Vector3 manualRotation = new Vector3(0, 0, 0); // Rotasi manual
    public Vector3 moveRightOffset = new Vector3(2f, 0f, 0f); // Offset untuk animasi ke kanan
    public Vector3 rotateCard = new Vector3(90, 90f, -90f); // Rotasi ke kanan (90 derajat di sumbu Y)

    [Header("UI Tombol")]
    public Button actionButton; // Referensi ke tombol UI yang akan ditampilkan
    public CanvasGroup actionButtonCanvasGroup; // Tambahkan referensi ke CanvasGroup untuk tombol
    public float buttonAnimationDuration = 0.3f; // Durasi animasi skala tombol

    // Variabel privat untuk posisi awal, skala, dan status target
    private Vector3 initialPosition;
    private Vector3 initialRotation;
    private Vector3 initialScale;

    private bool isAtTarget = false; // Menyimpan status apakah kartu sedang di posisi target
    private ActionSystem actionSystem;
    private StockCard stockCard;

    void Start()
    {
        // Jika cameraTransform tidak diset, gunakan kamera utama
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        actionSystem = FindObjectOfType<ActionSystem>();
        stockCard = GetComponent<StockCard>();

        // Awalnya sembunyikan tombol dengan skala 0 dan blokir raycast
        if (actionButton != null && actionButtonCanvasGroup != null)
        {
            actionButton.transform.localScale = Vector3.zero;
            actionButton.gameObject.SetActive(false);
            actionButtonCanvasGroup.blocksRaycasts = false; // Blokir interaksi tombol saat tidak aktif
        }
    }

    

    public void SetInitialPosition(Vector3 position)
    {
        initialPosition = position;
    }

    public void SetInitialScale(Vector3 scale)
    {
        initialScale = scale;
    }

    private void OnMouseDown()
    {
        if (isAtTarget)
        {
            // Animasi kembali ke posisi awal
            AnimateToInitial();
        }
        else
        {
            // Animasi ke posisi target di depan kamera
            AnimateToTarget();
        }

        if (actionSystem != null)
        {
            actionSystem.OnCardClick(this); // Kirim referensi kartu yang diklik ke deck
            SendStockCardToActionSystem();
        }

        // Tukar status posisi kartu
        isAtTarget = !isAtTarget;
    }
    public void SendStockCardToActionSystem()
    {
        if (actionSystem != null && stockCard != null)
        {
            actionSystem.ReceiveStockCard(stockCard); // Kirim StockCard ke ActionSystem
        }
        else
        {
            Debug.LogWarning("ActionSystem or StockCard is not assigned.");
        }
    }

    public void AnimateToTarget()
    {
        // Hitung posisi target agar selalu di depan kamera menggunakan transform kamera
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z  +
                                 cameraTransform.right * offsetFromCamera.x +
                                 cameraTransform.up * offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(manualRotation);

        // Animasi ke target menggunakan DoTween
        transform.DOMove(targetPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DORotateQuaternion(targetRotation, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(targetScale, animationDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            // Tampilkan dan animasikan tombol saat animasi selesai
            if (actionButton != null && actionButtonCanvasGroup != null)
            {
                actionButton.gameObject.SetActive(true);
                AnimateButtonScale(actionButton, true); // Animasi dari skala 0 ke 1
                actionButtonCanvasGroup.blocksRaycasts = true; // Aktifkan interaksi tombol setelah animasi
            }
        });
    }

    public void AnimateToInitial()
    {
        // Sembunyikan tombol dengan animasi skala sebelum animasi kembali ke posisi awal
        if (actionButton != null && actionButtonCanvasGroup != null)
        {
            actionButtonCanvasGroup.blocksRaycasts = false; // Nonaktifkan interaksi saat animasi menyembunyikan
            AnimateButtonScale(actionButton, false); // Animasi dari skala 1 ke 0
        }

        // Animasi kembali ke posisi awal menggunakan DoTween
        transform.DOLocalMove(initialPosition, animationDuration).SetEase(Ease.InOutQuad);
        transform.DOScale(initialScale, animationDuration).SetEase(Ease.InOutQuad);
    }

    public void ActiveCardAnimation()
    {
        Vector3 targetPosition = transform.position + moveRightOffset;

        // Animasi bergerak ke kanan
        transform.DOMove(targetPosition, activeDuration).SetEase(Ease.InOutQuad);

        // Animasi rotasi menggunakan DOLocalRotate dengan sudut Euler langsung
        transform.DOLocalRotate(rotateCard, activeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            // Hapus GameObject setelah animasi selesai
            Destroy(gameObject);
        });

        // Sembunyikan tombol dengan animasi skala sebelum animasi kembali ke posisi awal
        if (actionButton != null && actionButtonCanvasGroup != null)
        {
            actionButtonCanvasGroup.blocksRaycasts = false; // Nonaktifkan interaksi saat animasi menyembunyikan
            AnimateButtonScale(actionButton, false); // Animasi dari skala 1 ke 0
        }
    }

    // Fungsi untuk menganimasikan skala tombol dari 0 ke 1 atau sebaliknya
    public void AnimateButtonScale(Button button, bool show)
    {
        // Pastikan untuk mengambil RectTransform tombol
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        if (buttonRect != null)
        {
            if (show)
            {
                // Animasi dari skala 0 ke 1 untuk menampilkan tombol
                buttonRect.DOScale(Vector3.one, buttonAnimationDuration).SetEase(Ease.OutBack);
            }
            else
            {
                // Animasi dari skala 1 ke 0 untuk menyembunyikan tombol
                buttonRect.DOScale(Vector3.zero, buttonAnimationDuration).SetEase(Ease.InBack).OnComplete(() =>
                {
                    button.gameObject.SetActive(false);
                });
            }
        }
    }

    public void IsCardChanges(bool IsCardChanges)
    {
        AnimateButtonScale(actionButton, IsCardChanges); 
    }

    public bool IsAtTarget()
    {
        return isAtTarget; // Mengembalikan status apakah kartu berada di posisi target
    }
}
