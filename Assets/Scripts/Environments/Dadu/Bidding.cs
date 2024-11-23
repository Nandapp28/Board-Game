using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BiddingPhase : MonoBehaviour
{
    [Header("UI Settings")]
    public RollDice Dice;
    public GameObject Player;
    public List<GameObject> PlayerList;


    private int CurrentPlayerIndex = 0;
    private bool isRollingDice = false;
    private bool isWaitingForDiceResult = false;

    #region Unity Lifecycle
    private void Start() {
        CollectingPlayer();
        ShufflePlayerList();
        StartDiceRollForNextPlayer();
    }

    private void Update()
    {
        HandleDiceRolling();
        // HandleDiceKinematic();
        HandleBothDiceStopped();
        HandleDiceToCamera();
    }
    #endregion
    
    #region Handle For Update


    private void HandleDiceRolling()
    {
        // Deteksi input untuk melempar dadu
        if (Input.GetKeyDown(KeyCode.Space) && !Dice.isRolling && !Dice.isMovingToCamera && isRollingDice && !isWaitingForDiceResult)
        {
            HandleRollDice();
        }
    }

    private void  HandleDiceKinematic()
    {
        // Deteksi input untuk mengubah status kinematic
        if (Input.GetKeyDown(KeyCode.Space) && Dice.isKinematic)
        {
            // Kembalikan status kinematic menjadi false
            Dice.diceRigidbody1.isKinematic = false;
            Dice.diceRigidbody2.isKinematic = false;
            Dice.isKinematic = false;
        }
    }

    private void HandleBothDiceStopped()
    {
        // Periksa apakah kedua dadu telah berhenti
        if (Dice.isRolling && Dice.diceRigidbody1.IsSleeping() && Dice.diceRigidbody2.IsSleeping() && isWaitingForDiceResult)
        {
            Dice.isRolling = false;

            // Ubah isKinematic menjadi true untuk menghentikan interaksi fisika
            Dice.diceRigidbody1.isKinematic = true;
            Dice.diceRigidbody2.isKinematic = true;
            Dice.isKinematic = true; // Menandakan bahwa isKinematic aktif

            Dice.CalculateDiceValues();
            HandleDiceProsesResult();

            // Ambil rotasi hanya jika belum diambil
            if (!Dice.hasRotationsCaptured)
            {
                Dice.currentRotation1 = Dice.diceRigidbody1.transform.rotation;
                Dice.currentRotation2 = Dice.diceRigidbody2.transform.rotation;

                // Tandai bahwa rotasi telah diambil
                Dice.hasRotationsCaptured = true;
                Dice.isMovingToCamera = true;
                Debug.Log("Mulai memindahkan dadu ke kamera!");
            }
        }
    }

    private void HandleDiceToCamera()
    {
        // Gerakkan dadu ke depan kamera
        if (Dice.isMovingToCamera)
        {
            Dice.MoveDiceToCamera(Dice.diceRigidbody1.gameObject ,Dice.offsetFromCamera1, Dice.offsetRotationFromCamera1, Dice.currentRotation1);
            Dice.MoveDiceToCamera(Dice.diceRigidbody2.gameObject,Dice.offsetFromCamera2, Dice.offsetRotationFromCamera2, Dice.currentRotation2);

            // Periksa jika dadu sudah sampai di posisi target
            if (Dice.CheckIfDiceReachedCamera(Dice.diceRigidbody1.gameObject ,Dice.offsetFromCamera1) && Dice.CheckIfDiceReachedCamera(Dice.diceRigidbody2.gameObject,Dice.offsetFromCamera2))
            {
                // Mulai timer setelah 5 detik
                Dice.timeSinceRolling += Time.deltaTime;
                if (Dice.timeSinceRolling >= 3f)
                {
                    // Kembalikan dadu ke posisi awal setelah 5 detik
                    Dice.ResetDicePosition(Dice.diceRigidbody1.gameObject , Dice.initialPosition1,Dice.initialRotation1 , Dice.resetPosition);
                    Dice.ResetDicePosition(Dice.diceRigidbody2.gameObject, Dice.initialPosition2,Dice.initialRotation2 , Dice.resetPosition);
                    
                    // Matikan gerakan ke kamera
                    Dice.isMovingToCamera = false;
                    
                    // Reset waktu
                    Dice.timeSinceRolling = 0f;
                }
            }
        }
    }
    #endregion

    void CollectingPlayer()
    {
        // Hanya tambahkan pemain jika daftar kosong
        if (PlayerList.Count == 0)
        {
            if (Player != null)
            {
                // Pastikan daftar dikosongkan sebelum menambahkan anak-anak untuk menghindari duplikasi
                PlayerList.Clear();

                // Tambahkan setiap anak dari Player ke PlayerList
                foreach (Transform child in Player.transform)
                {
                    Player playerComponent = child.GetComponent<Player>();

                    if(playerComponent != null)
                    {
                        PlayerList.Add(playerComponent.gameObject);  // Tambahkan GameObject ke daftar
                    }else{
                         Debug.Log("Tidak ditemukan komponen Player di: " + child.gameObject.name);
                    }
                }

                // Pernyataan debugging untuk memastikan pemain ditambahkan dengan benar
                Debug.Log("PlayerList berhasil diisi dengan " + PlayerList.Count + " pemain.");
            }
            else
            {
                Debug.LogError("GameObject 'Player' belum ditetapkan.");
            }
        }
    }

    void ShufflePlayerList()
    {
        // Algoritma Fisher-Yates shuffle
        for (int i = PlayerList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);  // Dapatkan indeks acak antara 0 dan i
            GameObject temp = PlayerList[i];
            PlayerList[i] = PlayerList[randomIndex];
            PlayerList[randomIndex] = temp;
        }

        // Pernyataan debugging untuk mengonfirmasi daftar telah diacak
        Debug.Log("PlayerList telah diacak.");
    }

    void StartDiceRollForNextPlayer()
    {
        if (CurrentPlayerIndex < PlayerList.Count)
        {
            Player currentPlayer = PlayerList[CurrentPlayerIndex].GetComponent<Player>();
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
    
    void HandleRollDice()
    {
        // Mengunci input lemparan sampai hasil dadu diproses
        if (!isWaitingForDiceResult)
        {
            Player currentPlayer = PlayerList[CurrentPlayerIndex].GetComponent<Player>();
            Debug.Log(currentPlayer.Name + " sedang melempar dadu...");

            Dice.RollTheDice();

            isRollingDice = false;
            isWaitingForDiceResult = true;
        }
    }

    void HandleDiceProsesResult()
    {
        Player currentPlayer = PlayerList[CurrentPlayerIndex].GetComponent<Player>();
        currentPlayer.RollDice(Dice.Dice1, Dice.Dice2); // Simpan hasil ke player

        Debug.Log(currentPlayer.Name + " mendapat nilai dadu: " + Dice.Dice1 + " dan " + Dice.Dice2);
        Debug.Log("Total nilai: " + currentPlayer.TotalScore);

        Dice.ResetDiceResult();

        CurrentPlayerIndex++;
        isWaitingForDiceResult = false; // Reset untuk giliran berikutnya

        StartDiceRollForNextPlayer();
    }

        void HandleDuplicateDiceResults()
    {
        var groupedResults = PlayerList.GroupBy(p => p.GetComponent<Player>().TotalScore).Where(g => g.Count() > 1).ToList();

        if (groupedResults.Count > 0)
        {
            Debug.Log("Ada pemain dengan nilai dadu yang sama. Menentukan prioritas berdasarkan urutan pemain asli.");

            foreach (var group in groupedResults)
            {
                List<GameObject> playersWithSameResult = group.ToList();

                // Urutkan berdasarkan urutan asli mereka di dalam daftar pemain
                playersWithSameResult = playersWithSameResult.OrderBy(p => PlayerList.IndexOf(p)).ToList();

                float increment = 0.01f; // Nilai tambahan kecil untuk pemain berdasarkan prioritas
                for (int i = 0; i < playersWithSameResult.Count; i++)
                {
                    playersWithSameResult[i].GetComponent<Player>().TotalScore += increment;
                    increment += 0.01f; // Tambahkan nilai kecil untuk membedakan urutan
                    Debug.Log($"{playersWithSameResult[i].GetComponent<Player>().Name} diberi prioritas dengan nilai baru: {playersWithSameResult[i].GetComponent<Player>().TotalScore}");
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
        PlayerList = PlayerList.OrderByDescending(p => p.GetComponent<Player>().TotalScore).ToList();

        // Tetapkan urutan main berdasarkan urutan yang baru
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].GetComponent<Player>().SetPlayOrder(i + 1); // Urutan dimulai dari 1
            Debug.Log(PlayerList[i].GetComponent<Player>().Name + " berada di urutan ke-" + (i + 1) + " dengan total nilai: " + PlayerList[i].GetComponent<Player>().TotalScore);
        }

        yield return new WaitForSeconds(1);
        
    }
}
