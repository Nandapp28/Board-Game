using UnityEngine;

public class Player : MonoBehaviour
{
    public string nama;       // Nama pemain
    public int nilaiDadu1;    // Nilai dadu pertama
    public int nilaiDadu2;    // Nilai dadu kedua
    public int totalNilai;    // Total nilai dari kedua dadu
    public int urutanMain;    // Urutan main

    public void RollDice(int dice1, int dice2)
    {
        nilaiDadu1 = dice1;
        nilaiDadu2 = dice2;
        totalNilai = nilaiDadu1 + nilaiDadu2;
    }

    public void SetUrutan(int urutan)
    {
        urutanMain = urutan;
    }
}
