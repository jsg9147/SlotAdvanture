using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryContext : MonoBehaviour
{
    public TMP_Text buttonText;
    public TMP_Text deleteButtonText;

    public Button useButton;
    public Button deleteButton;

    private void Start()
    {
        useButton.onClick.AddListener(ItemManager.Instance.UseItemBtn);
        useButton.onClick.AddListener(SetActiveFalse);
        deleteButton.onClick.AddListener(ItemManager.Instance.DeleteItem);
        deleteButton.onClick.AddListener(SetActiveFalse);
    }

    void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
