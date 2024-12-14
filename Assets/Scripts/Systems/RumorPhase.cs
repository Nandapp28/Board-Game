using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

[System.Serializable]
public class TransformCamera {
    public Vector3 Position;
    public Vector3 Rotation = new Vector3(65,90,0);
    public GameObject rumorCards;
    public List<GameObject> Cards = new List<GameObject>(); // Inisialisasi list Cards
    public GameObject CurrentCard;
}

[System.Serializable]
public class RumorCardAnimator {
    public Transform cameraTransform; // Transform kamera
    public Vector3 offsetFromCamera; // Offset dari kamera
    public Vector3 manualRotation; // Rotasi manual untuk kartu
    public float animationDuration = 1f; // Durasi animasi
}

public class RumorPhase : MonoBehaviour {

    [Header("Camera Position And Rotation Settings")]
    [SerializeField] private TransformCamera Infrastuktur = new TransformCamera();
    [SerializeField] private TransformCamera Consumen = new TransformCamera();
    [SerializeField] private TransformCamera Finance = new TransformCamera();
    [SerializeField] private TransformCamera Mining = new TransformCamera();

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Kecepatan pergerakan kamera
    [SerializeField] private float waitTime = 10f; // Waktu tunggu antar sektor

    [Header("Rumor Card Animation Settings")]
    public RumorCardAnimator cardSettings; 

    private Camera MainCamera;
    private Vector3 initialPosition; // Menyimpan posisi awal
    private Quaternion initialRotation; // Menyimpan rotasi awal
    private TransformCamera[] sectors;
    private int CurrentSectorIndex;
    private GameManager gameManager;

    #region Unity Lifecycle

    /// Memulai fase rumor dan menginisialisasi kamera.
    public void StartRumorhPase() {
        gameManager = FindAnyObjectByType<GameManager>();
        MainCamera = Camera.main; // Mengambil kamera utama
        cardSettings.cameraTransform = MainCamera.transform;
        initialPosition = MainCamera.transform.position; // Simpan posisi awal
        initialRotation = MainCamera.transform.rotation; // Simpan rotasi awal

        sectors = new TransformCamera[] { Infrastuktur,Consumen, Finance, Mining  };
        CurrentSectorIndex = 0; // Inisialisasi indeks sektor saat ini

        CollectCards();

        StartCoroutine(MoveCameraThroughSectors());
    }

    // metode untuk mengambil semua child di Cards
    private void CollectCards() {
        foreach (var sector in sectors) {
            sector.Cards.Clear(); // Kosongkan list sebelum mengumpulkan
            if (sector.rumorCards != null) {
                foreach (Transform child in sector.rumorCards.transform) {
                    sector.Cards.Add(child.gameObject); // Tambahkan child ke dalam list
                }
            }
        }
    }
    #endregion

    #region Camera Movement Coroutines
    /// Menggerakkan kamera melalui semua sektor.
    private IEnumerator MoveCameraThroughSectors() {
        while (CurrentSectorIndex < sectors.Length) {
            yield return StartCoroutine(MoveCameraToSector(sectors[CurrentSectorIndex]));
            AnimateCurrentCard();
            yield return new WaitForSeconds(waitTime); // Tunggu sebelum pindah ke sektor berikutnya
            CurrentSectorIndex++;
        }

        // Setelah semua sektor, kembali ke posisi awal
        yield return StartCoroutine(MoveCameraToInitialPosition());
        EndPhase();
    }

    /// Memindahkan kamera ke sektor tertentu.
    /// <param name="sector">Sektor yang dituju.</param>
    private IEnumerator MoveCameraToSector(TransformCamera sector) {
        Vector3 startPosition = MainCamera.transform.position;
        Quaternion startRotation = MainCamera.transform.rotation;
        Vector3 targetPosition = sector.Position;
        Quaternion targetRotation = Quaternion.Euler(sector.Rotation);

        float elapsedTime = 0f;

        // Interpolasi posisi dan rotasi kamera
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            MainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Pastikan posisi dan rotasi akhir tepat
        MainCamera.transform.position = targetPosition;
        MainCamera.transform.rotation = targetRotation;
    }

