using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public DiceRoll dice1;  // First dice
    public DiceRoll dice2;  // Second dice

    void Start()
    {
        // You can assign the DiceRoll components from the Inspector, or use FindObjectOfType
    }

    void Update()
    {
        if (dice1.diceFaceNumber != 0 && dice2.diceFaceNumber != 0)
        {
            int totalScore = dice1.diceFaceNumber + dice2.diceFaceNumber;
            Debug.Log(totalScore);
            scoreText.text = "Total: " + totalScore.ToString(); 
        }
    }
}
