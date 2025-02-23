using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DarkTonic.MasterAudio;

public class ButtonEventManager : MonoBehaviour, IPointerEnterHandler
{
    void Start()
    {
        ClickSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MasterAudio.PlaySound("OnMouse");
    }

    void ClickSound()
    {
        GetComponent<Button>().onClick.AddListener(() => MasterAudio.PlaySound("Click"));
    }

    public void BackMain() => SceneManager.LoadScene("Main");
}
