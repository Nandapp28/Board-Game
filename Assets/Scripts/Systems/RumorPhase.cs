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
    public Vector3 initPosition;
    public Quaternion initRotation;
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
    [SerializeField] public TransformCamera Infrastuktur = new TransformCamera();
    [SerializeField] public TransformCamera Consumen = new TransformCamera();
    [SerializeField] public TransformCamera Finance = new TransformCamera();
    [SerializeField] public TransformCamera Mining = new TransformCamera();

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Kecepatan pergerakan kamera
    [SerializeField] private float waitTime = 10f; // Waktu tunggu antar sektor

    [Header("Rumor Card Animation Settings")]
    public RumorCardAnimator cardSettings; 

    private Camera MainCamera;
    private Vector3 initialPosition; // Menyimpan posisi awal
    private Quaternion initialRotation; // Menyimpan rotasi awal
    public TransformCamera[] sectors;
    public int CurrentSectorIndex = 0;
    private GameManager gameManager;
    private StockPriceManager stockPriceManager;

    #region Unity Lifecycle

    /// Memulai fase rumor dan menginisialisasi kamera.
    private void Start() {

        gameManager = FindAnyObjectByType<GameManager>();
        stockPriceManager = FindAnyObjectByType<StockPriceManager>();
        MainCamera = Camera.main; // Mengambil kamera utama
        cardSettings.cameraTransform = MainCamera.transform;
        initialPosition = MainCamera.transform.position; // Simpan posisi awal
        initialRotation = MainCamera.transform.rotation; // Simpan rotasi awal

        sectors = new TransformCamera[] { Infrastuktur,Consumen, Finance, Mining  };
        CurrentSectorIndex = 0; // Inisialisasi indeks sektor saat ini
        CollectCards();
    }
    public void StartRumorhPase() {
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
            ApplyRumorCardEffect(CurrentSectorIndex);
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
        if(sectors[CurrentSectorIndex].CurrentCard == null)
        {
            SelectRandomCard();
        }

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
    
    private void SelectRandomCard()
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
        sectors[CurrentSectorIndex].CurrentCard = null;
    }
    #endregion

    #region End Phase
    private void EndPhase()
    {
        CurrentSectorIndex = 0;
        Debug.Log("Rumor Phase Berakhir.");
        gameManager.currentGameState = GameManager.GameState.Resolution;
        gameManager.StartNextPhase();
    }
    #endregion

    #region Action Phase For Active Card Insider Trade
    public void MoveCardToCameraInsiderTrade(int index , float duration)
    {
        if (sectors[index].CurrentCard == null)
        {
            Debug.LogError("CurrentCard is null for sector index: " + index);
            return; // Keluar dari metode jika tidak ada kartu
        }

        // Hitung posisi target agar selalu di depan kamera
        Vector3 targetPosition = cardSettings.cameraTransform.position + cardSettings.cameraTransform.forward * cardSettings.offsetFromCamera.z +
                                cardSettings.cameraTransform.right * cardSettings.offsetFromCamera.x +
                                cardSettings.cameraTransform.up * cardSettings.offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(cardSettings.manualRotation);

        // Animasi ke target menggunakan DoTween
        sectors[index].CurrentCard.transform.DOMove(targetPosition, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() => {
                
            });
        sectors[index].CurrentCard.transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.InOutQuad);
    }

    public void MoveCardToinitInsiderTrade(int index, float duration)
    {
        if (sectors[index].CurrentCard == null)
        {
            Debug.LogError("CurrentCard is null for sector index: " + index);
            return; // Keluar dari metode jika tidak ada kartu
        }

        Vector3 initPosition = sectors[index].initPosition;
        Quaternion initRotation = sectors[index].initRotation; // Simpan rotasi awal sebagai Quaternion

        sectors[index].CurrentCard.transform.DOMove(initPosition, duration)
            .SetEase(Ease.InOutQuad);
        
        // Menggunakan rotasi awal yang disimpan
        sectors[index].CurrentCard.transform.DORotateQuaternion(initRotation, duration)
            .SetEase(Ease.InOutQuad);
    }
    public void SelectRandomCardInsideTrade(int index)
    {
        if (sectors[index].Cards.Count > 0) // Pastikan ada kartu di list sektor saat ini
        {
            int randomIndex = Random.Range(0, sectors[index].Cards.Count); // Pilih indeks acak
            sectors[index].CurrentCard = sectors[index].Cards[randomIndex]; // Ambil kartu yang dipilih dan simpan di CurrentCard
            Debug.Log($"Kartu terpilih: {sectors[index].CurrentCard.name}"); // Tampilkan nama kartu yang dipilih
            sectors[index].initPosition = sectors[index].CurrentCard.transform.position; // Simpan posisi awal kartu
            sectors[index].initRotation = sectors[index].CurrentCard.transform.rotation; // Simpan rotasi
            sectors[index].Cards.RemoveAt(randomIndex);
        }
        else
        {
            Debug.Log("Tidak ada kartu untuk dipilih.");
        }
    }

    #endregion

    #region Rumor Card Effect

    private void ApplyRumorCardEffect(int index)
    {
        RumorCard rumorCard = sectors[index].CurrentCard.GetComponent<RumorCard>();

        switch (rumorCard.Type)
        {
            case RumorCard.RumorType.ExtraFee:
                break;
            case RumorCard.RumorType.Merger:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,3);
                break;
            case RumorCard.RumorType.CompetitiveTender:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,1);
                break;
            case RumorCard.RumorType.AssetRevaluation:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,1);
                break;
            case RumorCard.RumorType.Expansion:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.WageIncrease:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.StockBuyback:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,1);
                break;
            case RumorCard.RumorType.TaxReduction:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.EconomicStimulus:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                IncreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.RupiahDepreciation:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.StockIssuance:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,1);
                break;
            case RumorCard.RumorType.AuditBribery:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.ForensicAudit:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.FinancialCrisis:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,2);
                break;
            case RumorCard.RumorType.FinancialDeficit:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,3);
                break;
            case RumorCard.RumorType.EconomicRecession:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                DecreaseSector(rumorCard,1);
                break;
            case RumorCard.RumorType.EconomicReform:
                Debug.Log("kartu Rumor Effect " + rumorCard.Type + " Diaktifkan");
                break;
            default:
                break;
        }
    }

    private void IncreaseSector(RumorCard rumorCard, int Increase)
    {
        switch (rumorCard.Connected_Sectors)
        {
            case RumorCard.Sector.Consumen:
                stockPriceManager.IncreaseCurrentPriceIndex(stockPriceManager.allSector.Consumen ,Increase);
                break;
            case RumorCard.Sector.Infrastuctur:
                stockPriceManager.IncreaseCurrentPriceIndex(stockPriceManager.allSector.Infrastuctur ,Increase);
                break;
            case RumorCard.Sector.Finance:
                stockPriceManager.IncreaseCurrentPriceIndex(stockPriceManager.allSector.Finance ,Increase);
                break;
            case RumorCard.Sector.Mining:
                stockPriceManager.IncreaseCurrentPriceIndex(stockPriceManager.allSector.Mining ,Increase);
                break;
        }
    }
    private void DecreaseSector(RumorCard rumorCard, int Decrease)
    {
        switch (rumorCard.Connected_Sectors)
        {
            case RumorCard.Sector.Consumen:
                stockPriceManager.DecreaseCurrentPriceIndex(stockPriceManager.allSector.Consumen ,Decrease);
                break;
            case RumorCard.Sector.Infrastuctur:
                stockPriceManager.DecreaseCurrentPriceIndex(stockPriceManager.allSector.Infrastuctur ,Decrease);
                break;
            case RumorCard.Sector.Finance:
                stockPriceManager.DecreaseCurrentPriceIndex(stockPriceManager.allSector.Finance ,Decrease);
                break;
            case RumorCard.Sector.Mining:
                stockPriceManager.DecreaseCurrentPriceIndex(stockPriceManager.allSector.Mining ,Decrease);
                break;
        }
    }

    #endregion
}