using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // UI text untuk menampilkan score
    public FaceDetector ground;  // Referensi untuk DiceRoll dadu kedua

    void Update()
    {
        // Log untuk mengecek apakah update dijalankan
        Debug.Log("Update running...");

        // Log untuk mengecek nilai dadu saat ini
        Debug.Log("Dice 1 Current Value: " + ground.faceNumber1);
        Debug.Log("Dice 2 Current Value: " + ground.faceNumber2);

        // Cek jika kedua dadu telah memiliki nilai face (tidak 0)
        if (ground.faceNumber1 != 0 && ground.faceNumber2 != 0)
        {
            // Jumlahkan nilai dari kedua dadu dan tampilkan hasilnya
            int totalScore = ground.faceNumber1 + ground.faceNumber2;
            Debug.Log("Dice 1: " + ground.faceNumber1);
            Debug.Log("Dice 2: " + ground.faceNumber2);
            Debug.Log("Total Score: " + totalScore);
            
            // Set nilai teks
            scoreText.text = totalScore.ToString();
        }
    }
}
