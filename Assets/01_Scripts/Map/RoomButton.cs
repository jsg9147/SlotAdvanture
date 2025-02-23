using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomButton : MonoBehaviour
{
    public MapTrigger mapTrigger;

    private RoomInfo _roomInfo;
    private Button button;

    public Image roomImage;
    public Image roomFrame;
    public float size;

    public List<Sprite> stageFrameSprites;

    public Sprite bossRoom;
    public Sprite treasureRoom;
    public Sprite storeRoom;
    public Sprite santuaryRoom;
    public Sprite gambleRoom;


    private void Awake()
    {
        size = GetComponent<RectTransform>().rect.width;
    }

    private void Start()
    {
        SetStageConcept(GameManager.Instance.stageData.stageConcept);
    }

    public void SetMapTrigger(MapTrigger mapTrigger) => this.mapTrigger = mapTrigger;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(MoveRoom);
        roomImage.color = Color.clear;

        _roomInfo = roomInfo;
        transform.name = roomInfo.center_Position.ToString();
        SetMapImage();
        if (_roomInfo.isClear)
        {
            button.interactable = false;
        }
    }

    public void MoveRoom()
    {
        Astar.PathFinding(GameManager.Instance.currentStageMap, GameManager.Instance.currentPlayerRoom, _roomInfo);
        if (_roomInfo.roomConcept == RoomConcept.BOSS)
        {
            mapTrigger.bossRoomEffect.transform.SetParent(GameManager.Instance.FrontCanvas.transform);
            mapTrigger.bossRoomEffect.SetActive(true);
            DarkTonic.MasterAudio.MasterAudio.PlaySound("Siren");
        }
        mapTrigger.CharacterMove(_roomInfo);
    }

    void SetMapImage()
    {
        if (_roomInfo.roomConcept == RoomConcept.BOSS)
            roomImage.sprite = bossRoom;
        if (_roomInfo.roomConcept == RoomConcept.TREASURE)
            roomImage.sprite = treasureRoom;
        if (_roomInfo.roomConcept == RoomConcept.STORE)
            roomImage.sprite = storeRoom;
        if (_roomInfo.roomConcept == RoomConcept.SANCTUARY)
            roomImage.sprite = santuaryRoom;
        if (_roomInfo.roomConcept == RoomConcept.GAMBLE)
            roomImage.sprite = gambleRoom;

        if (_roomInfo.roomName != "NORMAL")
        {
            roomImage.color = Color.white;
        }
    }

    void SetStageConcept(StageConcept stageConcept)
    {
        roomFrame.sprite = stageFrameSprites[((int)stageConcept)];
    }

    public void SetInteractable(bool isActive)
    {
        button.interactable = isActive;
    }
}
