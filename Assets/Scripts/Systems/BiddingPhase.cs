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
    private PlayerManager playerManager;

    private int currentPlayerIndex = 0;
    private bool isRollingDice = false;
    private bool isBiddingDone = false;
    private bool isWaitingForDiceResult = false;
    private CameraAnimation Camera;
    private BiddingPhaseUI biddingPhaseUI;

    #region Unity Lifecycle

    private void Start() {
        biddingPhaseUI = FindAnyObjectByType<BiddingPhaseUI>();
        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        if (Camera == null){
            Camera = GameObject.FindObjectOfType<CameraAnimation>();
            Debug.Log("Camera Telah ditemukan");
        }
    }

    public void StartBiddingPhase() 
    {
        playerManager.ShufflePlayers();
        StartDiceRollForNextPlayer();
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
            AudioManagers.instance.PlaySoundEffect(1);
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

    private void StartDiceRollForNextPlayer()
    {
        if (currentPlayerIndex < playerManager.PlayerCount)
        {
            playerManager.HighightPlayerTurn(currentPlayerIndex);
            Player currentPlayer = playerManager.playerList[currentPlayerIndex].GetComponent<Player>();
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
            Player currentPlayer = playerManager.playerList[currentPlayerIndex].GetComponent<Player>();
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
        biddingPhaseUI.AnimateActive();
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
        biddingPhaseUI.AnimateDeactive();
        StartDiceRollForNextPlayer();
    }

    private void HandleDiceProcessResult()
    {
        Player currentPlayer = playerManager.playerList[currentPlayerIndex].GetComponent<Player>();
        currentPlayer.RollDice(Dice.Dice1, Dice.Dice2);
        Debug.Log(currentPlayer.Name + " mendapat nilai dadu: " + Dice.Dice1 + " dan " + Dice.Dice2);
        Debug.Log("Total nilai: " + currentPlayer.TotalScore);
        Dice.ResetDiceResult();
        currentPlayerIndex++;
        isWaitingForDiceResult = false;
    }

    private void HandleDuplicateDiceResults()
    {
        // Group players by their dice result (TotalScore), where there are duplicate results.
        var groupedResults = playerManager.playerList
            .GroupBy(p => p.GetComponent<Player>().TotalScore) // Group by TotalScore
            .Where(g => g.Count() > 1) // Filter only groups with more than one player (duplicate scores)
            .ToList();

        if (groupedResults.Count > 0)
        {
            Debug.Log("Ada pemain dengan nilai dadu yang sama. Menentukan prioritas berdasarkan urutan pemain asli.");

            // For each group with the same dice result, sort by their original position in playerManager.playerList
            foreach (var group in groupedResults)
            {
                List<Player> playersWithSameResult = group.Select(g => g.GetComponent<Player>()).ToList();
                
                // Sort players by their original index in the playerManager.playerList to maintain the original order
                playersWithSameResult.Sort((player1, player2) => playerManager.playerList.IndexOf(player1).CompareTo(playerManager.playerList.IndexOf(player2)));

                AssignPriorityToPlayers(playersWithSameResult); // Give priority or handle further sorting
            }
        }
        else
        {
            Debug.Log("Tidak ada nilai dadu yang sama atau semua telah diselesaikan.");
        }

        // Sort players by their final TotalScore in descending order
        SortPlayersByDiceResult();
    }


    private void AssignPriorityToPlayers(List<Player> playersWithSameResult)
{
    // Use a temporary priority system to avoid modifying TotalScore directly
    float priorityOffset = 0.01f;

    for (int i = 0; i < playersWithSameResult.Count; i++)
    {
        Player player = playersWithSameResult[i];
        
        // Optionally, store the priority in a new field or handle tie-breaking via another method
        player.SetPriority(priorityOffset);
        priorityOffset += 0.01f; // Increment priority offset for each player with the same score

        Debug.Log($"{player.Name} diberi prioritas dengan nilai baru: {player.TotalScore + priorityOffset}");
    }
}


    private void SortPlayersByDiceResult()
    {
        StartCoroutine(SortPlayersByDiceResultCoroutine());
    }

    private IEnumerator SortPlayersByDiceResultCoroutine()
    {
        playerManager.playerList = playerManager.playerList.OrderByDescending(p => p.GetComponent<Player>().TotalScore).ToList();
        for (int i = 0; i < playerManager.PlayerCount; i++)
        {
            playerManager.playerList[i].GetComponent<Player>().SetPlayOrder(i + 1);
            Debug.Log(playerManager.playerList[i].GetComponent<Player>().Name + " berada di urutan ke-" + (i + 1) + " dengan total nilai: " + playerManager.playerList[i].GetComponent<Player>().TotalScore);
        }

        gameManager.currentGameState = GameManager.GameState.Action;
        yield return new WaitForSeconds(2);
        gameManager.StartNextPhase();
        playerManager.ResetHighightPlayerTurn();
    }
}