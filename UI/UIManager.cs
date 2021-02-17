using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [System.Serializable]
    public struct UIStats
    {
        public TMP_Text[] stats;
    }

    public bool rollButtonState;

    GameManager gameManager;
    [SerializeField]
    GameObject toolTipPanel;
    [SerializeField]
    GameObject[] toolTipUI;

    // "wskazniki" do przycisku, tektu wyswietlajacego liczbe oczek, notyfikacje i panel ustawien
    public Button buttonRoll;
    public Button buttonNextTurn;
    public TMP_Text diceText;
    public TMP_Text notificationText;
    public TMP_Text playerNameText;
    public GameObject optionsPanel;
    public GameObject statsPanel;
    [SerializeField]
    Button[] skillButtons;
    [SerializeField]
    GameObject[] skillFrames;
    [SerializeField]
    List<UIStats> UIstats = new List<UIStats>(4);

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (statsPanel.activeSelf) statsPanel.SetActive(false);
            else
            {
                UpdateStatsPanel();
                statsPanel.SetActive(true);
            }
        }
    }

    public void FillToolTip(int skillIndex)
    {
        Player player = gameManager.GetCurrentPlayer();

        toolTipPanel.SetActive(true);
        toolTipUI[0].GetComponent<TMP_Text>().text = player.skills[skillIndex].name;
        toolTipUI[1].GetComponent<TMP_Text>().text = player.skills[skillIndex].description;
        toolTipUI[2].GetComponent<TMP_Text>().text = "Target: " + player.skills[skillIndex].targetLimit.ToString();
        toolTipUI[3].GetComponent<TMP_Text>().text = "Cooldown: " + player.skills[skillIndex].cooldown.ToString();
        toolTipUI[4].GetComponent<TMP_Text>().text = "Range: " + player.skills[skillIndex].boundaryRange.ToString();
        toolTipUI[5].GetComponent<TMP_Text>().text = "Duration: " + player.skills[skillIndex].duration.ToString();
    }
    public void HideToolTip()
    {
        toolTipPanel.SetActive(false);
    }

    public void SetRollButton(bool interactable)
    {
        rollButtonState = buttonRoll.interactable = interactable;
    }

    public void SetDiceText(string text)
    {
        diceText.text = text;
    }

    public void SetNotificationText(string text)
    {
        notificationText.text = text;
    }

    public bool IsRollButtonInteractable()
    {
        return buttonRoll.interactable;
    }

    public void SetNextTurnButton(bool interactable)
    {
        buttonNextTurn.interactable = interactable;
    }

    public void SetPlayerNameLabel(string name)
    {
        playerNameText.text = name;
    }

    public void ResetGameUI(string firstPlayerName)
    {
        diceText.text = "";
        notificationText.text = "";
        playerNameText.text = firstPlayerName;
        buttonRoll.interactable = true;
        buttonNextTurn.interactable = false;
        optionsPanel.SetActive(false);
    }

    public void UpdateStatsPanel()
    {
        if (gameManager.playerStats == null || gameManager.playerStats.Count == 0)
            return;

        for (int i = 0; i < UIstats.Count; i++)
        {
            Stats stats = gameManager.playerStats[i];
            UIstats[i].stats[0].text = stats.Name;
            UIstats[i].stats[1].text = stats.Kills.ToString();
            UIstats[i].stats[2].text = stats.Deaths.ToString();
            UIstats[i].stats[3].text = stats.OnesRolled.ToString();
            UIstats[i].stats[4].text = stats.TwosRolled.ToString();
            UIstats[i].stats[5].text = stats.ThreesRolled.ToString();
            UIstats[i].stats[6].text = stats.FoursRolled.ToString();
            UIstats[i].stats[7].text = stats.FivesRolled.ToString();
            UIstats[i].stats[8].text = stats.SixsRolled.ToString();
            UIstats[i].stats[9].text = stats.Gold.ToString();
        }
    }

    public void UpdateSkillsIcons(List<Skill> skills)
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            skillButtons[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/" + skills[i].iconName);
            UpdateSkillSlotAppearance(i, skills[i].currCooldown);
        }
    }
    public void UpdateSkillSlotAppearance(int slotIndex, int cooldown)
    {
        if (cooldown > 0)
        {
            skillButtons[slotIndex].interactable = false;
            skillButtons[slotIndex].GetComponentInChildren<TMP_Text>().text = cooldown.ToString();
        }
        else
        {
            skillButtons[slotIndex].interactable = true;
            skillButtons[slotIndex].GetComponentInChildren<TMP_Text>().text = "";
        }
    }
    public void ToggleSkillHighlight(int index, bool toggle)
    {
        skillFrames[index].SetActive(toggle);
    }
}
