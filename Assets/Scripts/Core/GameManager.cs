using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BiddingPhase biddingPhase; // Fase bidding
    public ActionPhase actionPhase; // Fase bidding
    public enum GameState { Bidding, Action, Selling, Rumor, Resolution, End }
    public GameState currentGameState = GameState.Bidding;

    void Start()
    {
        StartNextPhase();
    }

    public void StartNextPhase()
    {
        switch (currentGameState)
        {
            case GameState.Bidding:
                HandleBiddingPhase();
                break;
            case GameState.Action:
                HandleActionPhase();
                break;
            case GameState.Selling:
                HandleSellingPhase();
                break;
            case GameState.Rumor:
                HandleRumorPhase();
                break;
            case GameState.Resolution:
                HandleResolutionPhase();
                break;
            case GameState.End:
                HandleEndPhase();
                break;
            default:
                Debug.Log("Unknown Phase");
                break;
        }
    }

    public void HandleBiddingPhase() // Diubah menjadi public
    {
        Debug.Log("Bidding Phase");
        biddingPhase.StartBiddingPhase();
    }

    public void HandleActionPhase() // Diubah menjadi public
    {
        Debug.Log("Action Phase");

        actionPhase.StartActionPhase();
        // Logika untuk fase Action
        // Setelah fase action selesai, pindah ke fase Selling
    }

    public void HandleSellingPhase() // Diubah menjadi public
    {
        Debug.Log("Selling Phase");
        // Logika untuk fase Selling
        // Setelah fase selling selesai, pindah ke fase Rumor
        currentGameState = GameState.Rumor; // Contoh transisi
    }

    public void HandleRumorPhase() // Diubah menjadi public
    {
        Debug.Log("Rumor Phase");
        // Logika untuk fase Rumor
        // Setelah fase rumor selesai, pindah ke fase Resolution
        currentGameState = GameState.Resolution; // Contoh transisi
    }

    public void HandleResolutionPhase() // Diubah menjadi public
    {
        Debug.Log("Resolution Phase");
        // Logika untuk fase Resolution
        // Setelah fase resolution selesai, pindah ke fase End
        currentGameState = GameState.End; // Contoh transisi
    }

    public void HandleEndPhase() // Diubah menjadi public
    {
        Debug.Log("End Phase");
        // Logika untuk fase End
        // Setelah fase end, game bisa direset atau ditutup
    }
}