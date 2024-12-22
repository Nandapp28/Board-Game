using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ActionPhase : MonoBehaviour
{
    [Header("Game Objects")]

    [Header("Player Management")]
    public CardManager cardManager; // Komponen CardManager

    [Header("UI Elements")]
    public Button ActiveButton; // Tombol untuk aksi aktif
    public Button KeepButton; // Tombol untuk menyimpan kartu
    [SerializeField] private TextMeshProUGUI timerText; // Referensi ke komponen TextMeshPro untuk timer

    [Header("Game Settings")]
    public const float selectionTime = 20f; // Waktu pemilihan dalam detik

    [Header("Game State")]
    private int currentPlayerIndex = 0; // Indeks pemain saat ini
    private Coroutine currentTimerCoroutine; // Menyimpan coroutine timer saat ini
    private CameraAnimation CameraAnimation;
    private GameManager gameManager;
    private bool IsFlashBuy = false;
    private int FlasbuyRemains = 0;
    private StockPriceManager stockPriceManager;
    private RumorPhase rumorPhase;
    public List<Vector3> CameraTransformPosition;
    public List<Vector3> CameraTransformRotation;
    private Camera mainCamera;
    private Vector3 InitializationCameraPosition;
    private Vector3 InitializationCameraRotation;
    private PlayerManager playerManager;


    #region Initialization
    // Memulai fase aksi dengan mengumpulkan pemain dan memulai pengambilan kartu

    private void Start() {
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        mainCamera = Camera.main;
        InitializationCameraPosition = mainCamera.transform.position;
        InitializationCameraRotation = mainCamera.transform.eulerAngles;
        gameManager = FindAnyObjectByType<GameManager>();
        stockPriceManager = FindAnyObjectByType<StockPriceManager>();
        rumorPhase = FindAnyObjectByType<RumorPhase>();
        if (CameraAnimation == null)
        {
            CameraAnimation = GameObject.FindObjectOfType<CameraAnimation>();
            Debug.Log("Camera Telah ditemukan");
        }

        CameraTransformPosition = new List<Vector3>()
        {
            rumorPhase.Consumen.Position,
            rumorPhase.Infrastuktur.Position,
            rumorPhase.Finance.Position,
            rumorPhase.Mining.Position,
        };
        CameraTransformRotation = new List<Vector3>()
        {
            rumorPhase.Consumen.Rotation,
            rumorPhase.Infrastuktur.Rotation,
            rumorPhase.Finance.Rotation,
            rumorPhase.Mining.Rotation,
        };
    }
    public void StartActionPhase()
    {

        CameraAnimation.ActionCamera();
        StartCoroutine(ShowTheCard());
    }

    private IEnumerator ShowTheCard()
    {
        yield return new WaitForSeconds(2f);
        cardManager.StartStockCards();
        StartDrawCardForNextPlayer();
    }

    #endregion

    #region Card Drawing
    // Memulai pengambilan kartu untuk pemain berikutnya
    void StartDrawCardForNextPlayer()
    {
        if(IsFlashBuy)
        {
            return;
        }

        if (cardManager.selectedCards.Count > 0)
        {
            Player currentPlayer = playerManager.playerList[currentPlayerIndex];
            Debug.Log("Sekarang giliran " + currentPlayer.Name + " untuk Mengambil Kartu.");

            // Mulai timer untuk pemilihan kartu
            currentTimerCoroutine = StartCoroutine(SelectCardWithTimer(currentPlayer));
        }
        else
        {
            Debug.Log("Semua kartu telah diambil.");
            Debug.Log("Action Phase Berakhir.");
            gameManager.currentGameState = GameManager.GameState.Selling;
            gameManager.StartNextPhase();
        }
    }

    // Coroutine untuk memilih kartu dengan timer
    private IEnumerator SelectCardWithTimer(Player currentPlayer)
    {
        if(IsFlashBuy)
        {
            yield break;
        }
        
        float timer = selectionTime;

        while (timer > 0)
        {
            // Update teks timer
            timerText.text = Mathf.Ceil(timer).ToString(); // Perbarui teks timer
            timer -= Time.deltaTime; // Kurangi waktu dengan waktu yang berlalu
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Waktu habis, pilih kartu secara acak
        Debug.Log(currentPlayer.Name + " waktu habis! Memilih kartu secara acak.");

        // Memilih kartu secara acak
        if (cardManager.selectedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, cardManager.selectedCards.Count);
            GameObject randomCard = cardManager.selectedCards[randomIndex];

            // Melakukan tindakan yang diperlukan pada kartu yang dipilih secara acak
            cardManager.currentActiveCard = randomCard.GetComponent<CardAnimation>(); // Asumsikan Anda memiliki komponen Card
            yield return StartCoroutine(HandleAnimationComplete(cardManager.currentActiveCard));
            RemoveCurrentCard(); // Menghapus kartu dari daftar
            HideActionButtons(); // Menyembunyikan tombol aksi
            EnableSelectedCardColliders(); // Mengaktifkan collider kartu yang dipilih
        }

        MoveToNextPlayer(); // Pindah ke pemain berikutnya
    }

    private IEnumerator HandleAnimationComplete(CardAnimation Card)
    {
        // Hapus kartu dari daftar atau lakukan tindakan lain
        Card.AnimatedToTarget();
        UpdatePlayerResources();
        yield return new WaitForSeconds(2); // Tunggu hingga animasi selesai
    }

    // Memindahkan giliran ke pemain berikutnya
    private void MoveToNextPlayer()
    {
        if(IsFlashBuy)
        {
            return;
        }

        currentPlayerIndex++;

        if (currentPlayerIndex >= playerManager.PlayerCount)
        {
            currentPlayerIndex = 0; // Kembali ke pemain pertama jika sudah mencapai akhir daftar
        }

        StartDrawCardForNextPlayer(); // Mulai giliran pemain berikutnya
    }
    #endregion

    #region Card Actions
    // Panggil fungsi ini ketika pemain telah memilih kartu
    public void OnCardActived()
    {
        StopCurrentTimer(); // Hentikan timer saat kartu diaktifkan
        StartCoroutine(ActiveCardEffect());
        RemoveCurrentCard(); // Hapus kartu yang sedang aktif
        HideActionButtons(); // Sembunyikan tombol aksi
        EnableSelectedCardColliders(); // Aktifkan collider kartu yang dipilih
        if(IsFlashBuy)
        {
            MoveToNextPlayer();
        }
    }

    // Panggil fungsi ini ketika pemain memilih untuk menyimpan kartu
    public void OnCardKeep()
    {
        StockCard card = cardManager.currentActiveCard.GetComponent<StockCard>();
        Player currentPlayer = playerManager.playerList[currentPlayerIndex];
        if(IsFlashBuy)
        {
            StopCurrentTimer();
            RemoveCurrentCard();
            HideActionButtons();
            EnableSelectedCardColliders();

            UpdatePlayerResources();
            FlasbuyRemains--;
            StartCoroutine(HandleFlashBuy());
        }else{
            if(card.Type == StockCard.StockType.TradeFee)
            {
                currentPlayer.RemoveWealth(1);
                Debug.Log("anda mendapakan 1");
            }
            StopCurrentTimer();
            RemoveCurrentCard();
            HideActionButtons();
            EnableSelectedCardColliders();

            UpdatePlayerResources();

            MoveToNextPlayer();
        }


    }

    private void UpdatePlayerResources()
    {
        if (cardManager.currentActiveCard != null)
        {
            StockCard card = cardManager.currentActiveCard.GetComponent<StockCard>();
            Player currentPlayer = playerManager.playerList[currentPlayerIndex];

            switch (card.Connected_Sectors)
            {
                case StockCard.Sector.Infrastuctur:
                    currentPlayer.AddIndustrial(1);
                    break;
                case StockCard.Sector.Finance:
                    currentPlayer.AddFinance(1);
                    break;
                case StockCard.Sector.Mining:
                    currentPlayer.AddMining(1);
                    break;
                case StockCard.Sector.Consumen:
                    currentPlayer.AddConsumen(1);
                    break;
                default:
                    // Handle unexpected sector if necessary
                    break;
            }
        }
    }

    public IEnumerator ActiveCardEffect()
    {
        if(cardManager.currentActiveCard != null)
        {
            StockCard card = cardManager.currentActiveCard.GetComponent<StockCard>();
            Player currentPlayer = playerManager.playerList[currentPlayerIndex];

             switch (card.Type)
            {
                case StockCard.StockType.FlashBuy:
                    Debug.Log("Kartu Flasbuy Di aktifkan");
                    IsFlashBuy = true;
                    FlasbuyRemains = 2;
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(HandleFlashBuy());
                    break;
                case StockCard.StockType.TradeFee:
                    HandleTradeFee(card, currentPlayer);
                    break;
                case StockCard.StockType.StockSplit:
                    StartCoroutine(HandleStockSplit(card));
                    break;
                case StockCard.StockType.InsiderTrade:
                    StartCoroutine(HandleInsiderTrade(card));
                    break;
                default:
                    break;

            }
        }
    }

    private IEnumerator HandleInsiderTrade(StockCard card)
    {
        switch(card.Connected_Sectors)
        {
            case StockCard.Sector.Infrastuctur:
                moveToSector(CameraTransformPosition[0],CameraTransformRotation[0]);
                yield return new WaitForSeconds(2f);
                AnimateCurrentCard(1);
                yield return new WaitForSeconds(5f);
                rumorPhase.MoveCardToinitInsiderTrade(1,1);
                ResetCameraTransform();
                break;
            case StockCard.Sector.Mining:
                moveToSector(CameraTransformPosition[3],CameraTransformRotation[3]);
                yield return new WaitForSeconds(2f);
                AnimateCurrentCard(3);
                yield return new WaitForSeconds(5f);
                rumorPhase.MoveCardToinitInsiderTrade(3,1);
                ResetCameraTransform();
                break;
            case StockCard.Sector.Finance:
                moveToSector(CameraTransformPosition[2],CameraTransformRotation[2]);
                yield return new WaitForSeconds(2f);
                AnimateCurrentCard(2);
                yield return new WaitForSeconds(5f);
                rumorPhase.MoveCardToinitInsiderTrade(2,1);
                ResetCameraTransform();
                break;
            case StockCard.Sector.Consumen:
                moveToSector(CameraTransformPosition[1],CameraTransformRotation[1]);
                yield return new WaitForSeconds(2f);
                AnimateCurrentCard(0);
                yield return new WaitForSeconds(5f);
                rumorPhase.MoveCardToinitInsiderTrade(0,1);
                ResetCameraTransform();
                break;
        }

        MoveToNextPlayer();
    }

    private void AnimateCurrentCard(int indexSector)
    {
        if(rumorPhase.sectors[indexSector].CurrentCard == null)
        {
            rumorPhase.SelectRandomCardInsideTrade(indexSector);
        }
        if(rumorPhase.sectors[indexSector].CurrentCard != null)
        {
            rumorPhase.MoveCardToCameraInsiderTrade(indexSector,1);
        }
    }

    private void moveToSector(Vector3 Position, Vector3 Rotation)
    {
        mainCamera.transform.DOMove(Position, 1f);
        Quaternion targetRotation = Quaternion.Euler(Rotation);
        mainCamera.transform.DORotateQuaternion(targetRotation, 1f);
    }

    private IEnumerator HandleStockSplit(StockCard card)
    {
        switch(card.Connected_Sectors)
        {
            case StockCard.Sector.Infrastuctur:
                moveToSector(CameraTransformPosition[0],CameraTransformRotation[0]);

                Sectors sectorsInfrastuctur = stockPriceManager.allSector.Infrastuctur;
                int IndexStockPriceInfrastuctur = stockPriceManager.allSector.Infrastuctur.CurrenPriceIndex;
                int SplitPriceInfrastuctur = (int)System.Math.Ceiling((double)IndexStockPriceInfrastuctur / 2);
                int fixdecreaseInfrastuctur = IndexStockPriceInfrastuctur - SplitPriceInfrastuctur;

                yield return new WaitForSeconds(2f);

                stockPriceManager.DecreaseCurrentPriceIndex(sectorsInfrastuctur, fixdecreaseInfrastuctur);
                yield return new WaitForSeconds(2f);
                ResetCameraTransform();
                break;

            case StockCard.Sector.Mining:
                moveToSector(CameraTransformPosition[3],CameraTransformRotation[3]);
                
                Sectors sectorsMining = stockPriceManager.allSector.Mining;
                int IndexStockPriceMining = stockPriceManager.allSector.Mining.CurrenPriceIndex;
                int SplitPriceMining = (int)System.Math.Ceiling((double)IndexStockPriceMining / 2);
                int fixdecreaseMining = IndexStockPriceMining - SplitPriceMining;

                yield return new WaitForSeconds(2f);
                
                stockPriceManager.DecreaseCurrentPriceIndex(sectorsMining, fixdecreaseMining);
                yield return new WaitForSeconds(2f);
                ResetCameraTransform();
                break;

            case StockCard.Sector.Finance:
                moveToSector(CameraTransformPosition[2],CameraTransformRotation[2]);

                Sectors sectorsFinance = stockPriceManager.allSector.Finance;
                int IndexStockPriceFinance = stockPriceManager.allSector.Finance.CurrenPriceIndex;
                int SplitPriceFinance = (int)System.Math.Ceiling((double)IndexStockPriceFinance / 2);
                int fixdecreaseFinance = IndexStockPriceFinance - SplitPriceFinance;

                yield return new WaitForSeconds(2f);
                
                stockPriceManager.DecreaseCurrentPriceIndex(sectorsFinance, fixdecreaseFinance);
                yield return new WaitForSeconds(2f);
                ResetCameraTransform();
                break;

            case StockCard.Sector.Consumen:
                moveToSector(CameraTransformPosition[1],CameraTransformRotation[1]);

                Sectors sectorsConsumen = stockPriceManager.allSector.Consumen;
                int IndexStockPriceConsumen = stockPriceManager.allSector.Consumen.CurrenPriceIndex;
                int SplitPriceConsumen = (int)System.Math.Ceiling((double)IndexStockPriceConsumen / 2);
                int fixdecreaseConsumen = IndexStockPriceConsumen - SplitPriceConsumen;

                yield return new WaitForSeconds(2f);
                
                stockPriceManager.DecreaseCurrentPriceIndex(sectorsConsumen, fixdecreaseConsumen);

                yield return new WaitForSeconds(2f);
                ResetCameraTransform();

                break;
        }

        MoveToNextPlayer();
    }

    private void ResetCameraTransform()
    {
        mainCamera.transform.DOMove(InitializationCameraPosition, 1f);
        Quaternion targetRotation = Quaternion.Euler(InitializationCameraRotation);
        mainCamera.transform.DORotateQuaternion(targetRotation, 1f);
    }

    private IEnumerator HandleFlashBuy()
    {
        if(FlasbuyRemains > 0 )
        {
            currentTimerCoroutine = StartCoroutine(TimeCountDownForFlashBuy());
            Debug.Log("Anda Dapat Mengambil " + FlasbuyRemains + " Kartu lagi");
        }else{
            IsFlashBuy = false;
            yield return new WaitForSeconds(0.2f);
            MoveToNextPlayer();
        }
    }

    private IEnumerator TimeCountDownForFlashBuy()
    {
        float timer = selectionTime;

        while (timer > 0)
        {
            // Update teks timer
            timerText.text = Mathf.Ceil(timer).ToString(); // Perbarui teks timer
            timer -= Time.deltaTime; // Kurangi waktu dengan waktu yang berlalu
            yield return null; // Tunggu hingga frame berikutnya
        }

        // Waktu habis, pilih kartu secara acak
        Debug.Log(" waktu habis! Memilih kartu secara acak.");

        // Memilih kartu secara acak
        if (cardManager.selectedCards.Count > 0)
        {
            int randomIndex = Random.Range(0, cardManager.selectedCards.Count);
            GameObject randomCard = cardManager.selectedCards[randomIndex];

            // Melakukan tindakan yang diperlukan pada kartu yang dipilih secara acak
            cardManager.currentActiveCard = randomCard.GetComponent<CardAnimation>(); // Asumsikan Anda memiliki komponen Card
            yield return StartCoroutine(HandleAnimationComplete(cardManager.currentActiveCard));
            RemoveCurrentCard(); // Menghapus kartu dari daftar
            HideActionButtons(); // Menyembunyikan tombol aksi
            EnableSelectedCardColliders(); // Mengaktifkan collider kartu yang dipilih
        }
        if(FlasbuyRemains > 0)
        {
            FlasbuyRemains--;
            StartCoroutine(HandleFlashBuy());
        }else{
            MoveToNextPlayer();
        }

    }

    private void HandleTradeFee(StockCard card, Player currentPlayer)
    {
        switch (card.Connected_Sectors)
        {
            case StockCard.Sector.Infrastuctur:
                int IndexStockPriceInfrastuctur = stockPriceManager.allSector.Infrastuctur.CurrenPriceIndex;
                int priceInfrastuctur = stockPriceManager.allSector.Infrastuctur.Sector[IndexStockPriceInfrastuctur].GetComponent<StockPrice>().value;
                priceInfrastuctur--;
                currentPlayer.AddWealth(priceInfrastuctur);
                Debug.Log("anda berhasil menjual saham dan mendapatkan : " + priceInfrastuctur);
                break;
            case StockCard.Sector.Consumen:
                int IndexStockPriceConsumen = stockPriceManager.allSector.Consumen.CurrenPriceIndex;
                int priceConsumen = stockPriceManager.allSector.Infrastuctur.Sector[IndexStockPriceConsumen].GetComponent<StockPrice>().value;
                priceConsumen--;
                currentPlayer.AddWealth(priceConsumen);
                Debug.Log("anda berhasil menjual saham dan mendapatkan : " + priceConsumen);
                break;
            case StockCard.Sector.Mining:
                int IndexStockPriceMining = stockPriceManager.allSector.Mining.CurrenPriceIndex;
                int priceMining = stockPriceManager.allSector.Infrastuctur.Sector[IndexStockPriceMining].GetComponent<StockPrice>().value;
                priceMining--;
                currentPlayer.AddWealth(priceMining);
                Debug.Log("anda berhasil menjual saham dan mendapatkan : " + priceMining);
                break;
            case StockCard.Sector.Finance:
                int IndexStockPriceFinance = stockPriceManager.allSector.Finance.CurrenPriceIndex;
                int priceFinance = stockPriceManager.allSector.Infrastuctur.Sector[IndexStockPriceFinance].GetComponent<StockPrice>().value;
                priceFinance--;
                currentPlayer.AddWealth(priceFinance);
                Debug.Log("anda berhasil menjual saham dan mendapatkan : " + priceFinance);
                break;
            default:
                break;
        }
        MoveToNextPlayer();
    }
    #endregion

    #region Timer and Card Management
    // Menghentikan timer saat ini
    private void StopCurrentTimer()
    {
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine); // Hentikan coroutine timer
            currentTimerCoroutine = null; // Reset coroutine
        }
    }

    // Menghapus kartu yang sedang aktif dari daftar
    private void RemoveCurrentCard()
    {
        cardManager.selectedCards.Remove(cardManager.currentActiveCard.gameObject); // Hapus kartu dari daftar
        Destroy(cardManager.currentActiveCard.gameObject); // Hapus objek kartu dari scene
    }

    // Menyembunyikan tombol aksi
    private void HideActionButtons()
    {
        cardManager.HideButton(ActiveButton); // Sembunyikan tombol aktif
        cardManager.HideButton(KeepButton); // Sembunyikan tombol simpan
    }

    // Mengaktifkan collider untuk kartu yang dipilih
    private void EnableSelectedCardColliders()
    {
        foreach (GameObject card in cardManager.selectedCards)
        {
            Collider collider = card.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = true; // Aktifkan collider jika ada
            }
        }
    }
    #endregion
}