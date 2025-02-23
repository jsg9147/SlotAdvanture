using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

public class GhostLegController : MonoBehaviour
{
    public GambleManager gambleManager;
    [SerializeField] private int baseBetMoney = 200;
    public int verticalMax = 6;
    public Button startBtn;
    public GameObject blind;
    public GameObject mouseBlock;

    public GhostLegSlime ghostLegSlime;
    public List<Button> numberBtns;
    public List<EndPoint> endPoints;
    public List<VerticalLine> verticalLines;
    ItemData rewardItem;

    bool isSelected;
    void Start()
    {
        startBtn.interactable = false;
        startBtn.onClick.AddListener(SlimeStart);
        SetHorizontalLines();
        isSelected = false;
        GameSetup();
    }

    public void GameSetup()
    {
        rewardItem = GameManager.Instance.ghostLegReward;
        if (rewardItem == null)
            SetReward();
        EndPointsMixMatch();
        blind.SetActive(true);

        startBtn.interactable = (ItemManager.Instance.currentMoney >= baseBetMoney) && isSelected;
    }
    void SetHorizontalLines()
    {
        for (int i = 0; i < verticalLines.Count - 1; i++)
        {
            verticalLines[i].SetLine(verticalMax);
        }
    }

    public void SlimeStart()
    {
        ItemManager.Instance.MoneyChange(-baseBetMoney);
        gambleManager.MoneyTextUpdate();
        SetLineOdd();
        blind.SetActive(false);
        mouseBlock.SetActive(true);
        ghostLegSlime.isMove = true;
        GameManager.Instance.playData.ghostLegCount++;
        startBtn.interactable = false;
    }

    void SetLineOdd()
    {
        bool isOdd = (Random.Range(0, 2) == 0);
        for (int i = 0; i < verticalLines.Count - 1; i++)
        {
            verticalLines[i].SetLineOdd(isOdd);
            isOdd = !isOdd;
        }
    }

    public void SelectNumber(int index)
    {
        ghostLegSlime.gameObject.SetActive(true);
        ghostLegSlime.transform.position = numberBtns[index].transform.position;
        isSelected = true;
        startBtn.interactable = (ItemManager.Instance.currentMoney >= baseBetMoney) && isSelected;
    }

    void EndPointsMixMatch()
    {
        int randomIndex = Random.Range(0, endPoints.Count);
        try
        {
            for (int i = 0; i < endPoints.Count; i++)
            {
                endPoints[i].SetWin(i == randomIndex);

                if(rewardItem != null)
                    endPoints[i].SetReward(rewardItem);
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
        
    }

    void SetReward()
    {
        rewardItem = ItemManager.Instance.itemPool.GetRandomItem(RoomConcept.GAMBLE);
        GameManager.Instance.ghostLegReward = rewardItem;
    }

    public void GameEnd(bool isWin)
    {
        ghostLegSlime.isMove = false;
        mouseBlock.SetActive(false);
        if (isWin)
        {
            Win();
        }
        else
        {
            Lose();
        }
    }

    void Win()
    {
        gambleManager.GetItemReward(rewardItem);
        GameManager.Instance.playData.ghostLegWinCount++;
        SetReward();
        GameReset();
    }

    void Lose()
    {
        MasterAudio.PlaySound("Lose");
        gambleManager.Defeat();
        ghostLegSlime.SetTrigger("Death");
        GameReset();
    }

    public void GameReset()
    {
        if (gambleManager.ghostLegWindow.activeSelf)
        {
            ghostLegSlime.animator.SetTrigger("Idle");
            ghostLegSlime.gameObject.SetActive(false);
            GameSetup();
        }
        startBtn.interactable = false;
    }

    public void SlimeSound() => MasterAudio.PlaySound("SlimeSound");
}
