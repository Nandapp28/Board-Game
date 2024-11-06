using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public float timeLimitPerTurn = 30f;
    public GameObject Players;
    public List<Player> PlayerList = new List<Player>();
    public ActionCardDeck carddeck;
    private int currentPlayerIndex = 0;
    private Player currentPlayer;
    private float turnTimer;
    private bool actionPhaseActive = true;
    private bool isDrawingCard = false;
    private StockCard currentDrawnCard;
    private bool waitingForPlayerChoice = false;
    private bool cardActivated = false;

    public ActionCardAnimation activeCard = null;

    private List<StockCard> cardPool;

    private CameraAnimation cameraAnimation;

    void Update()
    {
        if (currentPlayer != null && actionPhaseActive)
        {
            // Jika kartu diaktifkan, langsung pindah ke pemain berikutnya tanpa menunggu timer
            if (cardActivated)
            {
                Debug.Log("Card effect activated, moving to next player immediately.");
                waitingForPlayerChoice = false;
                cardActivated = false; // Reset setelah efek kartu diterapkan
                MoveToNextPlayer();
            }

            // Timer berkurang jika tidak ada kartu yang diaktifkan
            turnTimer -= Time.deltaTime;
            if (turnTimer <= 0)
            {
                Debug.Log(currentPlayer.Name + " time is up! Drawing a random card.");
                DrawRandomCard(currentPlayer);
            }

            if (Input.GetKeyDown(KeyCode.A) && !waitingForPlayerChoice)
            {
                DrawCard(currentPlayer);
            }

            if (waitingForPlayerChoice)
            {
                if (Input.GetKeyDown(KeyCode.N))
                {
                    Debug.Log(currentPlayer.Name + " chose to store the card: " + currentDrawnCard.Type);
                    waitingForPlayerChoice = false;
                    MoveToNextPlayer();
                }
            }
        }
    }

    public void StartActionPhase()
    {
        Debug.Log("Action phase started");

        cameraAnimation = FindObjectOfType<CameraAnimation>();

        // cameraAnimation.ActionCamera();
        CollectPlayers();

        if (PlayerList.Count > 0)
        {
            SortPlayersByTurnOrder();
            StartTurn();
            carddeck.StartCardDeck();
        }
        else
        {
            Debug.Log("No players found.");
        }
    }

    void CollectPlayers()
    {
        if (PlayerList.Count == 0 && Players != null)
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
                    Debug.Log("Player component not found on: " + child.gameObject.name);
                }
            }
        }
    }

    public void SortPlayersByTurnOrder()
    {
        PlayerList.Sort((player1, player2) => player1.playOrder.CompareTo(player2.playOrder));
    }

    private void StartTurn()
    {
        if (currentPlayerIndex < PlayerList.Count)
        {
            currentPlayer = PlayerList[currentPlayerIndex];
            Debug.Log("It's " + currentPlayer.Name + "'s turn.");
            turnTimer = timeLimitPerTurn;
            isDrawingCard = false;
            waitingForPlayerChoice = false; // Tambahkan ini agar dipastikan tidak tertahan dari turn sebelumnya
        }
        else
        {
            CheckIfMoreCardsAvailable();
        }
    }

    private void DrawCard(Player player)
    {
        if (isDrawingCard) return;

        isDrawingCard = true;
        int cardIndex = Random.Range(0, cardPool.Count);
        currentDrawnCard = cardPool[cardIndex];

        Debug.Log(player.Name + " drew a card: " + currentDrawnCard.Type);
        Debug.Log(player.Name + ", do you want to activate or store the card: " + currentDrawnCard.Type + "? (Press 'Y' to activate, 'N' to store)");
        waitingForPlayerChoice = true;
        isDrawingCard = false;
    }

    private void DrawRandomCard(Player player)
    {
        if (carddeck.selectedCards.Count == 0)
        {
            Debug.Log("No cards available in the deck.");
            StartCoroutine(EndActionPhase());
            return;
        }

        int cardIndex = Random.Range(0, carddeck.selectedCards.Count);
        Transform selectedCard = carddeck.selectedCards[cardIndex];

        carddeck.selectedCards.RemoveAt(cardIndex);
        carddeck.AnimateAndDestroyCard(selectedCard);

        Debug.Log(player.Name + " drew a random card.");

        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++;
        if (currentPlayerIndex >= PlayerList.Count)
        {
            currentPlayerIndex = 0; // Reset ke pemain pertama
            Debug.Log("Round complete, resetting player index.");
        }

        StartTurn();
    }

    private void CheckIfMoreCardsAvailable()
    {
        if (carddeck.selectedCards.Count > 0)
        {
            currentPlayerIndex = 0;
            StartTurn();
        }
        else
        {
            StartCoroutine(EndActionPhase());
        }
    }

public void ActivateCurrentCardAnimation()
{
    if (activeCard != null && activeCard.IsAtTarget())
    {
        int cardIndex = carddeck.selectedCards.IndexOf(activeCard.transform);
        if (cardIndex != -1)
        {
            carddeck.selectedCards.RemoveAt(cardIndex);
        }

        activeCard.ActiveCardAnimation();
        activeCard = null;

        // Aktifkan efek kartu setelah animasi selesai
        if (currentDrawnCard != null)
        {
            Debug.Log(currentPlayer.Name + " activated the card: " + currentDrawnCard.Type);
            currentDrawnCard.ActivateEffect(currentPlayer); // Memanggil efek dari StockCard
            cardActivated = true; // Menandai bahwa kartu telah diaktifkan
        }else{
            Debug.Log("currentDrawncard tidak ada");
        }

        turnTimer = timeLimitPerTurn; // Reset timer jika diperlukan
        Debug.Log("Card animation activated, effect applied.");
    }
    else
    {
        Debug.Log("No card is ready to activate animation.");
    }
}


    public void OnCardClick(ActionCardAnimation clickedCard)
    {
        if (activeCard != null && activeCard != clickedCard)
        {
            activeCard.AnimateToInitial();
        }

        if (activeCard != clickedCard)
        {
            clickedCard.AnimateToTarget();
            activeCard = clickedCard;
        }
    }

    public void ReceiveStockCard(StockCard stockCard)
    {
        if (stockCard != null)
        {
            currentDrawnCard = stockCard; // Simpan StockCard ke variabel
            Debug.Log("StockCard received and stored: " + currentDrawnCard.Type);
        }
        else
        {
            Debug.LogWarning("No StockCard received.");
        }
    }

    private IEnumerator EndActionPhase()
    {
        actionPhaseActive = false;
        yield return new WaitForSeconds(2f);
        Debug.Log("Action phase ended.");
    }
}
