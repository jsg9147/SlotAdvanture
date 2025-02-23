using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StoreSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector]
    public ItemData itemData;
    public Button buyButton;    

    public Image itemIcon;

    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text countText;
    public GameObject countGroup;

    public Image slotColorPanel;

    public bool isSelected;

    StoreManager storeManager;

    public int itemCount;
    void Start()
    {
        isSelected = false;
        SetFont();
    }

    void SetFont()
    {
        nameText.font = LocalizationManager.Instance.GetFont();
        priceText.font = LocalizationManager.Instance.GetFont();
        countText.font = LocalizationManager.Instance.GetFont();
    }
    public void SetStoreManager(StoreManager storeManager) => this.storeManager = storeManager;

    public void SetItem(ItemData itemData)
    {
        this.itemData = itemData;
        itemIcon.sprite = itemData.icon;
        nameText.text = itemData.GetItemName();
        priceText.text = ((int)(itemData.price)).ToString();
        countGroup.SetActive(false);
    }

    public void SetPriceText(int price)
    {
        priceText.text = (price * 1.5f).ToString();
    }

    public void SetItemCount(int count)
    {
        countGroup.SetActive(true);
        itemCount = count;
        countText.text = itemCount.ToString();
    }

    public void ChangeItemCount(int changeValue)
    {
        if(!countGroup.activeSelf)
            countGroup.SetActive(true);

        itemCount += changeValue;
        countText.text = itemCount.ToString();

        if (itemCount <= 0 && itemData.type == ItemType.Consumable)
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.ItemInfoPopupActive(itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData != null)
            UIController.Instance.PopupOff();
    }
}
