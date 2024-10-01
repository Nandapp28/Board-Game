using TMPro;
using UnityEngine;

public class PlayerUi : MonoBehaviour
{
    public Player player;
    public TextMeshProUGUI nama;
    public TextMeshProUGUI urutan;

    void Update()
    {
        nama.text = player.nama;
        urutan.text = player.urutanMain.ToString();
    }
}