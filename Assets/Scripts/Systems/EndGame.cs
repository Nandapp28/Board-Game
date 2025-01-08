using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [System.Serializable]
    public class PlayerChip
    {
        public GameObject Crown;
        public GameObject WinnerBg;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI TotalWealth;

        public PlayerChip(GameObject crown, GameObject winnerBg, TextMeshProUGUI name, TextMeshProUGUI totalWealth)
        {
            Crown = crown;
            WinnerBg = winnerBg;
            Name = name;
            TotalWealth = totalWealth;
        }
    }

    public GameObject EndGameParent;
    public Button HomeButton;
    public Button PlayAgainButton;
    public List<PlayerChip> PlayerChips = new();
    public List<Player> PlayersByWealth = new();

    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = FindAnyObjectByType<PlayerManager>();
        EndGameParent.SetActive(false);
        InitializePlayerChips();

        EndGameParent.transform.localScale = Vector3.zero;
        HomeButton.onClick.AddListener(HomeButtonHandler);
        PlayAgainButton.onClick.AddListener(PlayAgainHandler);
    }


    public void StartEndGame()
    {
        EndGameParent.SetActive(true);
        SortPlayersByWealth();
        UpdatePlayerChips();
        ShowUpAnimations();
    }

    private void InitializePlayerChips()
    {
        foreach (Transform child in EndGameParent.transform)
        {
            if (child.name == "PlayerOrderChip")
            {
                var crown = FindChildRecursive(child, "Crown");
                var winnerBg = FindChildRecursive(child, "Winner Bg");
                var name = FindChildRecursive(child, "Name")?.GetComponent<TextMeshProUGUI>();
                var totalWealth = FindChildRecursive(child, "TextWealth")?.GetComponent<TextMeshProUGUI>();

                if (crown && winnerBg && name && totalWealth)
                {
                    PlayerChips.Add(new PlayerChip(crown, winnerBg, name, totalWealth));
                }
            }
        }
    }

    private GameObject FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child.gameObject;

            var found = FindChildRecursive(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }

    private void SortPlayersByWealth()
    {
        _playerManager.playerList.Sort((p1, p2) => p2.Wealth.CompareTo(p1.Wealth));
        PlayersByWealth = new List<Player>(_playerManager.playerList);
    }

    private void UpdatePlayerChips()
    {
        for (int i = 0; i < PlayersByWealth.Count && i < PlayerChips.Count; i++)
        {
            var chip = PlayerChips[i];
            var player = PlayersByWealth[i];

            chip.Name.text = player.Name;
            chip.TotalWealth.text = $"${player.Wealth}";

            bool isWinner = (i == 0);
            chip.Crown.SetActive(isWinner);
            chip.WinnerBg.SetActive(isWinner);
        }
    }

    private void ShowUpAnimations()
    {
        EndGameParent.transform.DOScale(0.5898893f, 0.7f);
    }

    private void HomeButtonHandler()
    {
        SceneManager.LoadScene(3);
    }

    private void PlayAgainHandler()
    {
        SceneManager.LoadScene(1);
    }
}
