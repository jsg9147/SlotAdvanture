using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfoPopupTopLayout : MonoBehaviour
{
    [SerializeField] private List<Sprite> labelGradeSprites;
    [SerializeField] private List<string> labelGradeTexts;

    [SerializeField] private Sprite playerLabelSprite;
    [SerializeField] private Sprite playerIconSprite;

    [SerializeField] private Sprite monsterLabelSprite;
    [SerializeField] private Sprite monsterIconSprite;


    [SerializeField] Image iconImage;

    [SerializeField] Image labelGrade;
    [SerializeField] TMP_Text labelText;

    [SerializeField] TMP_Text nameText;

    [SerializeField] GameObject priceGroup;
    [SerializeField] TMP_Text priceText;

    [SerializeField] GameObject turnGroup;
    [SerializeField] TMP_Text turnText;

    public void SetPrice(int price)
    {
        priceGroup.SetActive(true);
        priceText.text = $"{price}";
    }

    void NumberGroupOff()
    {
        priceGroup.SetActive(false);
        turnGroup.SetActive(false);
    }

    private void Start()
    {
        SetFont();
    }

    public void SetFont()
    {
        nameText.font = LocalizationManager.Instance.GetFont();
        priceText.font = LocalizationManager.Instance.GetFont();
        turnText.font = LocalizationManager.Instance.GetFont();
    }

    public void SetItemData(ItemData itemData)
    {
        try
        {
            NumberGroupOff();
            labelGrade.sprite = labelGradeSprites[itemData.itemGrade];
            labelText.text = labelGradeTexts[itemData.itemGrade];
            nameText.text = (itemData.GetItemName());
            
            iconImage.sprite = itemData.icon;
            SetPrice(itemData.price);
        }
        catch (System.ArgumentOutOfRangeException ex)
        {
            print($"{itemData.itemGrade}");
            print(ex);
        }
        catch (System.ArgumentException ex)
        {
            print($"{itemData.itemGrade}");
            print(ex);
        }
    }

    public void SetSkillData(SkillObject skillObject)
    {
        NumberGroupOff();
        labelGrade.sprite = labelGradeSprites[skillObject.grade];
        labelText.text = labelGradeTexts[skillObject.grade];
        
        nameText.text = (skillObject.GetName());
        iconImage.sprite = skillObject.SkillIcon;
        
    }
    public void SetEffectData(StatusEffect statusEffect)
    {
        try
        {
            NumberGroupOff();
            labelGrade.sprite = labelGradeSprites[0];
            nameText.text = (statusEffect.name);
            iconImage.sprite = statusEffect.icon;
            turnText.text = statusEffect.RemainingTurns.ToString();
            turnGroup.SetActive(true);
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
            NumberGroupOff();
            labelGrade.sprite = labelGradeSprites[0];
            turnGroup.SetActive(true);
        }
    }

    public void SetUnitData(Unit unit)
    {
        NumberGroupOff();
        iconImage.sprite = playerIconSprite;
        nameText.text = (unit.belong == Belong.Player) ? (unit.unitData.UnitName) : unit.unitData.MonsterName;
    }

    public void SetLabel(Belong belong)
    {
        labelGrade.sprite = belong == Belong.Player ? playerLabelSprite : monsterLabelSprite;
        labelText.text = belong == Belong.Player ? "Player" : "Monster";
    }
}
