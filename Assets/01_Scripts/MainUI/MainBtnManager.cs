using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBtnManager : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button exitBtn;
    void Start()
    {
        startBtn.onClick.AddListener(() => GameManager.Instance.LoadScene("Lobby"));
        exitBtn.onClick.AddListener(() => Application.Quit());
    }
}
