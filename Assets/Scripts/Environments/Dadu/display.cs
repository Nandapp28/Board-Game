using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // UI text untuk menampilkan score
    public FaceDetector ground;  // Referensi untuk DiceRoll dadu kedua

    void Update()
    {
    

        // Cek jika kedua dadu telah memiliki nilai face (tidak 0)
        if (ground.faceNumber1 != 0 && ground.faceNumber2 != 0)
        {
            // Jumlahkan nilai dari kedua dadu dan tampilkan hasilnya
            int totalScore = ground.faceNumber1 + ground.faceNumber2;
            // Set nilai teks
            scoreText.text = totalScore.ToString();
        }
    }
}
