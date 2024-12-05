using UnityEngine;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour {

    [SerializeField]
    private GameObject players; // GameObject yang berisi pemain
    private List<Player> playerList = new List<Player>(); // Daftar pemain

    void Start() {
        if (players == null) {
            Debug.LogError("Players GameObject is not assigned!");
            return;
        }
        CollectPlayers();
    }

    private void CollectPlayers() {
        // Clear the list before collecting players
        playerList.Clear();

        // Get all Player components from the children of the players GameObject
        Player[] playerComponents = players.GetComponentsInChildren<Player>();

        // Add each Player component to the playerList
        foreach (Player player in playerComponents) {
            playerList.Add(player);
            Debug.Log($"Collected player: {player.Name}, Play Order: {player.playOrder}");
        }

        // Log the number of players collected
        Debug.Log("Number of players collected: " + playerList.Count);
    }

    public void SortPlayersByPlayOrder() {
        playerList.Sort((a, b) => a.playOrder.CompareTo(b.playOrder));
        Debug.Log("Players sorted by play order.");
    }

    public Player GetPlayer(int index) {
        if (index < 0 || index >= playerList.Count) {
            Debug.LogError("Player index out of range.");
            return null;
        }
        return playerList[index];
    }

    public int PlayerCount {
        get { return playerList.Count; }
    }
}