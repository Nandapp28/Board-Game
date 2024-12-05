using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class SellingPhase : MonoBehaviour {
    public TextMeshProUGUI Timers;
    private PlayerManager Players;
    private int currentPlayerIndex = 0;
    private Coroutine currentTimerCoroutine; // Menyimpan coroutine timer saat ini
    private SellingPhaseUI sellingPhaseUI; // Referensi UI untuk fase penjualan

    public void StartSellingPhase() {
        Players = FindObjectOfType<PlayerManager>();
        sellingPhaseUI = FindObjectOfType<SellingPhaseUI>();

        Players.SortPlayersByPlayOrder();
        sellingPhaseUI.StartSellingPhaseUI();
        StartSellingPhaseForNexPlayer();
    }

    private void StartSellingPhaseForNexPlayer() {
        if (currentPlayerIndex < Players.PlayerCount) {
            Player currentPlayer = Players.GetPlayer(currentPlayerIndex);
            // Lakukan sesuatu dengan currentPlayer
            currentTimerCoroutine = StartCoroutine(SellActionCardsWithTimer());
        }else{
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
        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++;
        StartSellingPhase(); // Mulai giliran pemain berikutnya
    }

    private void EndPhase() {
        Debug.Log("Selling Phase Berakhir.");
    }
}