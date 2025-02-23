using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSlotController : MonoBehaviour
{
    public List<BuffSlot> buffSlots;

    void SlotInitialize()
    {
        for (int i = 0; i < buffSlots.Count; i++)
        {
            buffSlots[i].gameObject.SetActive(false);
        }
    }

    public void SetBuffSlot(List<StatusEffect> statusEffects)
    {
        SlotInitialize();
        for (int i = 0; i < buffSlots.Count; i++)
        {
            if (i < statusEffects.Count)
            {
                buffSlots[i].SetStatusInfo(statusEffects[i]);
                buffSlots[i].gameObject.SetActive(true);
            }
        }
    }
}