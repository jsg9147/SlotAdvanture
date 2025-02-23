using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class BlackJackController : MonoBehaviour
{
    [SerializeField]
    private ItemSlot rewardSlot;

    [Header("Button Group")]
    public Button hitBtn;
    public Button standBtn;
    public Button restartBtn;

    [Header("Card Group")]
    public Card cardFrontPrefab;
    public Sprite[] cardFrontSprites;

    public TMP_Text playerScoreText;
    public TMP_Text dealerScoreText;

    List<Card> playerHand;
    List<Card> dealerHand;

    public Transform playerCardLayout;
    public Transform dealerCardLayout;

    public bool isPlayerTurn = true;
    public bool isGameOver = false;

    public Transform cardParent;

    List<Card> cardPooling;

    int cardIndex;

    [Header("UI Group")]
    public GameObject startUI;
    public GameObject gameUI;

    GambleManager gambleManager;

    private void Start()
    {
        //resultText.text = ("21 게임");
        CardObjPooling();
        SetRewardItem();

        hitBtn.onClick.AddListener(() => PlayerHit());
        standBtn.onClick.AddListener(() => PlayerStand());
        restartBtn.onClick.AddListener(() => Restart());

        MoneyCheck();
    }

    public void SetGambleMgr(GambleManager gambleManager) => this.gambleManager = gambleManager;
    void Restart()
    {
        GameManager.Instance.playData.BlackJackStart();
        MasterAudio.PlaySound("BlackjackStart");
        gambleManager.GambleBetting();
        
        GameReset();

        gameUI.SetActive(true);
        startUI.SetActive(false);
        DealInitialCards();
        hitBtn.interactable = true;
        standBtn.interactable = true;
    }

    void CardObjPooling()
    {
        cardPooling = new List<Card>();
        cardIndex = 0;
        for (int i = 0; i < 20; i++)
        {
            Card cardObj = Instantiate(cardFrontPrefab, cardParent);
            cardPooling.Add(cardObj);
            cardObj.gameObject.SetActive(false);
        }
    }

    public void DealInitialCards()
    {
        DealerPlay();
        PlayerHit();
    }

    bool ChkGameOver()
    {
        int value = playerHand.Sum(x => x.value);

        return value > 21;
    }

    void PlayerHit()
    {
        MasterAudio.PlaySound("BlackjackDeal");
        if (isPlayerTurn && !isGameOver)
        {
            playerHand.Add(GetCard(true));
            if (dealerHand.Sum(x => x.value) <= 21)
            {
                playerScoreText.text = playerHand.Sum(x => x.value).ToString();
            }
            else
            {
                playerScoreText.text = "Burst";
                hitBtn.interactable = false;
                standBtn.interactable = false;
            }

            if (ChkGameOver())
            {
                isPlayerTurn = false;
                StartCoroutine(LoseEffect());
            }
        }
    }

    IEnumerator LoseEffect()
    {
        yield return new WaitForSeconds(2f);
        Lose();
    }

    void DealerHit()
    {
        MasterAudio.PlaySound("BlackjackDeal");
        dealerHand.Add(GetCard(false));
        if (dealerHand.Sum(x => x.value) <= 21)
        {
            dealerScoreText.text = dealerHand.Sum(x => x.value).ToString();
        }
        else
        {
            dealerScoreText.text = "Burst";
        }

        if (!isPlayerTurn)
            StartCoroutine(DealerTurn());
    }
    Card GetCard(bool isPlayer)
    {
        Card drawCard = cardPooling[cardIndex];
        cardIndex++;

        drawCard.isPlayerCard = isPlayer;
        drawCard.gameObject.SetActive(true);
        int value = Random.Range(1, 37);
        drawCard.SetValue(value, cardFrontSprites[value - 1]);
        CardMove(drawCard);
        return drawCard;
    }

    void CardMove(Card card)
    {
        float moveTarget = card.isPlayerCard ? -230f : 255f;

        card.GetComponent<RectTransform>().DOAnchorPosY(moveTarget, 0.5f);
        StartCoroutine(SetCardParent(card));
    }

    IEnumerator SetCardParent(Card card)
    {
        yield return new WaitForSeconds(0.5f);

        if(card.isPlayerCard)
            card.transform.SetParent(playerCardLayout);
        else
            card.transform.SetParent(dealerCardLayout);
    }

    void PlayerStand()
    {
        if (isPlayerTurn && !isGameOver)
        {
            isPlayerTurn = false;
            DealerHit();
        }
    }

    IEnumerator DealerTurn()
    {
        yield return new WaitForSeconds(1f);
        DealerPlay();
    }

    void DealerPlay()
    {
        // 딜러의 턴 구현

        int value = dealerHand.Sum(x => x.value);

        if (value < 17 || playerHand.Sum(x => x.value) > value)
        {
            DealerHit();
        }
        else
        {
            CheckWinner();
        }
    }

    void CheckWinner()
    {
        if (playerHand.Sum(x => x.value) <= 21)
        {
            if (playerHand.Sum(x => x.value) == dealerHand.Sum(x => x.value))
                Draw();
            else if (playerHand.Sum(x => x.value) == 21 && dealerHand.Sum(x => x.value) != 21)
                BlackJackWin();

            else if (dealerHand.Sum(x => x.value) < playerHand.Sum(x => x.value))
                Win();

            else if (dealerHand.Sum(x => x.value) > 21)
                Win();

            else if (playerHand.Sum(x => x.value) < dealerHand.Sum(x => x.value))
                Lose();
        }
        else
            Lose();

        gambleManager.MoneyTextUpdate();
        gameUI.SetActive(false);
        MoneyCheck();
    }

    void Win()
    {
        //gambleManager.GetMoney(gambleManager.currentBetMoney * 2);
        gambleManager.GetGoldReward(gambleManager.currentBetMoney * 2);
        GameReset();
        GameManager.Instance.playData.blackJackWinCount++;
        MasterAudio.PlaySound("GetMoney");
    }
    void Lose()
    {
        GameReset();
        gambleManager.Defeat();
        gambleManager.GetMoney(0);
        MoneyCheck();
        MasterAudio.PlaySound("Lose");
    }

    void BlackJackWin()
    {
        gambleManager.GetMoney(gambleManager.currentBetMoney * 2);
        gambleManager.GetItemReward(rewardSlot.itemData);
        GameManager.Instance.playData.blackJackGetItemCount++;
        GameManager.Instance.playData.blackJackWinCount++;
        GameManager.Instance.blackjackReward = null;
     
        SetRewardItem();
        GameReset();
    }

    void Draw()
    {
        gambleManager.GetMoney(gambleManager.currentBetMoney);
        gambleManager.Draw();
        GameReset();
    }

    public void GameReset()
    {
        playerHand = new();
        dealerHand = new();

        isPlayerTurn = true;
        isGameOver = false;

        for (int i = 0; i < cardPooling.Count; i++)
        {
            cardPooling[i].transform.SetParent(cardParent);
            cardPooling[i].ResetValue();
            cardPooling[i].gameObject.SetActive(false);
        }

        playerScoreText.text = "0";
        dealerScoreText.text = "0";
        cardIndex = 0;

        hitBtn.interactable = false;
        standBtn.interactable = false;

        startUI.SetActive(true);
    }

    void SetRewardItem()
    {
        ItemData rewardItem = GameManager.Instance.blackjackReward;

        if (rewardItem == null)
        {
            rewardItem = ItemManager.Instance.itemPool.GetRandomItem(RoomConcept.GAMBLE);
            GameManager.Instance.blackjackReward = rewardItem;
        }

        rewardSlot.SetRewardItem(rewardItem);
    }

    void MoneyCheck()
    {
        restartBtn.interactable = ItemManager.Instance.currentMoney >= gambleManager.currentBetMoney;
    }
}