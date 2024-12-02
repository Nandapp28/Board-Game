using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BiddingPhase : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;
    [Header("UI Settings")]
    public RollDice Dice;
    public GameObject Player;
    public List<GameObject> PlayerList;

    private int currentPlayerIndex = 0;
    private bool isRollingDice = false;
    private bool isWaitingForDiceResult = false;
    private CameraAnimation Camera;

    #region Unity Lifecycle

    public void StartBiddingPhase() 
    {
        CollectPlayers();
        ShufflePlayerList();
        StartDiceRollForNextPlayer();
        
        if (Camera == null){
            Camera = GameObject.FindObjectOfType<CameraAnimation>();
            Debug.Log("Camera Telah ditemukan");
        }

        Camera.BiddingCamera();

    }

    private void Update()
    {
        HandleDiceRolling();
        HandleBothDiceStopped();
        HandleDiceToCamera();
    }
    #endregion
    
    #region Handle For Update

    private void HandleDiceRolling()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !Dice.isRolling && !Dice.isMovingToCamera && isRollingDice && !isWaitingForDiceResult)
        {
            RollDiceForCurrentPlayer();
        }
    }

    private void HandleBothDiceStopped()
    {
        if (Dice.isRolling && Dice.diceRigidbody1.IsSleeping() && Dice.diceRigidbody2.IsSleeping() && isWaitingForDiceResult)
        {
            StopDiceRolling();
        }
    }

    private void HandleDiceToCamera()
    {
        if (Dice.isMovingToCamera)
        {
            MoveDiceToCamera();
        }
    }
    #endregion

    private void CollectPlayers()
    {
        if (PlayerList.Count == 0 && Player != null)
        {
            PlayerList.Clear();
            foreach (Transform child in Player.transform)
            {
                Player playerComponent = child.GetComponent<Player>();
                if (playerComponent != null)
                {
                    PlayerList.Add(playerComponent.gameObject);
                }
                else
                {
                    Debug.LogWarning("Tidak ditemukan komponen Player di: " + child.gameObject.name);
                }
            }
            Debug.Log("PlayerList berhasil diisi dengan " + PlayerList.Count + " pemain.");
        }
        else if (Player == null)
        {
            Debug.LogError("GameObject 'Player' belum ditetapkan.");
        }
    }

    private void ShufflePlayerList()
    {
        for (int i = PlayerList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (PlayerList[i], PlayerList[randomIndex]) = (PlayerList[randomIndex], PlayerList[i]);
        }
        Debug.Log("PlayerList telah diacak.");
    }

    private void StartDiceRollForNextPlayer()
    {
        if (currentPlayerIndex < PlayerList.Count)
        {
            Player currentPlayer = PlayerList[currentPlayerIndex].GetComponent<Player>();
            Debug.Log("Sekarang giliran " + currentPlayer.Name + " untuk melempar dadu.");
            isRollingDice = true;
            isWaitingForDiceResult = false;
        }
        else
        {
            Debug.Log("Semua pemain telah melempar dadu.");
            currentPlayerIndex = 0;
            HandleDuplicateDiceResults();
        }
    }

    private void RollDiceForCurrentPlayer()
    {
        if (!isWaitingForDiceResult)
        {
            Player currentPlayer = PlayerList[currentPlayerIndex].GetComponent<Player>();
            Debug.Log(currentPlayer.Name + " sedang melempar dadu...");
            Dice.RollTheDice();
            isRollingDice = false;
            isWaitingForDiceResult = true;
        }
    }

    private void StopDiceRolling()
    {
        Dice.isRolling = false;
        SetDiceKinematic(true);
        Dice.CalculateDiceValues();
        HandleDiceProcessResult();
        CaptureDiceRotation();
    }

    private void SetDiceKinematic(bool isKinematic)
    {
        Dice.diceRigidbody1.isKinematic = isKinematic;
        Dice.diceRigidbody2.isKinematic = isKinematic;
        Dice.isKinematic = isKinematic;
    }

    private void CaptureDiceRotation()
    {
        if (!Dice.hasRotationsCaptured)
        {
            Dice.currentRotation1 = Dice.diceRigidbody1.transform.rotation;
            Dice.currentRotation2 = Dice.diceRigidbody2.transform.rotation;
            Dice.hasRotationsCaptured = true;
            Dice.isMovingToCamera = true;
            Debug.Log("Mulai memindahkan dadu ke kamera!");
        }
    }

    private void MoveDiceToCamera()
    {
        Dice.MoveDiceToCamera(Dice.diceRigidbody1.gameObject, Dice.offsetFromCamera1, Dice.offsetRotationFromCamera1, Dice.currentRotation1);
        Dice .MoveDiceToCamera(Dice.diceRigidbody2.gameObject, Dice.offsetFromCamera2, Dice.offsetRotationFromCamera2, Dice.currentRotation2);

        if (Dice.CheckIfDiceReachedCamera(Dice.diceRigidbody1.gameObject, Dice.offsetFromCamera1) && 
            Dice.CheckIfDiceReachedCamera(Dice.diceRigidbody2.gameObject, Dice.offsetFromCamera2))
        {
            Dice.timeSinceRolling += Time.deltaTime;
            if (Dice.timeSinceRolling >= 1.5f)
            {
                ResetDicePositions();
                Dice.isMovingToCamera = false;
                Dice.timeSinceRolling = 0f;
            }
        }
    }

    private void ResetDicePositions()
    {
        Dice.ResetDicePosition(Dice.diceRigidbody1.gameObject, Dice.initialPosition1, Dice.initialRotation1, Dice.resetPosition);
        Dice.ResetDicePosition(Dice.diceRigidbody2.gameObject, Dice.initialPosition2, Dice.initialRotation2, Dice.resetPosition);
    }

    private void HandleDiceProcessResult()
    {
        Player currentPlayer = PlayerList[currentPlayerIndex].GetComponent<Player>();
        currentPlayer.RollDice(Dice.Dice1, Dice.Dice2);
        Debug.Log(currentPlayer.Name + " mendapat nilai dadu: " + Dice.Dice1 + " dan " + Dice.Dice2);
        Debug.Log("Total nilai: " + currentPlayer.TotalScore);
        Dice.ResetDiceResult();
        currentPlayerIndex++;
        isWaitingForDiceResult = false;
        StartDiceRollForNextPlayer();
    }

    private void HandleDuplicateDiceResults()
    {
        var groupedResults = PlayerList.GroupBy(p => p.GetComponent<Player>().TotalScore).Where(g => g.Count() > 1).ToList();

        if (groupedResults.Count > 0)
        {
            Debug.Log("Ada pemain dengan nilai dadu yang sama. Menentukan prioritas berdasarkan urutan pemain asli.");
            foreach (var group in groupedResults)
            {
                List<GameObject> playersWithSameResult = group.ToList();
                playersWithSameResult = playersWithSameResult.OrderBy(p => PlayerList.IndexOf(p)).ToList();
                AssignPriorityToPlayers(playersWithSameResult);
            }
        }

        Debug.Log("Tidak ada nilai dadu yang sama atau semua telah diselesaikan. Mengurutkan pemain berdasarkan nilai dadu.");
        SortPlayersByDiceResult();
    }

    private void AssignPriorityToPlayers(List<GameObject> playersWithSameResult)
    {
        float increment = 0.01f;
        for (int i = 0; i < playersWithSameResult.Count; i++)
        {
            playersWithSameResult[i].GetComponent<Player>().TotalScore += increment;
            increment += 0.01f;
            Debug.Log($"{playersWithSameResult[i].GetComponent<Player>().Name} diberi prioritas dengan nilai baru: {playersWithSameResult[i].GetComponent<Player>().TotalScore}");
        }
    }

    private void SortPlayersByDiceResult()
    {
        StartCoroutine(SortPlayersByDiceResultCoroutine());
    }

    private IEnumerator SortPlayersByDiceResultCoroutine()
    {
        PlayerList = PlayerList.OrderByDescending(p => p.GetComponent<Player>().TotalScore).ToList();
        for (int i = 0; i < PlayerList.Count; i++)
        {
            PlayerList[i].GetComponent<Player>().SetPlayOrder(i + 1);
            Debug.Log(PlayerList[i].GetComponent<Player>().Name + " berada di urutan ke-" + (i + 1) + " dengan total nilai: " + PlayerList[i].GetComponent<Player>().TotalScore);
        }

        gameManager.currentGameState = GameManager.GameState.Action;
        yield return new WaitForSeconds(2);
        gameManager.StartNextPhase();
    }
}