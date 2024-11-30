using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionPhase : MonoBehaviour
{
    [SerializeField] private GameObject players; // GameObject yang berisi semua player
    public List<Player> playerList = new List<Player>(); // List untuk menyimpan komponen Player
    public CardManager cardManager; // Komponen CardManager
    public Button ActiveButton;
    public Button KeepButton;


    public void StartActionPhase()
    {
        CollectPlayers();
        cardManager.StartStockCards();
    }

    void CollectPlayers()
    {
        if (players == null)
        {
            Debug.LogError("GameObject players tidak ditetapkan!");
            return;
        }

        // Menghapus semua elemen dalam playerList sebelum mengisi
        playerList.Clear();

        // Memeriksa apakah GameObject players memiliki child
        if (players.transform.childCount == 0)
        {
            Debug.LogWarning("Tidak ada player ditemukan di dalam GameObject players!");
            return;
        }

        // Mengambil semua child dari GameObject players
        foreach (Transform child in players.transform)
        {
            Player playerComponent = child.GetComponent<Player>(); // Mendapatkan komponen Player
            if (playerComponent != null)
            {
                playerList.Add(playerComponent); // Menambahkan komponen Player ke dalam playerList
            }
            else
            {
                Debug.LogWarning("Child " + child.name + " tidak memiliki komponen Player.");
            }
        }

        // Menampilkan jumlah player yang dikumpulkan
        Debug.Log("Jumlah player yang dikumpulkan: " + playerList.Count);
        
        // Menampilkan nama dari setiap player yang dikumpulkan
        DisplayPlayerNames();
    }

    private void DisplayPlayerNames()
    {
        foreach (var player in playerList)
        {
            Debug.Log("Player ditemukan: " + player.Name);
        }
    }

}