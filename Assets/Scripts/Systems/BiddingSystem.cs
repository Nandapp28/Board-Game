using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Untuk menggunakan LINQ
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BiddingSystem : MonoBehaviour
{
    [Header("UI Setinggs")]
    public GameObject Players;
    public DiceRoll dice1, dice2;
    public FaceDetector faceDetector;
    public Transform Camera;

    [Header("Bidding Setinggs")]
    public List<Player> PlayerList = new List<Player>();
    public event Action OnBiddingCompleted;

    [Header("Dice")]
    [SerializeField] public Vector3 Dice1OffsetOnCamera = new Vector3(0, 0, 2);
    [SerializeField] public Vector3 Dice2OffsetOnCamera = new Vector3(0, 0, 2);
    [SerializeField] public Vector3 manualRotationDice1;
    [SerializeField] public Vector3 manualRotationDice2;
    [SerializeField] public float Duration;
    [SerializeField] private Ease Ease;
    

    public int CurrentPlayerIndex = 0;
    public bool isRollingDice = false;
    public bool isWaitingForDiceResult = false;

    public CameraAnimation cameraAnimation;


    public void DiceOnCamera()
    {
        dice1.DiceToCamera(Camera,Dice1OffsetOnCamera,manualRotationDice1,Duration,Ease);
        dice2.DiceToCamera(Camera,Dice2OffsetOnCamera,manualRotationDice2,Duration,Ease);
    }

    void Update()
    {
        // Mencegah lemparan ganda ketika dadu sedang diroll
        if (isRollingDice && Input.GetKeyDown(KeyCode.Space) && !isWaitingForDiceResult)
        {
            RollDice();
        }

        // Proses hasil dadu hanya ketika dadu benar-benar sudah berhenti
        if (isWaitingForDiceResult && faceDetector.AreBothDiceStopped())
        {
            ProcessDiceResults();
        }
    }

    // Fungsi untuk memulai bidding
    public void StartBidding()
    {
        Debug.Log("Bidding Phase Dimulai");

        cameraAnimation = FindObjectOfType<CameraAnimation>();

        cameraAnimation.BiddingCamera();
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
            Debug.Log("Sekarang giliran " + currentPlayer.Name + " untuk melempar dadu.");

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
        // Mengunci input lemparan sampai hasil dadu diproses
        if (!isWaitingForDiceResult)
        {
            Player currentPlayer = PlayerList[CurrentPlayerIndex];
            Debug.Log(currentPlayer.Name + " sedang melempar dadu...");

            dice1.RollDice();
            dice2.RollDice();

            isRollingDice = false;
            isWaitingForDiceResult = true;
        }
    }

    // Fungsi untuk memproses hasil lemparan dadu
    void ProcessDiceResults()
    {
        Player currentPlayer = PlayerList[CurrentPlayerIndex];
        
        // Pastikan hasil hanya diproses setelah kedua dadu benar-benar berhenti
        if (faceDetector.AreBothDiceStopped())
        {
            var (diceResult1, diceResult2) = faceDetector.GetDiceResults();
            currentPlayer.RollDice(diceResult1, diceResult2); // Simpan hasil ke player

            Debug.Log(currentPlayer.Name + " mendapat nilai dadu: " + diceResult1 + " dan " + diceResult2);
            Debug.Log("Total nilai: " + currentPlayer.TotalScore);

            faceDetector.ResetDiceDetection(); // Reset deteksi dadu sebelum giliran berikutnya
            CurrentPlayerIndex++;
            isWaitingForDiceResult = false; // Reset untuk giliran berikutnya
            
            DiceOnCamera();

            StartDiceRollForNextPlayer();

        }
    }


    // Cek apakah ada nilai dadu yang sama di antara para pemain dan beri prioritas berdasarkan urutan pemain asli
    void HandleDuplicateDiceResults()
    {
        var groupedResults = PlayerList.GroupBy(p => p.TotalScore).Where(g => g.Count() > 1).ToList();

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
                    playersWithSameResult[i].TotalScore += increment;
                    increment += 0.01f; // Tambahkan nilai kecil untuk membedakan urutan
                    Debug.Log($"{playersWithSameResult[i].Name} diberi prioritas dengan nilai baru: {playersWithSameResult[i].TotalScore}");
                }
            }
        }

        Debug.Log("Tidak ada nilai dadu yang sama atau semua telah diselesaikan. Mengurutkan pemain berdasarkan nilai dadu.");
        SortPlayersByDiceResult(); // Mengurutkan pemain berdasarkan nilai
    }

    // Fungsi untuk mengurutkan pemain berdasarkan nilai dadu
    void SortPlayersByDiceResult()
    {
        StartCoroutine(SortPlayersByDiceResultCoroutine());
    }
    private IEnumerator SortPlayersByDiceResultCoroutine()
    {
        // Urutkan pemain berdasarkan total nilai dari yang tertinggi ke terendah 
        PlayerList = PlayerList.OrderByDescending(p => p.TotalScore).ToList();

        // Tetapkan urutan main berdasarkan urutan yang baru
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].SetPlayOrder(i + 1); // Urutan dimulai dari 1
            Debug.Log(PlayerList[i].Name + " berada di urutan ke-" + (i + 1) + " dengan total nilai: " + PlayerList[i].TotalScore);
        }

        cameraAnimation.ResetCamera();

        yield return new WaitForSeconds(3);
        
        OnBiddingCompleted?.Invoke();
    }


}