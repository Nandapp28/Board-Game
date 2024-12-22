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

    private void Start() {
        gameManager = FindAnyObjectByType<GameManager>();
        Players = FindObjectOfType<PlayerManager>();
        sellingPhaseUI = FindObjectOfType<SellingPhaseUI>();
    }

    public void StartSellingPhase() {
        Players.SortPlayersByPlayOrder();
        sellingPhaseUI.StartSellingPhaseUI();
        StartSellingPhaseForNexPlayer();
    }

    private void StartSellingPhaseForNexPlayer() {
        if (currentPlayerIndex < Players.PlayerCount) {
            Player currentPlayer = Players.GetPlayer(currentPlayerIndex);
            sellingPhaseUI.GetStockData(currentPlayer);
            // Lakukan sesuatu dengan currentPlayer
            currentTimerCoroutine = StartCoroutine(SellActionCardsWithTimer());
        }else{
            sellingPhaseUI.sectorsParent.gameObject.SetActive(false);
            EndPhase();
        }
    }

    private IEnumerator SellActionCardsWithTimer() {
        float timer = 30f; // Waktu 30 detik untuk keputusan

        while (timer > 0) {
            Timers.text = Mathf.Ceil(timer).ToString();
            timer -= Time.deltaTime; // Kurangi waktu berdasarkan waktu yang berlalu
            yield return null; // Tunggu hingga frame berikutnya
        }
        sellingPhaseUI.ResetCounts();
        // Jika waktu habis, lakukan tindakan yang sesuai
        Debug.Log("Waktu habis untuk pemain " + currentPlayerIndex + ". Melanjutkan ke pemain berikutnya.");
        MoveToNextPlayer();
    }

    public void OnSellButtonClick(){
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine); // Hentikan coroutine timer
            currentTimerCoroutine = null; // Reset coroutine
        }
        
        Player currentPlayer = Players.GetPlayer(currentPlayerIndex);
        int TotalEarnings = sellingPhaseUI.CalculateTotalEarnings();

        currentPlayer.Wealth = TotalEarnings;

        sellingPhaseUI.ResetCounts();
        MoveToNextPlayer();
    }
    public void OnSkipButtonClick(){
        if (currentTimerCoroutine != null)
        {
            StopCoroutine(currentTimerCoroutine); // Hentikan coroutine timer
            currentTimerCoroutine = null; // Reset coroutine
        }

        sellingPhaseUI.ResetCounts();
        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++;
        StartSellingPhase();
    }

    private void EndPhase() {
        Debug.Log("Selling Phase Berakhir.");
        gameManager.currentGameState = GameManager.GameState.Rumor;
        gameManager.StartNextPhase();
    }
}