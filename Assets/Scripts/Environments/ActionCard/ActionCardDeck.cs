using UnityEngine;
using System.Collections;

public class ActionCardDeck : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform; // Transform camera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Offset distance from camera
    public Vector3 manualRotation = Vector3.zero; // Manual rotation of the cards

    [Header("Card Settings")]
    public GameObject cardPrefab; // Card prefab
    public int rows = 2; // Number of rows
    public int columns = 5; // Number of columns
    public float spacingX = 1.0f; // Spacing between cards in the X axis
    public float spacingY = 1.0f; // Spacing between cards in the Y axis
    public float spacingZ = 0.0f; // Optional depth spacing

    [Header("Animation Settings")]
    public float animationDuration = 0.5f; // Duration of card appearance animation
    public float cardDelay = 0.2f; // Delay between card appearances
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Target scale of the card

    private GameObject[] cards; // Array to store cards
    private int prevRows, prevColumns; // Previous layout values
    private float prevSpacingX, prevSpacingY; // Previous spacing values

    public void StartCardDeck()
    {
        cards = new GameObject[rows * columns]; // Initialize array for cards
        PositionParentInFrontOfCamera(); // Position parent in front of camera
        CacheCurrentValues(); // Cache current values
        StartCoroutine(GenerateCardGridWithAnimation()); // Generate card grid with animation
    }

    private void PositionParentInFrontOfCamera()
    {
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z +
                                 cameraTransform.right * offsetFromCamera.x +
                                 cameraTransform.up * offsetFromCamera.y;

        transform.position = targetPosition; // Set position
        transform.rotation = Quaternion.Euler(manualRotation); // Set rotation
    }

    private IEnumerator GenerateCardGridWithAnimation()
    {
        Vector3 centerOffset = CalculateCenterOffset(); // Calculate offset to center the grid
        int cardIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newCard = CreateCard(cardIndex++, row, column, centerOffset); // Create card
                StartCoroutine(AnimateCardAppearance(newCard, targetScale)); // Animate card appearance
                yield return new WaitForSeconds(cardDelay); // Wait before next card
            }
        }
    }

    private Vector3 CalculateCenterOffset()
    {
        float totalWidth = (columns - 1) * spacingX; // Total width of grid
        float totalHeight = (rows - 1) * spacingY;   // Total height of grid
        return new Vector3(totalWidth / 2, totalHeight / 2, spacingZ / 2); // Center offset
    }

    private GameObject CreateCard(int cardIndex, int row, int column, Vector3 centerOffset)
    {
        GameObject newCard = Instantiate(cardPrefab, transform); // Instantiate card prefab
        Vector3 cardPosition = new Vector3(column * spacingX, row * spacingY, 0) - centerOffset; // Calculate position
        newCard.transform.localPosition = cardPosition; // Set local position
        newCard.transform.localScale = Vector3.zero; // Set initial scale

        // Set initial position and scale for ActionCardAnimation component
        ActionCardAnimation cardAnimation = newCard.GetComponent<ActionCardAnimation>();
        if (cardAnimation != null)
        {
            cardAnimation.SetInitialPosition(cardPosition);
            cardAnimation.SetInitialScale(targetScale);
        }

        cards[cardIndex] = newCard; // Store card in array
        return newCard; // Return created card
    }

    private IEnumerator AnimateCardAppearance(GameObject card, Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = card.transform.localScale;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            card.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration); // Scale animation
            yield return null; // Wait for next frame
        }

        card.transform.localScale = targetScale; // Ensure final scale is correct
    }

    private void ClearOldCards()
    {
        if (cards != null)
        {
            foreach (GameObject card in cards)
            {
                if (card != null)
                {
                    Destroy(card); // Destroy old card
                }
            }
        }
    }

    private bool ValuesChanged()
    {
        return rows != prevRows || columns != prevColumns || spacingX != prevSpacingX || spacingY != prevSpacingY; // Check if layout changed
    }

    private void CacheCurrentValues()
    {
        prevRows = rows; // Cache current values
        prevColumns = columns;
        prevSpacingX = spacingX;
        prevSpacingY = spacingY;
    }
}
