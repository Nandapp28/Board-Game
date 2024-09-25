using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    public DiceRoll diceRollScript;  // Public so it can be manually assigned in the Inspector
    public int faceNumber;

    void Start()
    {
        // If diceRollScript is not manually assigned, try to get it from the parent
        if (diceRollScript == null)
        {
            diceRollScript = GetComponentInParent<DiceRoll>();
        }

        // If it's still null, log an error
        if (diceRollScript != null)
        {
            Debug.Log("DiceRollScript assigned correctly.");
        }
        else
        {
            Debug.LogError("DiceRollScript is still null. Make sure the Dice object has the DiceRoll script and it's assigned correctly.");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Face"))
        {
            if (diceRollScript != null && diceRollScript.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = diceRollScript.GetComponent<Rigidbody>();

                if (rb.velocity.magnitude == 0 && rb.angularVelocity.magnitude == 0)
                {
                    if (int.TryParse(other.name.Replace("Face", ""), out faceNumber))
                    {
                        diceRollScript.diceFaceNumber = faceNumber;
                        Debug.Log("Dice Face Number: " + diceRollScript.diceFaceNumber);
                    }
                }
            }
            else
            {
                if (diceRollScript == null)
                {
                    Debug.LogError("diceRollScript is null. Ensure the Dice object has the DiceRoll script and it's assigned correctly.");
                }
            }
        }
    }
}
