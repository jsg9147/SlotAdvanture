using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public Transform slotLayout;

    public Image moveIcon;

    public Canvas canvas;

    [SerializeField] private InfoPopup infoPopupPrefab;
    private InfoPopup infoPopup;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += LoadSceneInit;

        //SetBuffPopup();

        infoPopup = Instantiate(infoPopupPrefab, transform);
        infoPopup.SetCanvas(gameObject);
        infoPopup.gameObject.SetActive(false);
    }

    void LoadSceneInit(Scene scene, LoadSceneMode mode)
    {
        canvas.worldCamera = Camera.main;
    }

    public void SetActiveMoveIcon(bool isActive, Sprite iconSprite = null)
    {
        moveIcon.gameObject.SetActive(isActive);
        if (iconSprite != null)
            moveIcon.sprite = iconSprite;
    }

    public void PopupOff()
    {
        infoPopup.gameObject.SetActive(false);
    }


    public void StatusEffectPopup(StatusEffect statusEffect)
    {
        infoPopup.gameObject.SetActive(true);
        infoPopup.SetBuffText(statusEffect);
    }

    public void SetSkillInfoPopup(UnitData unitData, SkillObject skillObject)
    {
        infoPopup.SetSkillData(skillObject, unitData);
        infoPopup.SetUnitData(unitData);
        infoPopup.gameObject.SetActive(true);
    }

    public void ItemInfoPopupActive(ItemData itemData)
    {
        infoPopup.SetItemData(itemData);
        infoPopup.gameObject.SetActive(true);
    }

    public void UnitDataPopupActive(Unit unit)
    {
        if (GameManager.Instance.infoPopupOn)
        {
            infoPopup.UnitInfoPopup(unit);
            infoPopup.gameObject.SetActive(true);
        }
    }
}
