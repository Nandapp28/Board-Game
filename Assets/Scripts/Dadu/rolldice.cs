using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    public float maxRandomForce = 10f;
    public float startRollForce = 10f;
    private float forceX, forceY, forceZ;
    public int diceFaceNumber;  // Holds the face number of this dice

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;  // Initially, keep the dice static
    }

    void Update()
    {
        // Check if the left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            RollDice();
        }
    }

    // Roll the dice by applying force and torque
    public void RollDice()
    {
        rb.isKinematic = false;  // Enable physics simulation

        forceX = Random.Range(0f, maxRandomForce);
        forceY = Random.Range(0f, maxRandomForce);
        forceZ = Random.Range(0f, maxRandomForce);

        rb.AddForce(Vector3.up * startRollForce);  // Start with an upward force
        rb.AddTorque(new Vector3(forceX, forceY, forceZ));  // Apply random torque to simulate randomness
    }
}
