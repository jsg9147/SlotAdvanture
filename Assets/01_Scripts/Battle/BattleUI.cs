using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public GameObject resultWindow;
    public GameObject loseWindow;

    public Button[] batttleButtons;

    public List<SkillSlot> skillSlots;

    public StatUI playerStatUI;
    public StatUI monsterStatUI;

    public List<ItemSlot> battleConsumeSlots;

    public Image doubleSpeedImage;
    public Sprite[] doubleSpeedActiveSprite;

    private void Start()
    {
        SetDoubleSpeedBtnImage();
    }

    public void PotionBlockWhenFullHP(Unit unit)
    {
        if(unit.unitData.currentHP >= unit.unitData.stat.HP)
        {
            for (int i = 0; i < battleConsumeSlots.Count; i++)
            {
                if (battleConsumeSlots[i].itemData != null)
                {
                    if (battleConsumeSlots[i].itemData.GetTotalStat(Stat.HP) != 0)
                    {
                        battleConsumeSlots[i].GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    battleConsumeSlots[i].SlotReset();
                }
            }
        }
    }

    public void ActionButtonInteractable(bool interactable)
    {
        for(int i = 0; i < batttleButtons.Length; i++)
        {
            batttleButtons[i].interactable = interactable;
        }
        ConsumeItemObjectSet(interactable);
    }

    public void SetUnitData(Unit unit)
    {
        try
        {
            for (int i = 0; i < skillSlots.Count; i++)
            {
                skillSlots[i].SetUnitData(unit.unitData, i);
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void ConsumeItemObjectSet(bool interactable)
    {
        try
        {
            foreach (int slotIndex in ItemManager.Instance.consumeItemSlot.Keys)
            {
                if (ItemManager.Instance.consumeItemSlot[slotIndex] != null)
                {
                    ItemData itemData = ItemManager.Instance.consumeItemSlot[slotIndex];
                    battleConsumeSlots[slotIndex].SetItem(itemData);
                    battleConsumeSlots[slotIndex].SetItemCount(ItemManager.Instance.HaveItemCount(itemData));
                    battleConsumeSlots[slotIndex].SetBattle();
                }
                else
                {
                    battleConsumeSlots[slotIndex].SlotReset();
                }
            }
            ConsumeInteractable(interactable);
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void ConsumeCountUpdate()
    {
        for (int i = 0; i < battleConsumeSlots.Count; i++)
        {
            if (battleConsumeSlots[i].itemData != null)
            {
                int itemCount = ItemManager.Instance.HaveItemCount(battleConsumeSlots[i].itemData);
                battleConsumeSlots[i].SetItemCount(itemCount);
                if (itemCount <= 0)
                {
                    ItemManager.Instance.consumeItemSlot.Remove(i);
                    battleConsumeSlots[i].GetComponent<Button>().interactable = false;
                }
            }
            else
            {
                battleConsumeSlots[i].SetItemCount(0);
                battleConsumeSlots[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ConsumeInteractable(bool interactable)
    {
        for (int i = 0; i < battleConsumeSlots.Count; i++)
        {
            battleConsumeSlots[i].GetComponent<Button>().interactable  = (interactable);
            if (battleConsumeSlots[i].itemData == null)
            {
                battleConsumeSlots[i].GetComponent<Button>().interactable = false;
            }
        }
    }
    public void LoseUI()
    {
        loseWindow.SetActive(true);
    }

    public void BackMain()
    {
        GameManager.Instance.LoadScene("MAIN");
    }

    public void SetDoubleSpeedBtnImage()
    {
        if (GameManager.Instance.doubleSpeed)
            doubleSpeedImage.sprite = doubleSpeedActiveSprite[1];
        else
            doubleSpeedImage.sprite = doubleSpeedActiveSprite[0];
    }
}
