using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleUnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button button;
    Unit unit;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => ClickEvent());
    }

    public void SetUnit(Unit unit) => this.unit = unit;

    void ClickEvent()
    {
        if (unit != null && unit.IsAlive)
        {
            BattleManager.instance?.SelectUnit(unit);
        }
        UIController.Instance.PopupOff();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(unit != null)
            UIController.Instance.UnitDataPopupActive(unit);
    }

    public void Interactable(bool isActive)
    {
        button.interactable = isActive;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }
}
