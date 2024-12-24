using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BiddingPhase biddingPhase; // Fase bidding
    public ActionPhase actionPhase; // Fase bidding
    public SellingPhase sellingPhase; // Fase bidding
    public RumorPhase rumorPhase; // Fase bidding
    public ResolutionPhase resolutionPhase; // Fase bidding
    public SemesterManager Semester;
    public StockPriceManager stockPriceManager;
    public CompanyPerformanceManager companyPerformanceManager;
    private PlayerManager playerManager;
    public enum GameState { Bidding, Action, Selling, Rumor, Resolution, NextSemester ,End }
    public GameState currentGameState = GameState.Bidding;

    void Start()
    {
        InitializeManagers();
        StartCoroutine(StartSemestersCoroutine());
    }

    private void InitializeManagers()
    {
        playerManager = FindAnyObjectByType<PlayerManager>();
        Semester = FindAnyObjectByType<SemesterManager>();
        stockPriceManager = FindAnyObjectByType<StockPriceManager>();
        companyPerformanceManager = FindAnyObjectByType<CompanyPerformanceManager>();
        InitializePhases();
        playerManager.StartPlayerManager();
        stockPriceManager.StartStockManager();
        companyPerformanceManager.StartCompanyProfile();
    }

    private void InitializePhases()
    {
        biddingPhase = FindObjectOfType<BiddingPhase>();
        actionPhase = FindObjectOfType<ActionPhase>();
        sellingPhase = FindObjectOfType<SellingPhase>();
        rumorPhase = FindObjectOfType<RumorPhase>();
        resolutionPhase = FindObjectOfType<ResolutionPhase>();
    }

    private IEnumerator StartSemestersCoroutine()
    {
        Semester.StartSemesters();

        while (!Semester.IsSemesterAnimateDone)
        {
            yield return null; // Tunggu hingga animasi selesai
        }

        StartNextPhase();
        Semester.IsSemesterAnimateDone = false;
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
            case GameState.NextSemester:
                StartCoroutine(HandleNextSemester());
                break;
            case GameState.End:
                HandleEndGame();
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
        sellingPhase.StartSellingPhase();
        // Logika untuk fase Selling
        // Setelah fase selling selesai, pindah ke fase Rumor
    }

    public void HandleRumorPhase() // Diubah menjadi public
    {
        Debug.Log("Rumor Phase");
        rumorPhase.StartRumorhPase();
    }

    public void HandleResolutionPhase() // Diubah menjadi public
    {
        Debug.Log("Resolution Phase");
        resolutionPhase.StartResolutionPhase();
    }
    public IEnumerator HandleNextSemester() // Diubah menjadi public
    {
        if(Semester.CurrentSemester <= 4)
        {
            Debug.Log("Next Semester");
            Semester.NextSemester();
            Semester.AnimateSemesters();
            currentGameState = GameState.Bidding;

            while (!Semester.IsSemesterAnimateDone)
            {
                yield return null; // Tunggu hingga animasi selesai
            }

            StartNextPhase();
            Semester.IsSemesterAnimateDone = false;
        }else{
            Debug.Log("Semester Selesai");
            HandleEndGame();
        }
    }

    public void HandleEndGame() // Diubah menjadi public
    {
        Debug.Log("End Phase");
        // Logika untuk fase End
        // Setelah fase end, game bisa direset atau ditutup
    }
}