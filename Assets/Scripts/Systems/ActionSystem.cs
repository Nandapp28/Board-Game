using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSystem : MonoBehaviour
{
    public float timeLimitPerTurn = 30f; // Time limit for each player's turn
    public GameObject Players; // GameObject containing player instances
    public List<Player> PlayerList = new List<Player>(); // List to hold player instances
    public ActionCardDeck carddeck;
    private int currentPlayerIndex = 0; // Index of the current player
    private Player currentPlayer;
    private float turnTimer; // Timer for each player's turn
    private bool actionPhaseActive = true; // Flag to check if action phase is active
    private bool isDrawingCard = false; // Flag to check if currently drawing a card
    private string currentDrawnCard; // To hold the drawn card name
    private bool waitingForPlayerChoice = false; // Flag to check if waiting for player choice

    // Example card pool (You can expand this list with actual card data)
    private List<string> cardPool = new() { "Flash Buy", "Trade Fee", "Stock Split", "Insider Trade" };

    // develop metode saja
    void Start()
    {
        StartActionPhase();
    }
    void Update()
    {
        // Check for input only if the current player's turn is active and action phase is active
        if (currentPlayer != null && actionPhaseActive)
        {
            turnTimer -= Time.deltaTime; // Decrease timer
            if (turnTimer <= 0)
            {
                Debug.Log(currentPlayer.nama + " time is up! Drawing a random card.");
                DrawRandomCard(currentPlayer); // Draw a random card if time is up
            }

            if (Input.GetKeyDown(KeyCode.A) && !waitingForPlayerChoice)
            {
                DrawCard(currentPlayer); // Draw a card when A is pressed
            }

            // Check for player's decision if waiting for input
            if (waitingForPlayerChoice)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    ActivateCardEffect(currentPlayer, currentDrawnCard);
                }
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    Debug.Log(currentPlayer.nama + " chose to store the card: " + currentDrawnCard + ". Effect will not be activated.");
                    MoveToNextPlayer();
                }
            }
        }
    }

    public void StartActionPhase()
    {
        Debug.Log("Action phase started");
        CollectPlayers();

        if (PlayerList.Count > 0)
        {
            SortPlayersByTurnOrder();
            StartTurn(); // Start the first player's turn
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
        PlayerList.Sort((player1, player2) => player1.urutanMain.CompareTo(player2.urutanMain));
    }

    private void StartTurn()
    {
        if (currentPlayerIndex < PlayerList.Count)
        {
            currentPlayer = PlayerList[currentPlayerIndex];
            Debug.Log("It's " + currentPlayer.nama + "'s turn.");
            turnTimer = timeLimitPerTurn; // Reset turn timer
            isDrawingCard = false; // Reset card drawing state
        }
        else
        {
            // All players have taken their turns
            StartCoroutine(EndActionPhase()); // Use coroutine to wait before ending the action phase
        }
    }

    private void DrawCard(Player player)
    {
        if (isDrawingCard) return; // Prevent drawing if already in the process of drawing a card

        isDrawingCard = true; // Set the flag to indicate card drawing has started
        // Logic to draw a random card from the card pool
        int cardIndex = Random.Range(0, cardPool.Count); // Get a random index
        currentDrawnCard = cardPool[cardIndex]; // Select the card from the pool

        // Log the drawn card
        Debug.Log(player.nama + " drew a card: " + currentDrawnCard);

        // Prompt for activation or storage
        Debug.Log(player.nama + ", do you want to activate or store the card: " + currentDrawnCard + "? (Press 'Y' to activate, 'N' to store)");
        waitingForPlayerChoice = true; // Set the flag to indicate waiting for player choice
        isDrawingCard = false; // Reset card drawing state
    }

    private void ActivateCardEffect(Player player, string drawnCard)
    {
        Debug.Log(player.nama + " activated the card: " + drawnCard);
        // Implement card effect logic here
        switch (drawnCard)
        {
            case "Flash Buy":
                Debug.Log("Effect: Draw an additional card.");
                DrawCard(player); // Allow drawing another card
                break;
            case "Trade Fee":
                Debug.Log("Effect: Apply trade fee logic.");
                break;
            case "Stock Split":
                Debug.Log("Effect: Apply stock split logic.");
                break;
            case "Insider Trade":
                Debug.Log("Effect: Apply insider trading logic.");
                break;
        }
        waitingForPlayerChoice = false; // Reset choice waiting flag
        MoveToNextPlayer();
    }

    private void DrawRandomCard(Player player)
    {
        // Logic to draw a random card for the player when time is up
        int cardIndex = Random.Range(0, cardPool.Count); // Get a random index
        currentDrawnCard = cardPool[cardIndex]; // Select the card from the pool

        // Log the drawn card
        Debug.Log(player.nama + " drew a random card: " + currentDrawnCard);

        // Move to the next player after the random card is drawn
        MoveToNextPlayer();
    }

    private void MoveToNextPlayer()
    {
        currentPlayerIndex++; // Move to the next player
        StartTurn(); // Start the next turn
    }

    private IEnumerator EndActionPhase()
    {
        actionPhaseActive = false; // Set the action phase to inactive
        // Wait for a short time to allow the last log to be visible
        yield return new WaitForSeconds(2f);
        Debug.Log("Action phase ended.");
        // Transition to the next phase
    }
}
