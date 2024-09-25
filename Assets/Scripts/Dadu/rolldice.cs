using UnityEngine;

public class DiceRoll : MonoBehaviour
{
    private Rigidbody rb;
    public float maxRandomForce = 10f;
    public float startRollForce = 10f;
    private float forceX, forceY, forceZ;
    public int diceFaceNumber;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RollDice();
        }
    }

    void RollDice()
    {
        rb.isKinematic = false;

        forceX = Random.Range(0f, maxRandomForce);
        forceY = Random.Range(0f, maxRandomForce);
        forceZ = Random.Range(0f, maxRandomForce);

        rb.AddForce(Vector3.up * startRollForce);
        rb.AddTorque(new Vector3(forceX, forceY, forceZ));
    }
}
