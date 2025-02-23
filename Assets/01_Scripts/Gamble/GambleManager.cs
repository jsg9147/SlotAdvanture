using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

public class GambleManager : MonoBehaviour
{
    [SerializeField]
    private int blackJackPrice;

    public Button backButton;
    public Button ghostLegButton;
    public Button blackjackButton;

    public BlackJackController blackJackController;
    public GhostLegController ghostLegController;

    public GameObject gambleWindow;
    public GameObject ghostLegWindow;
    public GameObject blackjackWindow;

    public TMP_Text moneyText;

    public GameObject betMoneyUISet;
    public TMP_Text betMoneyText;
    public Button betMoneyUpBtn;
    public Button betMoneyDownBtn;
    [HideInInspector]
    public int currentBetMoney;
    
    [SerializeField]
    private int basicBetMoney = 100;

    [Header("Reward List UI")]
    public GameObject getRewardListContent;

    public RewardController rewardController;
    private void Start()
    {
        UIOff();
        ghostLegButton.onClick.AddListener(() => GhostLegWindowOn());
        blackjackButton.onClick.AddListener(() => BlackJackStart());
        backButton.onClick.AddListener(() => BackBtn());

        moneyText.text = ItemManager.Instance.currentMoney.ToString();

        blackJackController.SetGambleMgr(this);
        gambleWindow.SetActive(true);

        betMoneyDownBtn.onClick.AddListener(() =>
        {
            BetMoneyChange(-basicBetMoney);
            BetBtnCheck();
        });
        betMoneyUpBtn.onClick.AddListener(() =>
        {
            BetMoneyChange(basicBetMoney);
            BetBtnCheck();
        });

        MasterAudio.ChangePlaylistByName("Gamble");

        SteamAchievements.Instance.HandleGamblerAchievement();
    }

    public void BlackJackStart()
    {
        UIOff();
        blackjackWindow.SetActive(true);
        betMoneyUISet.SetActive(true);
        moneyText.text = ItemManager.Instance.currentMoney.ToString();
        // 블랙잭도 보상 세팅 만들어야함
    }

    public void GhostLegWindowOn()
    {
        UIOff();
        ghostLegWindow.SetActive(true);
        moneyText.text = ItemManager.Instance.currentMoney.ToString();
    }

    public void BetStop()
    {
        betMoneyUpBtn.interactable = false;
        betMoneyDownBtn.interactable = false;
    }

    void UIOff()
    {
        BettingMoneyReset();
        betMoneyUISet.SetActive(false);
        gambleWindow.SetActive(false);
        blackjackWindow.SetActive(false);
        ghostLegWindow.SetActive(false);
    }

    public void BetBtnCheck()
    {
        betMoneyUpBtn.interactable = ItemManager.Instance.currentMoney - currentBetMoney >= basicBetMoney;
        betMoneyDownBtn.interactable = currentBetMoney >= basicBetMoney;
    }

    public void GambleBetting()
    {
        ItemManager.Instance.MoneyChange(-currentBetMoney);
        moneyText.text = ItemManager.Instance.currentMoney.ToString();
        BetStop();
    }

    void BettingMoneyReset()
    {
        currentBetMoney = 0;
        BetMoneyChange(basicBetMoney);
    }

    void BetMoneyChange(int changeMoney)
    {
        currentBetMoney += changeMoney;
        if (currentBetMoney < basicBetMoney)
            currentBetMoney = basicBetMoney;

        betMoneyText.text = currentBetMoney.ToString();
        MasterAudio.PlaySound("Bet");
    }

    public void GetMoney(int value)
    {
        ItemManager.Instance.MoneyChange(value);
        moneyText.text = ItemManager.Instance.currentMoney.ToString();
        BetBtnCheck();
        if(value > 0)
            MasterAudio.PlaySound("GetMoney");
    }

    public void MoneyTextUpdate()
    {
        moneyText.text = ItemManager.Instance.currentMoney.ToString();
    }

    void BackBtn()
    {
        if (gambleWindow.activeSelf)
        {
            GameManager.Instance.LoadScene("MAP");
        }
        else
        {
            UIOff();
            blackJackController.GameReset();
            ghostLegController.GameReset();
            gambleWindow.SetActive(true);
        }
    }

    public void GetItemReward(ItemData itemData)
    {
        rewardController.GambleReward(itemData);
        MasterAudio.PlaySound("GetItem");
    }
    public void GetGoldReward(int gold)
    {
        rewardController.GoldReward(gold);
    }

    public void Defeat()
    {
        rewardController.Defeat();
    }

    public void Draw()
    {
        rewardController.Draw();
    }
}
