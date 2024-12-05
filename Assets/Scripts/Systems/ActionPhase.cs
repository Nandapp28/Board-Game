using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPhase : MonoBehaviour
{
    [SerializeField] private GameObject players; // GameObject yang berisi semua player
    public List<Player> playerList = new List<Player>(); // List untuk menyimpan komponen Player
    public CardManager cardManager; // Komponen CardManager
    public Button ActiveButton; // Tombol untuk aksi aktif
    public Button KeepButton; // Tombol untuk menyimpan kartu
    public const float selectionTime = 20f; // Waktu pemilihan dalam detik

    [SerializeField] private TextMeshProUGUI timerText; // Referensi ke komponen TextMeshPro untuk timer

    private int currentPlayerIndex = 0; // Indeks pemain saat ini
    private Coroutine currentTimerCoroutine; // Menyimpan coroutine timer saat ini
    private CameraAnimation Camera;
    private GameManager gameManager;


    // Memulai fase aksi dengan mengumpulkan pemain dan memulai pengambilan kartu
    public void StartActionPhase()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        if (Camera == null){
            Camera = GameObject.FindObjectOfType<CameraAnimation>();
            Debug.Log("Camera Telah ditemukan");
        }
        
        CollectPlayers();
        Camera.ActionCamera();
        StartCoroutine(ShowTheCard());

    }

    private IEnumerator ShowTheCard(){
        yield return new WaitForSeconds(2f);
        cardManager.StartStockCards();
        StartDrawCardForNextPlayer();
    }

    // Mengumpulkan semua pemain dari GameObject yang ditetapkan
    void CollectPlayers()
    {
        if (players == null)
        {
            Debug.LogError("GameObject players tidak ditetapkan!");
            return;
        }

        playerList.Clear();

        if (players.transform.childCount == 0)
        {
            Debug.LogWarning("Tidak ada player ditemukan di dalam GameObject players!");
            return;
        }

        foreach (Transform child in players.transform)
        {
            Player playerComponent = child.GetComponent<Player>();
            if (playerComponent != null)
            {
                playerList.Add(playerComponent); // Tambahkan komponen Player ke daftar
            }
            else
            {
                Debug.LogWarning("Child " + child.name + " tidak memiliki komponen Player.");
            }
        }

        Debug.Log("Jumlah player yang dikumpulkan: " + playerList.Count);
        SortPlayersByPlayOrder(); // Urutkan pemain berdasarkan urutan bermain
        DisplayPlayerNames(); // Tampilkan nama pemain
    }

    // Mengurutkan pemain berdasarkan urutan bermain
    private void SortPlayersByPlayOrder()
    {
        playerList.Sort((a, b) => a.playOrder.CompareTo(b.playOrder));
        Debug.Log("Players sorted by play order.");
    }
    
    // Menampilkan nama pemain di log
    private void DisplayPlayerNames()
    {
        foreach (var player in playerList)
        {
            Debug.Log("Player ditemukan: " + player.Name);
        }
    }

    // Memulai pengambilan kartu untuk pemain berikutnya
    void StartDrawCardForNextPlayer()
    {
        if (cardManager.selectedCards.Count > 0)
        {
            Player currentPlayer = playerList[currentPlayerIndex];
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
        currentPlayerIndex++;

        if (currentPlayerIndex >= playerList.Count ){
            currentPlayerIndex = 0; // Kembali ke pemain pertama jika sudah mencapai akhir daftar
        }

        StartDrawCardForNextPlayer(); // Mulai giliran pemain berikutnya
    }

    // Panggil fungsi ini ketika pemain telah memilih kartu
    public void OnCardActived()
    {
        StopCurrentTimer(); // Hentikan timer saat kartu diaktifkan
        RemoveCurrentCard(); // Hapus kartu yang sedang aktif
        HideActionButtons(); // Sembunyikan tombol aksi
        EnableSelectedCardColliders(); // Aktifkan collider kartu yang dipilih
        MoveToNextPlayer(); // Pindah ke pemain berikutnya
    }

    // Panggil fungsi ini ketika pemain memilih untuk menyimpan kartu
    public void OnCardKeep()
    {
        StopCurrentTimer();
        RemoveCurrentCard();
        HideActionButtons();
        EnableSelectedCardColliders();

        UpdatePlayerResources();

        MoveToNextPlayer();
    }

    private void UpdatePlayerResources()
    {

        if (cardManager.currentActiveCard != null)
        {
            StockCard card = cardManager.currentActiveCard.GetComponent<StockCard>();
            Player currentPlayer = playerList[currentPlayerIndex];

            switch (card.Connected_Sectors)
            {
                case StockCard.Sector.Infrastukur:
                    currentPlayer.AddIndustrial(1);
                    break;
                case StockCard.Sector.Keuangan:
                    currentPlayer.AddFinance(1);
                    break;
                case StockCard.Sector.Mining:
                    currentPlayer.AddMining(1);
                    break;
                case StockCard.Sector.Consumer:
                    currentPlayer.AddConsumen(1);
                    break;
                default:
                    // Handle unexpected sector if necessary
                    break;
            }

        }
    }

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
}