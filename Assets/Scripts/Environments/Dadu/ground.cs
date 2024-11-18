using System;
using System.Collections;
using UnityEngine;

public class FaceDetector : MonoBehaviour
{
    public DiceRoll diceRollScript1;
    public DiceRoll diceRollScript2;
    public int faceNumber1;
    public int faceNumber2;
    private bool isDice1Stopped = false;
    private bool isDice2Stopped = false;

    void Start()
    {
        diceRollScript1 = FindObjectOfType<DiceRoll>();
        diceRollScript2 = FindObjectOfType<DiceRoll>();
    }

    public void OnTriggerStay(Collider other)
    {
   
        if (other.CompareTag("Dice1"))
        {
            Rigidbody rb = diceRollScript1.GetComponent<Rigidbody>();
            if (rb.IsSleeping() && !isDice1Stopped)
            {
                // Tunggu sebentar sebelum mendeteksi apakah benar-benar berhenti
                StartCoroutine(WaitForDiceToSettle(() => {
                    if (int.TryParse(other.name.Replace("Face", ""), out faceNumber1))
                    {
                        isDice1Stopped = true;
                        Debug.Log("Dice 1 stopped with face: " + faceNumber1);
                    }
                }));
            }
        }

        // Cek apakah dadu kedua telah berhenti
        if (other.CompareTag("Dice2"))
        {
            Rigidbody rb = diceRollScript2.GetComponent<Rigidbody>();
            if (rb.IsSleeping() && !isDice2Stopped)
            {
                // Tunggu sebentar sebelum mendeteksi apakah benar-benar berhenti
                StartCoroutine(WaitForDiceToSettle(() => {
                    if (int.TryParse(other.name.Replace("Face", ""), out faceNumber2))
                    {
                        isDice2Stopped = true;
                        Debug.Log("Dice 2 stopped with face: " + faceNumber2);
                    }
                }));
            }
        }
    }
    
    private IEnumerator WaitForDiceToSettle(Action callback)
    {
        yield return new WaitForSeconds(1f); 
        callback();
    }

    // Fungsi untuk mengembalikan apakah kedua dadu sudah berhenti dan mendapatkan hasilnya
    public bool AreBothDiceStopped()
    {
        return isDice1Stopped && isDice2Stopped;
    }

    // Fungsi untuk mengambil hasil nilai dadu
    public (int, int) GetDiceResults()
    {
        return (faceNumber1, faceNumber2);
    }

    // Fungsi untuk mereset status deteksi dadu
    public void ResetDiceDetection()
    {
        isDice1Stopped = false;
        isDice2Stopped = false;
        faceNumber1 = 0;
        faceNumber2 = 0;
    }
}
