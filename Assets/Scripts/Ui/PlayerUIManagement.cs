using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUIManagement : MonoBehaviour
{
    [System.Serializable]
    public class PlayerUIManagerData
    {
        public string playerName;
        public string playOrder;
        public string diceResult;
        public string wealth;
        public string consumen; // Perbaikan penamaan
        public string infrastructure; // Perbaikan penamaan
        public string finance; // Perbaikan penamaan
        public string mining; // Perbaikan penamaan
    }

    [System.Serializable]
    public class PlayerUI
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI PlayOrder;
        public TextMeshProUGUI DiceResult;
        public TextMeshProUGUI Wealth; // Added for wealth
        public TextMeshProUGUI Consumen; // Added for Consumen
        public TextMeshProUGUI Infrastructure; // Perbaikan penamaan
        public TextMeshProUGUI Finance; // Added for Finance
        public TextMeshProUGUI Mining; // Added for Mining
        public Player player;
    }

    public GameObject parentPlayerContainer; 
    public PlayerUIManagerData playerUIManagerData;
    public List<PlayerUI> players = new List<PlayerUI>(); 

    private void Start() 
    {
        CollectPlayers(); 
    }

    private void Update() {
        foreach (var ui in players)
        {
            ui.Name.text = ui.player.Name;
            ui.PlayOrder.text = ui.player.playOrder.ToString();
            ui.DiceResult.text = Mathf.Round(ui.player.TotalScore).ToString();
            ui.Wealth.text = ui.player.Wealth.ToString();
            ui.Consumen.text = ui.player.Consumen.ToString();
            ui.Infrastructure.text = ui.player.Infrastuctur.ToString();
            ui.Finance.text = ui.player.Finance.ToString();
            ui.Mining.text = ui.player.Mining.ToString();
        }
    }

    private void CollectPlayers()
    {
        foreach (Transform child in parentPlayerContainer.transform)
        {
            Player playerr = child.GetComponent<Player>(); 
            if (playerr != null) 
            {
                PlayerUI playerUI = new PlayerUI
                {
                    Name = FindUIElement(child, playerUIManagerData.playerName),
                    PlayOrder = FindUIElement(child, playerUIManagerData.playOrder),
                    DiceResult = FindUIElement(child, playerUIManagerData.diceResult),
                    Wealth = FindUIElement(child, playerUIManagerData.wealth), // Collect wealth UI
                    Consumen = FindUIElement(child, playerUIManagerData.consumen), // Collect Consumen UI
                    Infrastructure = FindUIElement(child, playerUIManagerData.infrastructure), // Collect Infrastructure UI
                    Finance = FindUIElement(child, playerUIManagerData.finance), // Collect Finance UI
                    Mining = FindUIElement(child, playerUIManagerData.mining), // Collect Mining UI
                    player = playerr
                };

                players.Add(playerUI); 
            }
        }
    }

    private TextMeshProUGUI FindUIElement(Transform parent, string name)
    {
        GameObject foundObject = FindGameObjectByName(parent, name);
        return foundObject != null ? foundObject.GetComponent<TextMeshProUGUI>() : null;
    }

    private GameObject FindGameObjectByName(Transform parent, string name)
    {
        if (parent.name == name)
        {
            return parent.gameObject;
        }

        foreach (Transform child in parent)
        {
            GameObject result = FindGameObjectByName(child, name);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

}