using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BiddingPhase biddingPhase; // Fase bidding
    public ActionPhase actionPhase; // Fase bidding
    public SellingPhase sellingPhase; // Fase bidding
    public RumorPhase rumorPhase; // Fase bidding
    public ResolutionPhase resolutionPhase; // Fase bidding
    public EndGame endGame;
    public SemesterManager Semester;
    public StockPriceManager stockPriceManager;
    public CompanyPerformanceManager companyPerformanceManager;
    private PlayerManager playerManager;
    private PhaseUI phaseUI;
    private ShadowBackground shadowBackground;
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
        shadowBackground = FindAnyObjectByType<ShadowBackground>();
        companyPerformanceManager = FindAnyObjectByType<CompanyPerformanceManager>();
        phaseUI = FindAnyObjectByType<PhaseUI>();
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
        endGame = FindAnyObjectByType<EndGame>();
        AudioManagers.instance.PlayMusic(1);
        AudioManagers.instance.SetMusicVolume(0.4f);
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
                StartCoroutine(HandleBiddingPhase());
                break;
            case GameState.Action:
                StartCoroutine(HandleActionPhase());
                break;
            case GameState.Selling:
                StartCoroutine(HandleSellingPhase());
                break;
            case GameState.Rumor:
                StartCoroutine(HandleRumorPhase());
                break;
            case GameState.Resolution:
                StartCoroutine(HandleResolutionPhase());
                break;
            case GameState.NextSemester:
                StartCoroutine(HandleNextSemester());
                break;
            case GameState.End:
                StartCoroutine(HandleEndGame());
                break;
            default:
                Debug.Log("Unknown Phase");
                break;
        }
    }

    public IEnumerator HandleBiddingPhase() // Diubah menjadi public
    {
        phaseUI.HandleBiddingPhase(true);
        yield return new WaitForSeconds(5f);
        phaseUI.HandleBiddingPhase(false);
        yield return new WaitForSeconds(1f);
        Debug.Log("Bidding Phase");
        biddingPhase.StartBiddingPhase();
    }

    public IEnumerator HandleActionPhase() // Diubah menjadi public
    {
        Debug.Log("Action Phase");
        phaseUI.HandleActionPhase(true);
        yield return new WaitForSeconds(5f);
        phaseUI.HandleActionPhase(false);
        yield return new WaitForSeconds(1f);

        actionPhase.StartActionPhase();
    }

    public IEnumerator HandleSellingPhase() // Diubah menjadi public
    {
        Debug.Log("Selling Phase");
        phaseUI.HandleSellingPhase(true);
        yield return new WaitForSeconds(5f);
        phaseUI.HandleSellingPhase(false);
        yield return new WaitForSeconds(1f);
        sellingPhase.StartSellingPhase();

    }

    public IEnumerator HandleRumorPhase() // Diubah menjadi public
    {
        Debug.Log("Rumor Phase");
        phaseUI.HandleRumorPhase(true);
        yield return new WaitForSeconds(5f);
        phaseUI.HandleRumorPhase(false);
        yield return new WaitForSeconds(1f);
        rumorPhase.StartRumorhPase();
    }

    public IEnumerator HandleResolutionPhase() // Diubah menjadi public
    {
        Debug.Log("Resolution Phase");
        phaseUI.HandleResolutionPhase(true);
        yield return new WaitForSeconds(5f);
        phaseUI.HandleResolutionPhase(false);
        yield return new WaitForSeconds(1f);
        StartCoroutine(resolutionPhase.StartResolutionPhase());
    }
    public IEnumerator HandleNextSemester() // Diubah menjadi public
    {
        Semester.NextSemester();
        if(Semester.CurrentSemester <= Semester.TotalSemester)
        {
            Debug.Log("Next Semester");

            Semester.AnimateSemesters();

            currentGameState = GameState.Bidding;

            while (!Semester.IsSemesterAnimateDone)
            {
                yield return null;
            }

            StartNextPhase();
            Semester.IsSemesterAnimateDone = false;
        }else{
            Debug.Log("Semester Selesai");
            StartCoroutine(HandleEndGame());
        }
    }

    public IEnumerator HandleEndGame()
    {
        Debug.Log("End Phase");
        stockPriceManager.SetAllPrice();
        for (int i = 0; i < playerManager.PlayerCount; i++)
        {
            Player player = playerManager.GetPlayer(i);
            int Consumen = player.Consumen*stockPriceManager.allSector.Consumen.CurrentPriceSector;
            int Infrastuctur = player.Infrastuctur*stockPriceManager.allSector.Infrastuctur.CurrentPriceSector;
            int Finance = player.Finance*stockPriceManager.allSector.Finance.CurrentPriceSector;
            int Mining = player.Mining*stockPriceManager.allSector.Mining.CurrentPriceSector;
            
            player.Wealth += Consumen + Infrastuctur + Finance + Mining;
        }
        shadowBackground.HandleShadowBackground(true);
        yield return new WaitForSeconds(1.5f);
        endGame.StartEndGame();
    }
}