using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    public DiceRoll diceRollScript1;
    public DiceRoll diceRollScript2;
    public int faceNumber1;
    public int faceNumber2;

    void Start()
    {
        diceRollScript1 = FindObjectOfType<DiceRoll>();
        diceRollScript2 = FindObjectOfType<DiceRoll>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dice1"))
        {
            Rigidbody rb = diceRollScript1.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude == 0)
            {
                
                if (int.TryParse(other.name.Replace("Face", ""), out faceNumber1))
                {
                    diceRollScript1.diceFaceNumber = faceNumber1;
                    Debug.Log("Dice1 Face Number: " + diceRollScript1.diceFaceNumber);
                }
            }
        }
        if (other.CompareTag("Dice2"))
        {
            Rigidbody rb = diceRollScript2.GetComponent<Rigidbody>();
            if (rb.velocity.magnitude == 0)
            {
                
                if (int.TryParse(other.name.Replace("Face", ""), out faceNumber2))
                {
                    diceRollScript2.diceFaceNumber = faceNumber2;
                    Debug.Log("Dice2 Face Number: " + diceRollScript2.diceFaceNumber);
                }
            }
        }
    }
}
