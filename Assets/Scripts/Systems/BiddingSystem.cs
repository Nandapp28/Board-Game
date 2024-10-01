using System;
using System.Collections.Generic;
using System.Linq; // Untuk menggunakan LINQ
using UnityEngine;
using Random = UnityEngine.Random;

public class BiddingSystem : MonoBehaviour
{
    public GameObject Players;
    public List<Player> PlayerList = new List<Player>();
    public DiceRoll dice1, dice2;
    public FaceDetector faceDetector;
    private int CurrentPlayerIndex = 0;
    private bool isRollingDice = false;
    private bool isWaitingForDiceResult = false;
    public event Action OnBiddingCompleted;

    void Update()
    {
        if (isRollingDice && Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }

        if (isWaitingForDiceResult && faceDetector.AreBothDiceStopped())
        {
            ProcessDiceResults();
        }
    }

    // Fungsi untuk memulai bidding
    public void StartBidding()
    {
        Debug.Log("Bidding Phase Dimulai");
        CollectingPlayer();

        if (PlayerList != null && PlayerList.Count > 0)
        {
            Debug.Log("Ada " + PlayerList.Count + " Player");

            ShufflePlayers();
            StartDiceRollForNextPlayer();
        }
        else
        {
            Debug.Log("Tidak ada Player");
        }
    }

    // Mengambil player yang ada
    void CollectingPlayer()
    {
        if (PlayerList.Count == 0)
        {
            if (Players != null)
            {
                foreach (Transform child in Players.transform)
                {
                    Player playerComponent = child.GetComponent<Player>();

                    if (playerComponent != null)
                    {
                        PlayerList.Add(playerComponent);
                    }
                    else
                    {
                        Debug.Log("Tidak ditemukan komponen Player di: " + child.gameObject.name);
                    }
                }
            }
            else
            {
                Debug.LogError("Players GameObject belum di-assign!");
            }
        }
    }

    // Fungsi untuk mengacak urutan pemain
    void ShufflePlayers()
    {
        for (int i = 0; i < PlayerList.Count; i++)
        {
            Player temp = PlayerList[i];
            int randomIndex = Random.Range(i, PlayerList.Count);
            PlayerList[i] = PlayerList[randomIndex];
            PlayerList[randomIndex] = temp;
        }

        Debug.Log("Urutan pemain telah diacak.");
    }

    // Memulai lemparan dadu untuk pemain berikutnya
    void StartDiceRollForNextPlayer()
    {
        if (CurrentPlayerIndex < PlayerList.Count)
        {
            Player currentPlayer = PlayerList[CurrentPlayerIndex];
            Debug.Log("Sekarang giliran " + currentPlayer.nama + " untuk melempar dadu.");

            isRollingDice = true;
            isWaitingForDiceResult = false;
        }
        else
        {
            Debug.Log("Semua pemain telah melempar dadu.");
            CurrentPlayerIndex = 0;
            HandleDuplicateDiceResults(); // Cek apakah ada pemain yang mendapatkan hasil yang sama
        }
    }

    // Fungsi untuk melempar dadu
    void RollDice()
    {
        Player currentPlayer = PlayerList[CurrentPlayerIndex];
        Debug.Log(currentPlayer.nama + " sedang melempar dadu...");

        dice1.RollDice();
        dice2.RollDice();

        isRollingDice = false;
        isWaitingForDiceResult = true;
    }

    // Fungsi untuk memproses hasil lemparan dadu
    void ProcessDiceResults()
    {
        Player currentPlayer = PlayerList[CurrentPlayerIndex];
        var (diceResult1, diceResult2) = faceDetector.GetDiceResults();

        currentPlayer.RollDice(diceResult1, diceResult2); // Simpan hasil ke player
        Debug.Log(currentPlayer.nama + " mendapat nilai dadu: " + diceResult1 + " dan " + diceResult2);
        Debug.Log("Total nilai: " + currentPlayer.totalNilai);

        faceDetector.ResetDiceDetection(); // Reset deteksi dadu sebelum lemparan berikutnya
        CurrentPlayerIndex++;
        isWaitingForDiceResult = false;

        StartDiceRollForNextPlayer();
    }

    // Cek apakah ada nilai dadu yang sama di antara para pemain dan beri prioritas berdasarkan urutan pemain asli
    void HandleDuplicateDiceResults()
    {
        var groupedResults = PlayerList.GroupBy(p => p.totalNilai).Where(g => g.Count() > 1).ToList();

        if (groupedResults.Count > 0)
        {
            Debug.Log("Ada pemain dengan nilai dadu yang sama. Menentukan prioritas berdasarkan urutan pemain asli.");

            foreach (var group in groupedResults)
            {
                List<Player> playersWithSameResult = group.ToList();

                // Urutkan berdasarkan urutan asli mereka di dalam daftar pemain
                playersWithSameResult = playersWithSameResult.OrderBy(p => PlayerList.IndexOf(p)).ToList();

                float increment = 0.01f; // Nilai tambahan kecil untuk pemain berdasarkan prioritas
                for (int i = 0; i < playersWithSameResult.Count; i++)
                {
                    playersWithSameResult[i].totalNilai += increment;
                    increment += 0.01f; // Tambahkan nilai kecil untuk membedakan urutan
                    Debug.Log($"{playersWithSameResult[i].nama} diberi prioritas dengan nilai baru: {playersWithSameResult[i].totalNilai}");
                }
            }
        }

        Debug.Log("Tidak ada nilai dadu yang sama atau semua telah diselesaikan. Mengurutkan pemain berdasarkan nilai dadu.");
        SortPlayersByDiceResult(); // Mengurutkan pemain berdasarkan nilai
    }

    // Fungsi untuk mengurutkan pemain berdasarkan nilai dadu
    void SortPlayersByDiceResult()
    {
        // Urutkan pemain berdasarkan total nilai dari yang tertinggi ke terendah 
        PlayerList = PlayerList.OrderByDescending(p => p.totalNilai).ToList();

        // Tetapkan urutan main berdasarkan urutan yang baru
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].SetUrutan(i + 1); // Urutan dimulai dari 1
            Debug.Log(PlayerList[i].nama + " berada di urutan ke-" + (i + 1) + " dengan total nilai: " + PlayerList[i].totalNilai);
        }
        OnBiddingCompleted?.Invoke();
    }
}