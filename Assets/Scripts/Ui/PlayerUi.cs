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
        Dice.text = player.totalNilai.ToString();
        urutan.text = player.urutanMain.ToString();
    }
}