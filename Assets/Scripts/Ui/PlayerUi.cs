using TMPro;
using UnityEngine;

public class PlayerUi : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI nama;
    public TextMeshProUGUI urutan;
    public TextMeshProUGUI Dice;

    void Update()
    {
        nama.text = player.nama;
        
        // Casting totalNilai to int first before converting to string
        int totalScoreInt = (int)player.totalNilai;
        Dice.text = totalScoreInt.ToString();
        
        urutan.text = player.urutanMain.ToString();
    }
}
