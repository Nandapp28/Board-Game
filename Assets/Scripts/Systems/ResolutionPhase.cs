using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.Remoting.Messaging; // Pastikan Anda menggunakan DoTween

[System.Serializable]
public class HelpCards
{
    public GameObject helpCardDeck;
    public List<HelpCard> helpcard; // Tetap menggunakan List<HelpCard>
}

[System.Serializable]
public class CardSettings
{
    public Vector3 offsetFromCamera; // Offset dari kamera
    public float animationDuration; // Durasi animasi
    public Vector3 manualRotation; // Rotasi manual jika diperlukan
    public Vector3 targetScale; // Skala target untuk kartu
}

public class ResolutionPhase : MonoBehaviour
{
    #region Variabel And Property
    [Header("Help Card")]
    public HelpCards helpCards;
    public CardSettings cardSettings; // Tambahkan pengaturan kartu
    public HelpCard duplicatedHelpCard;

    public Camera Camera;
    public PlayerManager Players;
    public int currentPlayerIndex = 0;

    private GameManager gameManager;
    private ResolutionPhaseUI resolutionPhaseUI;
    
    [Header("Semester")]
    public SemesterManager semesterManager;
    public AllSectors allSectorsSemester;
    public CompanyPerformanceManager companyPerformanceManager;

    [Header("Camera")]
    public Vector3 Position;
    public Vector3 Rotation;
    private Vector3 initialCameraPosition;
    private Vector3 initialCameraRotation;
    #endregion

    #region Unity Methods

    private void Start() {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        semesterManager = FindAnyObjectByType<SemesterManager>();
        resolutionPhaseUI = FindAnyObjectByType<ResolutionPhaseUI>();
        companyPerformanceManager = FindAnyObjectByType<CompanyPerformanceManager>();
        Players = FindAnyObjectByType<PlayerManager>();
        allSectorsSemester = semesterManager.NewSectors;
        Camera = Camera.main;
        initialCameraPosition = Camera.transform.position;
        initialCameraRotation = Camera.transform.eulerAngles;
    }
    public IEnumerator StartResolutionPhase()
    {
        currentPlayerIndex = 0;
        resolutionPhaseUI.HandleToken(true);
        yield return new WaitForSeconds(3f);
        resolutionPhaseUI.HandleToken(false);
        yield return new WaitForSeconds(1f);
        MainCameraTargetPosition();
        yield return new WaitForSeconds(1.5f);
        SemesterCheck();
        CollectHelpCards();
        yield return new WaitForSeconds(2f);
        resolutionPhaseUI.HandleDividen(true);
        yield return new WaitForSeconds(3f);
        resolutionPhaseUI.HandleDividen(false);
        yield return new WaitForSeconds(1f);
        DividenCheck();
        yield return new WaitForSeconds(2f);
        MainCameraResetPostion();
        yield return new WaitForSeconds(1.5f);
        resolutionPhaseUI.HandleHelpCard(1);
        StartCoroutine(StartForNexPlayer());
    }
    #endregion

    private void MainCameraTargetPosition()
    {
        Camera.transform.DOMove(Position,0.7f);
        Camera.transform.DORotate(Rotation,0.7f);
    }

    private void MainCameraResetPostion()
    {
        Camera.transform.DOMove(initialCameraPosition,0.7f);
        Camera.transform.DORotate(initialCameraRotation,0.7f);
    }

    public IEnumerator StartForNexPlayer()
    {
        if(semesterManager.CurrentSemester < semesterManager.TotalSemester)
        {
            yield return new WaitForSeconds(0.2f);
            Debug.Log("Current Player Index: " + currentPlayerIndex);
            Debug.Log("Player Count: " + Players.PlayerCount);
            if (currentPlayerIndex < Players.PlayerCount)
            {
                Players.HighightPlayerTurn(currentPlayerIndex);
                resolutionPhaseUI.HandleHelpCard(1);
            }
            else
            {
                Debug.Log("No more players to process.");
            }
        }else{
            EndPhase();
        }

    }

    public void BuyButton()
    {
        buttonSoundEffect();
        resolutionPhaseUI.HandleHelpCard(0);
        StartCoroutine(DuplicateCard());
    }