    /// Mengembalikan kamera ke posisi awal.
    private IEnumerator MoveCameraToInitialPosition() {
        Vector3 startPosition = MainCamera.transform.position;
        Quaternion startRotation = MainCamera.transform.rotation;
        Vector3 targetPosition = initialPosition;
        Quaternion targetRotation = initialRotation;

        float elapsedTime = 0f;

        // Interpolasi posisi dan rotasi kamera kembali ke posisi awal
        while (elapsedTime < 1f) {
            elapsedTime += Time.deltaTime * moveSpeed;
            MainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            MainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime);
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Pastikan posisi dan rotasi akhir tepat
        MainCamera.transform.position = targetPosition;
        MainCamera.transform.rotation = targetRotation;
    }

    #endregion

    #region Rumor Card Animation
    public void AnimateCurrentCard()
    {
        // Pilih kartu secara acak dari sektor saat ini
        SelectRandomCard();

        // Pastikan ada kartu yang dipilih
        if (sectors[CurrentSectorIndex].CurrentCard != null)
        {
            // Pindahkan kartu yang dipilih ke posisi di depan kamera
            MoveCardToCamera();
        }
        else
        {
            Debug.Log("Tidak ada kartu yang dipilih untuk dianimasikan.");
        }
    }
    
    public void SelectRandomCard()
    {
        if (sectors[CurrentSectorIndex].Cards.Count > 0) // Pastikan ada kartu di list sektor saat ini
        {
            int randomIndex = Random.Range(0, sectors[CurrentSectorIndex].Cards.Count); // Pilih indeks acak
            sectors[CurrentSectorIndex].CurrentCard = sectors[CurrentSectorIndex].Cards[randomIndex]; // Ambil kartu yang dipilih dan simpan di CurrentCard
            Debug.Log($"Kartu terpilih: {sectors[CurrentSectorIndex].CurrentCard.name}"); // Tampilkan nama kartu yang dipilih
            
            sectors[CurrentSectorIndex].Cards.RemoveAt(randomIndex);
        }
        else
        {
            Debug.Log("Tidak ada kartu untuk dipilih.");
        }
    }

    private void MoveCardToCamera()
    {
        // Hitung posisi target agar selalu di depan kamera
        Vector3 targetPosition = cardSettings.cameraTransform.position + cardSettings.cameraTransform.forward * cardSettings.offsetFromCamera.z +
                                 cardSettings.cameraTransform.right * cardSettings.offsetFromCamera.x +
                                 cardSettings.cameraTransform.up * cardSettings.offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(cardSettings.manualRotation);

        // Animasi ke target menggunakan DoTween
        sectors[CurrentSectorIndex].CurrentCard.transform.DOMove(targetPosition, cardSettings.animationDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                ActiveCardEffect(); // Panggil metode ActiveCardEffect setelah animasi selesai
            });
        sectors[CurrentSectorIndex].CurrentCard.transform.DORotateQuaternion(targetRotation, cardSettings.animationDuration).SetEase(Ease.InOutQuad);
    }
    private void ActiveCardEffect()
    {
        Debug.Log("Kartu Rumor Diaktifkan");
        DestoryCard();
    }
    private void DestoryCard()
    {
        Destroy(sectors[CurrentSectorIndex].CurrentCard, 2);
        Debug.Log("Kartu Rumor " + sectors[CurrentSectorIndex].CurrentCard.name + " Berhasil Di hapus");
    }
    #endregion

    #region End Phase
    private void EndPhase()
    {
        Debug.Log("Rumor Phase Berakhir.");
        gameManager.currentGameState = GameManager.GameState.Resolution;
        gameManager.StartNextPhase();
    }
    #endregion
}