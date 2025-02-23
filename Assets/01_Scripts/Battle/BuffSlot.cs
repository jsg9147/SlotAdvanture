using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BuffSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public StatusEffect statusEffect;
    public Image iconSprite;


    public void SetStatusInfo(StatusEffect statusEffect)
    {
        this.statusEffect = statusEffect;
        iconSprite.sprite = statusEffect.icon;

        string remainingTurn = statusEffect.RemainingTurns <= 0 ? "" : statusEffect.RemainingTurns.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.StatusEffectPopup(statusEffect);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    private void OnMouseEnter()
    {
        UIController.Instance.StatusEffectPopup(statusEffect);
    }

    private void OnMouseExit()
    {
        UIController.Instance.PopupOff();
    }


    private void OnDisable()
    {
        
    }
}
