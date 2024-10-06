using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public ActionDeck actionDeck;
    public float timeLimitPerTurn = 30f;
    public GameObject Players;
    public List<Player> PlayerList = new List<Player>();

    public void StartActionPhase()
    {
        Debug.Log("Action phase Di Mulai");
        CollectingPlayer();

        if (PlayerList != null && PlayerList.Count > 0)
        {
            Debug.Log("Ada " + PlayerList.Count + " Player");
            
            SortPlayersByUrutan();
        }
        else
        {
            Debug.Log("Tidak ada Player");
        }
    }

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

    public void SortPlayersByUrutan()
    {
        // Mengurutkan Player berdasarkan urutanMain secara menaik (nilai terkecil berada di urutan pertama)
        PlayerList.Sort((player1, player2) => player1.urutanMain.CompareTo(player2.urutanMain));

        Debug.Log("Player telah diurutkan berdasarkan urutan main.");
    }

}

