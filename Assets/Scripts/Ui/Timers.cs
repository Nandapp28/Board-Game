using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CircularCountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [Tooltip("Durasi timer dalam detik")]
    public float countdownTime = 60f;

    [Header("UI References")]
    [Tooltip("Referensi ke Image lingkaran untuk progres")]
    public Image circleFill;         
    [Tooltip("Referensi ke TextMeshPro untuk angka di tengah")]
    public TextMeshProUGUI timerText;

    private float currentTime; // Waktu yang tersisa
    private bool isTimerRunning = false; // Status timer

    // Event yang dipanggil saat timer habis
    public event Action OnTimerEnd;

    private void Start()
    {
        ResetTimer();
        StartTimer(); // Timer otomatis berjalan saat mulai
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime; // Kurangi waktu setiap frame
                UpdateUI();
            }
            else
            {
                TimerEnd(); // Panggil fungsi saat waktu habis
            }
        }
    }

    /// Memperbarui UI timer
    private void UpdateUI()
    {
        // Format angka timer sebagai menit:detik (00:00)
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";

        // Update progres lingkaran
        if (circleFill != null)
            circleFill.fillAmount = Mathf.Clamp01(currentTime / countdownTime);
    }

    /// Fungsi untuk memulai timer
    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
        }
    }

    /// Fungsi untuk menghentikan timer
    public void StopTimer()
    {
        isTimerRunning = false;
    }

    /// Fungsi untuk mengatur ulang timer ke waktu awal
    public void ResetTimer()
    {
        currentTime = countdownTime;
        UpdateUI(); // Pastikan UI diperbarui
    }

    /// Fungsi yang dipanggil ketika waktu habis
    private void TimerEnd()
    {
        currentTime = 0;
        isTimerRunning = false;
        UpdateUI();

        Debug.Log("Waktu habis!");

        // Panggil event jika ada listener
        OnTimerEnd?.Invoke();
    }
}
