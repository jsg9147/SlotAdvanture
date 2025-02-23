using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SlotMachineManager : MonoBehaviour
{
    private static SlotMachineManager _instance;

    public static SlotMachineManager Instance
    {
        get 
        {
            if (_instance == null)
            {
                return null;
            }
            return _instance; 
        }
    }

    public Sprite[] slotItemSprites;

    public SlotMachineUI slotMachinePrefab;

    public float actionExcutionDelay;

    SlotMachineUI playerSlotMachineUI;
    SlotMachineUI monsterSlotMachineUI;

    AudioSource audioSource;

    public float result;
    private void Awake()
    {
        if (null == _instance)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        SlotInstantiate();
        audioSource = GetComponent<AudioSource>();
    }

    public void SlotInstantiate()
    {
        if (playerSlotMachineUI == null)
            playerSlotMachineUI = Instantiate(slotMachinePrefab, UIController.Instance.slotLayout);
        if(monsterSlotMachineUI == null)
            monsterSlotMachineUI = Instantiate(slotMachinePrefab, UIController.Instance.slotLayout);

        SlotMachineSet();

        playerSlotMachineUI.gameObject.SetActive(false);
        monsterSlotMachineUI.gameObject.SetActive(false);
    }

    void SlotMachineSet()
    {
        playerSlotMachineUI.SetSlotItemSprites(slotItemSprites);
        monsterSlotMachineUI.SetSlotItemSprites(slotItemSprites);

        playerSlotMachineUI.SetBelong(Belong.Player);
        monsterSlotMachineUI.SetBelong(Belong.Monster);
    }

    public void BattleSlotStart()
    {
        playerSlotMachineUI.gameObject.SetActive(true);
        monsterSlotMachineUI.gameObject.SetActive(true);

        playerSlotMachineUI.SlotMachineStart(actionExcutionDelay);
        monsterSlotMachineUI.SlotMachineStart(actionExcutionDelay);
        PlaySlotSound();
        StartCoroutine(ActionExcution());
    }

    void PlaySlotSound()
    {
        float clipLength = audioSource.clip.length;
        float startTime = Mathf.Abs(actionExcutionDelay - clipLength);

        audioSource.time = startTime;

        audioSource.pitch = GameManager.Instance.doubleSpeed ? 2.0f : 1.0f;

        audioSource.Play();
    }

    public void SlotStart(Belong belong)
    {
        SlotMachineUI slotMachine = belong == Belong.Player ? playerSlotMachineUI : monsterSlotMachineUI;
        slotMachine.gameObject.SetActive(true);
        slotMachine.SlotMachineStart(actionExcutionDelay);

        if (belong == Belong.Player)
            PlaySlotSound();
    }

    IEnumerator ActionExcution()
    {
        yield return new WaitForSeconds(DoubleSpeedCal(actionExcutionDelay));

        BattleManager.instance.MonsterActionExe();
    }

    float DoubleSpeedCal(float originSpeed)
    {
        float speed = originSpeed;
        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.doubleSpeed ? speed / 2 : speed;
        }

        return speed;
    }
}
