using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotMachineUI : MonoBehaviour
{
    public Button[] slots; //  슬롯 한줄씩
    public Transform[] slotItemTransforms;  // 내부의 돌아가는 오브젝트
    public DisplayNumberSlot[] displaySlots; // 숫자 하나씩

    Sprite[] slotItemSprites;
    List<int> resultIndexList;

    Belong belong;

    float height;
    
    public void SetSlotItemSprites(Sprite[] slotSprites)
    {
        resultIndexList = new List<int>();
        slotItemSprites = slotSprites;
        SetSlotItem();
    }

    public void SetBelong(Belong belong) => this.belong = belong;

    public void SlotMachineStart(float disapearTime)
    {
        SlotReset();
        StartSlot();

        StartCoroutine(DelaySetAcitve(disapearTime));
    }

    void SetSlotItem()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            for (int j = 0; j < displaySlots[i].slotSprite.Count; j++)
            {
                slots[i].interactable = false;

                displaySlots[i].slotSprite[j].sprite = slotItemSprites[j];
            }
        }
    }

    void StartSlot()
    {
        for (int i = 0; i < slotItemTransforms.Length; i++)
        {
            height = slotItemTransforms[i].GetComponent<RectTransform>().rect.height;
            height += slotItemTransforms[i].GetComponent<VerticalLayoutGroup>().spacing;
            resultIndexList.Add(Random.Range(0, 10));
            SlotMove(i);
        }
    }

    void SlotReset()
    {
        for (int i = 0; i < slotItemTransforms.Length; i++)
        {
            slotItemTransforms[i].localPosition = Vector3.zero;
        }

        resultIndexList.Clear();
    }

    void SlotMove(int index)
    {
        int loopCount = Random.Range(1, 2);
        slotItemTransforms[index].DOLocalMoveY(height * (displaySlots[index].slotSprite.Count - 1), DoubleSpeedCal(1f))
                .SetEase(Ease.Linear)
                .SetLoops(loopCount, LoopType.Restart)
                .OnComplete(() =>
                {
                    slotItemTransforms[index].DORewind();
                    SlotResult(slotItemTransforms[index], index);
                });
    }

    void SlotResult(Transform slot, int index)
    {
        slot.DOLocalMoveY(height * resultIndexList[index], DoubleSpeedCal(2f))
            .SetEase(Ease.OutQuart);

        if(index == slotItemTransforms.Length - 1)
        {
            Result();
        }
    }

    void Result()
    {
        int slotResult = 0;
        slotResult += resultIndexList[0] * 100;
        slotResult += resultIndexList[1] * 10;
        slotResult += resultIndexList[2];

        SlotMachineManager.Instance.result = slotResult;

        if (BattleManager.instance != null)
        {
            BattleManager.instance.SetSlotResult(belong, slotResult);
            GameManager.Instance.playData.SetSlotResult(belong, slotResult);
        }

        if (belong == Belong.Player)
        {
            if (slotResult == 0)
                SteamAchievements.Instance.HandleUnluckyStartAchievement();
            if (slotResult == 777)
                SteamAchievements.Instance.HandleGoodLuckAchievement();
            if (slotResult == 999)
                SteamAchievements.Instance.HandleHit999Achievement();
        }
    }

    float DoubleSpeedCal(float originSpeed)
    {
        float speed = originSpeed;
        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.doubleSpeed ? speed / 2 : speed;
        }

        return speed;
    }

    IEnumerator DelaySetAcitve(float disapearTime)
    {
        yield return new WaitForSeconds(DoubleSpeedCal(disapearTime));

        for (int i = 0; i < slotItemTransforms.Length; i++)
        {
            slotItemTransforms[i].DOKill();
        }

        gameObject.SetActive(false);
    }
}

[System.Serializable]
public class DisplayNumberSlot
{
    public List<Image> slotSprite = new List<Image>();
}