    public void SkipButton()
    {
        buttonSoundEffect();
        MoveNextPlayer();
    }

    private IEnumerator DuplicateCard()
    {
        DuplicateRandomHelpCard();
        Debug.Log("Menunggu selama 10 detik sebelum melanjutkan ke pemain berikutnya...");
        yield return new WaitForSeconds(3f);
        DestroyCard();
        MoveNextPlayer();
    }

    private void MoveNextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex < Players.PlayerCount)
        {
            StartCoroutine(StartForNexPlayer());
        }
        else
        {
            resolutionPhaseUI.HandleHelpCard(0); // Sembunyikan HelpCard jika selesai
            Debug.Log("All players have been processed.");
            EndPhase();
        }
    }


    private void DestroyCard()
    {
        if (duplicatedHelpCard != null)
        {
            Debug.Log("Destroying duplicated card.");
            Destroy(duplicatedHelpCard.gameObject);
            duplicatedHelpCard = null;
        }
    }

    #region Help Card Management
    private void CollectHelpCards()
    {
        helpCards.helpcard.Clear();
        foreach (Transform child in helpCards.helpCardDeck.transform)
        {
            HelpCard helpCardComponent = child.GetComponent<HelpCard>(); // Mengambil komponen HelpCard
            if (helpCardComponent != null) // Memeriksa apakah komponen HelpCard ada
            {
                helpCards.helpcard.Add(helpCardComponent); // Menambahkan komponen HelpCard ke list
            }
        }

        Debug.Log("Jumlah HelpCard yang ditemukan: " + helpCards.helpcard.Count);
    }

    private void DuplicateRandomHelpCard()
    {
        if (helpCards.helpcard.Count > 0) // Pastikan ada kartu dalam list
        {
            int randomIndex = Random.Range(0, helpCards.helpcard.Count); // Ambil indeks acak
            HelpCard randomHelpCard = helpCards.helpcard[randomIndex]; // Ambil kartu acak

            // Menduplikat objek HelpCard
            GameObject DuplicateObject = Instantiate(randomHelpCard.gameObject); // Membuat salinan baru dari objek HelpCard

            duplicatedHelpCard = DuplicateObject.GetComponent<HelpCard>();
            if (duplicatedHelpCard != null)
            {
                Debug.Log("HelpCard duplicated: " + duplicatedHelpCard.name);
            }
            else
            {
                Debug.LogError("Duplicated HelpCard component is null!");
            }

            // Pindahkan kartu yang telah diduplikat ke depan kamera
            MoveCardToCamera(DuplicateObject.transform);
        }
        else
        {
            Debug.LogWarning("Tidak ada HelpCard yang tersedia untuk diduplikasi.");
        }
    }
    #endregion

    #region Card Animation
    private void MoveCardToCamera(Transform cardTransform)
    {
        // Hitung posisi target agar selalu di depan kamera dengan offset
        Vector3 targetPosition = Camera.transform.position + Camera.transform.forward * cardSettings.offsetFromCamera.z +
                                 Camera.transform.right * cardSettings.offsetFromCamera.x +
                                 Camera.transform.up * cardSettings.offsetFromCamera.y;

        // Gunakan rotasi manual yang telah diatur
        Quaternion targetRotation = Quaternion.Euler(cardSettings.manualRotation);

        // Pindahkan kartu ke posisi target
        Debug.Log("Moving card to position: " + targetPosition);
        cardTransform.DOMove(targetPosition, cardSettings.animationDuration).SetEase(Ease.InOutQuad);
        cardTransform.DORotateQuaternion(targetRotation, cardSettings.animationDuration).SetEase(Ease.InOutQuad);

        // Atur skala kartu
        cardTransform.localScale = Vector3.zero; // Mulai dari skala 0
        cardTransform.DOScale(cardSettings.targetScale, cardSettings.animationDuration).SetEase(Ease.OutBack); // Animasi skala
    }
    #endregion

    #region Semester
    public void SemesterCheck()
    {
        if(semesterManager.CurrentSemester > 0)
        {
            int index = semesterManager.CurrentSemester-1;
            Semester semesterSectors1 = allSectorsSemester.Consumer[index].GetComponent<Semester>();
            Semester semesterSectors2 = allSectorsSemester.Infrastuctur[index].GetComponent<Semester>();
            Semester semesterSectors3 = allSectorsSemester.Finance[index].GetComponent<Semester>();
            Semester semesterSectors4 = allSectorsSemester.Mining[index].GetComponent<Semester>();

            CompanyProfileUpdate(semesterSectors1,companyPerformanceManager.allSectorIn.Consumen);
            CompanyProfileUpdate(semesterSectors2,companyPerformanceManager.allSectorIn.Infrastuctur);
            CompanyProfileUpdate(semesterSectors3,companyPerformanceManager.allSectorIn.Finance);
            CompanyProfileUpdate(semesterSectors4,companyPerformanceManager.allSectorIn.Mining);

        }
    }

    public void CompanyProfileUpdate(Semester semester, SectorsIn companyProfile)
    {
        if(semester.value == 1)
        {
            companyPerformanceManager.IncreaseCurrentPriceIndex(companyProfile, 1);
        }
        if(semester.value == 2)
        {
            companyPerformanceManager.IncreaseCurrentPriceIndex(companyProfile, 2);
        }
        if(semester.value == -1)
        {
            companyPerformanceManager.DecreaseCurrentPriceIndex(companyProfile, 1);
        }
        if(semester.value == -2)
        {
            companyPerformanceManager.DecreaseCurrentPriceIndex(companyProfile, 2);
        }
    }
    #endregion

    #region Dividen

    public void DividenCheck()
    {
        int ConsumenCompanyIndex = companyPerformanceManager.allSectorIn.Consumen.CurrenIndikatorIndex;
        int InfrastucturCompanyIndex = companyPerformanceManager.allSectorIn.Infrastuctur.CurrenIndikatorIndex;
        int FinanceCompanyIndex = companyPerformanceManager.allSectorIn.Finance.CurrenIndikatorIndex;
        int MiningCompanyIndex = companyPerformanceManager.allSectorIn.Mining.CurrenIndikatorIndex;

        Debug.Log("Current Player Index: " + currentPlayerIndex);
        if (currentPlayerIndex < Players.PlayerCount)
        {
            Player CurrentPlayer = Players.playerList[currentPlayerIndex];

            if(CurrentPlayer.Consumen > 0)
            {
                Company consumen = companyPerformanceManager.allSectorIn.Consumen.Sector[ConsumenCompanyIndex].GetComponent<Company>();
                CurrentPlayer.AddWealth(consumen.Dividen);
            }
            if(CurrentPlayer.Infrastuctur > 0)
            {
                Company Infrastuctur = companyPerformanceManager.allSectorIn.Infrastuctur.Sector[InfrastucturCompanyIndex].GetComponent<Company>();
                CurrentPlayer.AddWealth(Infrastuctur.Dividen);
            }
            if(CurrentPlayer.Finance > 0)
            {
                Company Finance = companyPerformanceManager.allSectorIn.Finance.Sector[FinanceCompanyIndex].GetComponent<Company>();
                CurrentPlayer.AddWealth(Finance.Dividen);
            }
            if(CurrentPlayer.Mining > 0)
            {
                Company Mining = companyPerformanceManager.allSectorIn.Mining.Sector[MiningCompanyIndex].GetComponent<Company>();
                CurrentPlayer.AddWealth(Mining.Dividen);
            }

            MoveNextPlayerDividen();
        }
        else
        {
            Debug.Log("No more players to process.");
            currentPlayerIndex = 0;

        }
    }

    public void MoveNextPlayerDividen()
    {
        currentPlayerIndex++;
        DividenCheck();
    }

    #endregion

    #region End Phase
    private void EndPhase()
    {
        resolutionPhaseUI.HandleHelpCard(0);
        resolutionPhaseUI.HelpCard.SetActive(false);
        gameManager.currentGameState = GameManager.GameState.NextSemester;

        gameManager.StartNextPhase();
        Players.ResetHighightPlayerTurn();
    }
    #endregion

    private void buttonSoundEffect()
    {
        AudioManagers.instance.PlaySoundEffect(0);
    }
    
}
