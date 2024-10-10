using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionCardDeck : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform cameraTransform; // Transform camera
    public Vector3 offsetFromCamera = new Vector3(0, 0, 2); // Offset distance from camera
    public Vector3 manualRotation = Vector3.zero; // Manual rotation of the cards

    [Header("Card Settings")]
    public GameObject CardParent; // Parent object containing all cards as children
    public Transform newParent; // New parent to hold selected cards to display
    public int rows = 2; // Number of rows
    public int columns = 5; // Number of columns
    public float spacingX = 1.0f; // Spacing between cards in the X axis
    public float spacingY = 1.0f; // Spacing between cards in the Y axis
    public float spacingZ = 0.0f; // Optional depth spacing

    [Header("Animation Settings")]
    public float animationDuration = 0.5f; // Duration of card appearance animation
    public float cardDelay = 0.2f; // Delay between card appearances
    public Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f); // Target scale of the card

    private List<Transform> allCards; // List to store all child cards
    private List<Transform> selectedCards; // List to store selected random cards

    public void StartCardDeck()
    {
        // Initialize lists
        allCards = new List<Transform>();
        selectedCards = new List<Transform>();

        // Get all children of CardParent and add them to the list
        int childCount = CardParent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = CardParent.transform.GetChild(i);
            allCards.Add(child); // Add each child to the list
        }

        // Randomly select 10 unique cards
        SelectRandomCards();

        // Position the newParent in front of the camera
        PositionNewParentInFrontOfCamera();

        // Start displaying selected cards in grid with animation
        StartCoroutine(GenerateCardGridWithAnimation());
    }

    private void SelectRandomCards()
    {
        // Create a temporary list to store available cards
        List<Transform> availableCards = new List<Transform>(allCards);

        // Randomly select 10 unique cards
        for (int i = 0; i < Mathf.Min(10, availableCards.Count); i++)
        {
            int randomIndex = Random.Range(0, availableCards.Count);
            Transform selectedCard = availableCards[randomIndex];
            selectedCards.Add(selectedCard);
            availableCards.RemoveAt(randomIndex); // Remove selected card to avoid duplicates
        }
    }

    private void PositionNewParentInFrontOfCamera()
    {
        // Calculate position in front of the camera, ensuring it is centered
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * offsetFromCamera.z;

        // Ensure the grid is centered vertically relative to the camera
        targetPosition += cameraTransform.up * (offsetFromCamera.y + (rows * spacingY) / 2);

        newParent.position = targetPosition; // Set new parent position
        newParent.rotation = Quaternion.Euler(manualRotation); // Set rotation for new parent
    }

    private IEnumerator GenerateCardGridWithAnimation()
    {
        Vector3 centerOffset = CalculateCenterOffset(); // Calculate offset to center the grid
        int cardIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (cardIndex >= selectedCards.Count) yield break; // Exit if no more cards to display
                Transform card = selectedCards[cardIndex++];
                card.SetParent(newParent, false); // Re-parent the card to newParent
                PositionCard(card, row, column, centerOffset); // Position the card in grid

                // Mendapatkan komponen ActionCardAnimation dan mengatur posisi dan skala inisial
                ActionCardAnimation cardAnimation = card.GetComponent<ActionCardAnimation>();
                if (cardAnimation != null)
                {
                    cardAnimation.SetInitialPosition(card.localPosition);
                    cardAnimation.SetInitialScale(targetScale);
                }

                StartCoroutine(AnimateCardAppearance(card)); // Animate card appearance
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

    private void PositionCard(Transform card, int row, int column, Vector3 centerOffset)
    {
        // Position the card in local space relative to the newParent
        Vector3 cardPosition = new Vector3(column * spacingX, -row * spacingY, 0) - centerOffset;
        card.localPosition = cardPosition; // Set local position relative to newParent
        card.localScale = Vector3.zero; // Set initial scale
        card.gameObject.SetActive(true); // Show card
    }

    private IEnumerator AnimateCardAppearance(Transform card)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = card.localScale;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            card.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration); // Scale animation
            yield return null; // Wait for next frame
        }

        card.localScale = targetScale; // Ensure final scale is correct
    }
}
