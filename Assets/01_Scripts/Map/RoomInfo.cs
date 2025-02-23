using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo
{
    private string _roomName;
    public string roomID;
    public string roomName
    {
        get { return _roomName; }
        set 
        { 
            _roomName = value;
            SetRoomConcept(_roomName);
        }
    }
    
    public RoomConcept roomConcept = RoomConcept.NORMAL;

    public Vector2Int room_Position;

    // 현재 방(개별)의 위치
    public Vector2Int center_Position;
    // 부모 방의 위치
    public Vector2Int parent_Position;
    // 해당 방(통합)의 중앙 위치
    public Vector2 mergeCenter_Position;
    // 해당 방의 상태 설정(true : 방 셋팅, false : 빈방)
    public bool isValidRoom;
    // 시작 방에서 부터 해당 방까지의 거리
    public int distance;

    public List<Unit> monsterPrefabs = new List<Unit>();
    public List<UnitData> monsterDatas = new List<UnitData>();

    public bool isClear;

    public bool isVisible;

    public RoomInfo previous;
    public RoomInfo next;

    public bool isCheck = false;

    public int F => G + H;

    public int G { get; private set; } = 0;
    public int H { get; private set; } = 0;

    public RectTransform buttonTrans;

    void SetRoomConcept(string name)
    {
        switch (name)
        {
            case "START_ROOM":
                roomConcept = RoomConcept.START;
                break;
            case "TREASURE":
                roomConcept = RoomConcept.TREASURE;
                break;
            case "BOSS":
                roomConcept = RoomConcept.BOSS;
                break;
            case "SANCTUARY":
                roomConcept = RoomConcept.SANCTUARY;
                break;
            case "STORE":
                roomConcept = RoomConcept.STORE;
                break;
            case "GAMBLE":
                roomConcept = RoomConcept.GAMBLE;
                break;
            default:
                break;
        }
    }

    public void SetMonsterData(List<Unit> unitPrefabs, List<UnitData> unitDatas)
    {
        monsterPrefabs = unitPrefabs;
        monsterDatas = unitDatas;
    }

    public void SetPrice(int g, int h)
    {
        this.G = g;
        this.H = h;
    }

    public void Reset()
    {
        isCheck = false;
        previous = null;
        next = null;
        this.G = 0;
        this.H = 0;
    }
}

public enum RoomConcept
{
    START,
    STORE,
    TREASURE,
    BOSS,
    SANCTUARY,
    NORMAL,
    GAMBLE,
    TEST
}