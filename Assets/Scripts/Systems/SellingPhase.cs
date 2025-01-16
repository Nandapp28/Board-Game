using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class SellingPhase : MonoBehaviour {

    [Header("UI Elements")]
    public TextMeshProUGUI Timers;

    private PlayerManager Players;
    private int currentPlayerIndex = 0;
    private Coroutine currentTimerCoroutine; 
    private SellingPhaseUI sellingPhaseUI;
    private GameManager gameManager;
    private ShadowBackground shadowBackground;

    private void Start() {
        gameManager = FindAnyObjectByType<GameManager>();
        Players = FindObjectOfType<PlayerManager>();
        sellingPhaseUI = FindObjectOfType<SellingPhaseUI>();
        shadowBackground = FindObjectOfType<ShadowBackground>();
    }

    public void StartSellingPhase() {
        Players.SortPlayersByPlayOrder();
        sellingPhaseUI.StartSellingPhaseUI();
        StartCoroutine(StartSellingPhaseForNexPlayerhandler());
        sellingPhaseUI.CollectCurrentPriceSector();
    }

    private IEnumerator StartSellingPhaseForNexPlayerhandler()
    {
        shadowBackground.HandleShadowBackground(true);
        yield return new WaitForSeconds(1f);
        StartSellingPhaseForNexPlayer();
    }

    private void StartSellingPhaseForNexPlayer() {
        if (currentPlayerIndex < Players.PlayerCount) {
            Player currentPlayer = Players.GetPlayer(currentPlayerIndex);
            sellingPhaseUI.GetStockData(currentPlayer);
            Players.HighightPlayerTurn(currentPlayerIndex);
            sellingPhaseUI.CurrentStock();
            // Lakukan sesuatu dengan currentPlayer
            currentTimerCoroutine = StartCoroutine(SellActionCardsWithTimer());
        }else{
            sellingPhaseUI.sectorsParent.gameObject.SetActive(false);
            Players.ResetHighightPlayerTurn();
            shadowBackground.HandleShadowBackground(false);
            EndPhase();
        }
    }

    private IEnumerator SellActionCardsWithTimer() {
        float timer = 30f; 

        while (timer > 0) {
            Timers.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime;
            yield return null; 
        }
        sellingPhaseUI.ResetCounts();
        Debug.Log("Waktu habis untuk pemain " + currentPlayerIndex + ". Melanjutkan ke pemain berikutnya.");
        MoveToNextPlayer();
    }

    public void OnSellButtonClick(){
        buttonSoundEffect();
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
            currentTimerCoroutine = null;
        }
        
        Player currentPlayer = Players.GetPlayer(currentPlayerIndex);
        int TotalEarnings = sellingPhaseUI.CalculateTotalEarnings();

        currentPlayer.Wealth += TotalEarnings;

        sellingPhaseUI.ResetCounts();
        MoveToNextPlayer();
    }
    public void OnSkipButtonClick(){
        buttonSoundEffect();
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine);
            currentTimerCoroutine = null;
        }

        sellingPhaseUI.ResetCounts();
        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++;
        StartSellingPhaseForNexPlayer();
    }

    private void ResetCurentPlayer()
    {
        currentPlayerIndex = 0;
    }

    private void EndPhase() {
        ResetCurentPlayer();
        Debug.Log("Selling Phase Berakhir.");
        gameManager.currentGameState = GameManager.GameState.Rumor;
        gameManager.StartNextPhase();
    }

    private void buttonSoundEffect()
    {
        AudioManagers.instance.PlaySoundEffect(0);
    }
}