using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;

public class MapGenerateData : MonoBehaviour
{
    public bool blackSheepWall;

    private static MapGenerateData _instance;

    [SerializeField]
    private int battleMonsterMaxCount;

    [SerializeField]
    private int minSpacialRoomCount;
    [SerializeField]
    private int minRoomCount;
    private int maxRoomCount;

    private int remakeRoomCount = 0;
    private int spacialRoomCount = 0;

    private StageData stageData;
    
    public RoomInfo[,] posArr;
    public List<RoomInfo> validRoomList = new List<RoomInfo>();

    public int maxDistance;

    public Vector2Int startRoomPosition;

    int currnetRoomCount = 0;

    int maxXpos, minXpos, maxYpos, minYpos;

    public GameObject tutorialStageBackground;
    public GameObject fireStageBackground;
    public GameObject forestStageBackground;
    public GameObject snowStageBackground;
    public GameObject caveStageBackground;
    public GameObject lastStageBackground;

    bool tutorialSanctuary = false;
    public static MapGenerateData Instance
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
        
    }

    private List<Vector2Int> direction4 = new List<Vector2Int>
    {
        Vector2Int.left,    
        Vector2Int.right,      
        Vector2Int.up,     
        Vector2Int.down        
    };

    public void SetStageMap(int stage)
    {
        tutorialSanctuary = true;
        StageData stageData = new StageData();
        stageData.SetStage(stage);
        maxRoomCount = minRoomCount + Random.Range(stage , stage + 3);
        MapArrayGenerate(stageData);
        SetupPosition();

        if(!tutorialSanctuary)
        {
            GameManager.Instance.ResetStageData();
            SetStageMap(0);
        }
    }

    public void MapArrayGenerate(StageData newStageData)
    {
        stageData = newStageData;

        GameManager.Instance.SetStageData(stageData);

        stageData.SetRandomStage();
        stageData.background = StageBackground();

        posArr = new RoomInfo[maxDistance * 2, maxDistance * 2];

        RealaseRoom();

        int x = Random.Range(0, maxDistance) + (int)(maxDistance / 2);
        int y = Random.Range(0, maxDistance) + (int)(maxDistance / 2);

        startRoomPosition = new Vector2Int(x, y);

        posArr[startRoomPosition.x, startRoomPosition.y] = AddNormalRoom(new RoomInfo(), startRoomPosition, "START_ROOM");
        posArr[startRoomPosition.x, startRoomPosition.y].distance = 0;
        posArr[startRoomPosition.x, startRoomPosition.y].isClear = true;
        currnetRoomCount++;

        int count = 0;

        while(true)
        {
            count++;
            if(!(PossibleRoomCountCheck()))
            {
                FindRoomDistance(startRoomPosition, startRoomPosition);

                AddRoomList();

                int randomRoomIndex = Random.Range(0, validRoomList.Count - 1); // 이전방 못씀

                Vector2Int position = new Vector2Int(validRoomList[randomRoomIndex].center_Position.x, validRoomList[randomRoomIndex].center_Position.y);
                MakeRoomArray(position);
            }
            else
            {
                break;
            }

            if (count >= 100)
            {
                print("맵 생성 알고리즘 다시 짜야함");
                break;
            }
        }

        FindRoomDistance(startRoomPosition, startRoomPosition);

        AddRoomList();
        SortRoomList(validRoomList);

        AddSpacialRoom("BOSS");
        AddSpacialRoom("STORE");
        AddSpacialRoom("TREASURE");
        AddSpacialRoom("GAMBLE");
        AddSpacialRoom("SANCTUARY");

        if(spacialRoomCount < minSpacialRoomCount && remakeRoomCount < 10)
        {
            GameManager.Instance.ResetStageData();
            StageData stageData = new StageData();
            MapArrayGenerate(stageData);
            remakeRoomCount++;
        }
        else
        {
            remakeRoomCount = 0;
        }
    }

    GameObject StageBackground()
    {
        StageConcept stageConcept = GameManager.Instance.stageData.stageConcept;
        GameObject background = tutorialStageBackground;

        if (stageConcept == StageConcept.Tutorial)
            background = tutorialStageBackground;
        else if (stageConcept == StageConcept.Fire)
            background = fireStageBackground;
        else if (stageConcept == StageConcept.Forest)
            background = forestStageBackground;
        else if (stageConcept == StageConcept.Snow)
            background = snowStageBackground;
        else if (stageConcept == StageConcept.Cave)
            background = caveStageBackground;
        else if (stageConcept == StageConcept.Final)
            background = lastStageBackground;

        return background;
    }

    void RealaseRoomPosition()
    {
        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                posArr[i, j] = new RoomInfo();
                posArr[i, j].isValidRoom = false;
                posArr[i, j].distance = -1;
            }
        }
    }

    void RealaseRoom()
    {
        RealaseRoomPosition();
        validRoomList.Clear();

        currnetRoomCount = 0;
    }

    RoomInfo AddNormalRoom(RoomInfo room, Vector2Int pos, string name)
    {
        RoomInfo normalRoom = room;
        normalRoom.roomID = name + "(" + pos.x + ", " + pos.y + ")";
        normalRoom.roomName = name;
        normalRoom.center_Position = pos;
        normalRoom.parent_Position = pos;
        normalRoom.isValidRoom = true;

        return normalRoom;
    }
    void SetMaxPositionInfo(Vector2Int pos)
    {
        if (pos.x > maxXpos)
            maxXpos = pos.x;

        if (pos.y > maxYpos)
            maxYpos = pos.y;

        if (pos.x < minXpos)
            minXpos = pos.x;

        if (pos.y < minYpos)
            minYpos = pos.y;
    }

    // 모든 RoomInfo 에서 시작방부터의 거리를 세팅
    void FindRoomDistance(Vector2Int currentPosition, Vector2Int previousPosition)
    {
        if (!PossibleArr(currentPosition))
            return;
        
        int _distance = posArr[currentPosition.x, currentPosition.y].distance;

        for(int i = 0; i < direction4.Count; i++)
        {
            Vector2Int adjustPosition = currentPosition + direction4[i];

            if(PossibleArr(adjustPosition) && adjustPosition != previousPosition)
            {
                if (posArr[adjustPosition.x, adjustPosition.y].isValidRoom)
                {
                    if (posArr[adjustPosition.x, adjustPosition.y].distance != -1)
                    {
                        if((_distance + 1) <= posArr[adjustPosition.x, adjustPosition.y].distance)
                        {
                            posArr[adjustPosition.x, adjustPosition.y].distance = _distance + 1;
                            FindRoomDistance(adjustPosition, currentPosition);
                        }
                    }
                    else
                    {
                        posArr[adjustPosition.x, adjustPosition.y].distance = _distance + 1;
                        FindRoomDistance(adjustPosition, currentPosition);
                    }
                }
            }
                
        }
    }

    bool PossibleArr(Vector2Int pos)
    {
        if ((0 <= (pos).x && (pos).x < (maxDistance * 2)) && (0 <= (pos).y && (pos).y < (maxDistance * 2)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void AddRoomList()
    {
        validRoomList.Clear();
        
        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                if(posArr[i,j].isValidRoom)
                {
                    validRoomList.Add(posArr[i, j]);
                }
            }
        }
    }

    void MakeRoomArray(Vector2Int start)
    {
        if (start.x >= (maxDistance * 2) || start.y >= (maxDistance * 2))
            return;
        if (PossibleRoomCountCheck())
            return;

        for (int i = 0; i < direction4.Count; i++)
        {
            bool directionsRand = (Random.value > 0.5f);
            if(directionsRand)
            {
                //int selectPattern = (int)Choose(persent);
                if (PossibleMakeRoom(start, direction4[i]))
                {
                    if (!PossibleRoomCountCheck())
                    {
                        Vector2Int move = start + direction4[i];

                        posArr[move.x, move.y].isValidRoom = true;
                        posArr[move.x, move.y].roomName = "NORMAL";
                        posArr[move.x, move.y].center_Position = start + direction4[i];
                        posArr[move.x, move.y].distance = -1;

                        currnetRoomCount++;
                        MakeRoomArray(move);
                    }
                }

            }
        }
    }

    bool PossibleMakeRoom(Vector2Int pos, Vector2Int move)
    {
        bool possible = true;

        Vector2Int next = pos + move;
        if (PossibleArr(next))
        {
            if (posArr[next.x, next.y].isValidRoom)
                return false;
        }
        else
            return false;

        return possible;
    }

    bool PossibleRoomCountCheck()
    {
        int minRoom = minRoomCount + (stageData.Stage * 2);
        maxRoomCount = minRoom + Random.Range(0, stageData.Stage + 5);
        return ((minRoom <= currnetRoomCount && currnetRoomCount <= maxRoomCount));
    }

    void SortRoomList(List<RoomInfo> root)
    {
        root.Sort(delegate (RoomInfo A, RoomInfo B)
        {
            if (A.distance > B.distance)
                return 1;
            else if (A.distance < B.distance)
                return -1;
            else
                return 0;
        });
    }

    void AddSpacialRoom(string roomName)
    {
        SortRoomList(validRoomList);

        bool selectSpacialRoom = false;

        for (int idx = validRoomList.Count - 1; 0 < idx; idx--)
        {
            if (!selectSpacialRoom)
            {
                int setListCnt = idx;
                bool roomAdded = false;
                Vector2Int pos = validRoomList[setListCnt].center_Position;

                for (int i = 0; i < direction4.Count; i++)
                {
                    selectSpacialRoom = false;
                    Vector2Int roomPos = posArr[pos.x, pos.y].center_Position + direction4[i];
                    if (PossibleArr(roomPos))
                    {
                        if ((AroundRoomCount(roomPos) < 2)
                            && !posArr[roomPos.x, roomPos.y].isValidRoom && DoesNotExistSpaicailRoom(roomPos))
                        {
                            posArr[roomPos.x, roomPos.y].roomName = roomName;
                            posArr[roomPos.x, roomPos.y].roomID = roomName + "(" + roomPos.x + ", " + roomPos.y + ")";
                            posArr[roomPos.x, roomPos.y].isValidRoom = true;
                            posArr[roomPos.x, roomPos.y].center_Position = roomPos;
                            posArr[roomPos.x, roomPos.y].parent_Position = roomPos;
                            posArr[roomPos.x, roomPos.y].mergeCenter_Position = roomPos;
                            posArr[roomPos.x, roomPos.y].distance = posArr[pos.x, pos.y].distance + 1;
                            posArr[roomPos.x, roomPos.y].isClear = false;
                            selectSpacialRoom = true;
                            validRoomList.Add(posArr[roomPos.x, roomPos.y]);
                            spacialRoomCount++;
                            break;
                        }
                    }
                }

                if (stageData.Stage == 0 && idx == 1)
                {
                    if (!roomAdded && roomName == "SANCTUARY")
                    {
                        tutorialSanctuary = false;
                    }
                }
            }
        }
    }


    int AroundRoomCount(Vector2Int pos)
    {
        int count = 0;

        if ((0 <= (pos.y - 1) && (pos.y - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x, pos.y - 1].isValidRoom)
            {
                count += 1;
            }
        }

        if ((0 <= (pos.y + 1) && (pos.y + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x, pos.y + 1].isValidRoom)
            {
                count += 1;
            }
        }

        if ((0 <= (pos.x - 1) && (pos.x - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x - 1, pos.y].isValidRoom)
            {
                count += 1;
            }
        }

        if ((0 <= (pos.x + 1) && (pos.x + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x + 1, pos.y].isValidRoom)
            {
                count += 1;
            }
        }

        return count;
    }

    bool DoesNotExistSpaicailRoom(Vector2Int pos)
    {
        bool doesNotExist = true;

        if ((0 <= (pos.y - 1) && (pos.y - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x, pos.y - 1].roomConcept != RoomConcept.NORMAL && posArr[pos.x, pos.y - 1].isValidRoom )
            {
                doesNotExist = false;
            }
        }

        if ((0 <= (pos.y + 1) && (pos.y + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x, pos.y + 1].roomConcept != RoomConcept.NORMAL && posArr[pos.x, pos.y + 1].isValidRoom)
            {
                doesNotExist = false;
            }
        }

        if ((0 <= (pos.x - 1) && (pos.x - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x - 1, pos.y].roomConcept != RoomConcept.NORMAL && posArr[pos.x - 1, pos.y].isValidRoom)
            {
                doesNotExist = false;
            }
        }

        if ((0 <= (pos.x + 1) && (pos.x + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.x + 1, pos.y].roomConcept != RoomConcept.NORMAL && posArr[pos.x + 1, pos.y].isValidRoom)
            {
                doesNotExist = false;
            }
        }

        return doesNotExist;
    }

    void SetupPosition()
    {
        List<RoomInfo> roomsList = new List<RoomInfo>();

        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                if (posArr[i, j].isValidRoom)
                {
                    Vector2Int tmpArrayPosition = new Vector2Int(i, j);

                    posArr[i, j] = NomalRoom(posArr[i, j], posArr[i,j].roomName);
                    posArr[i, j].center_Position = tmpArrayPosition - startRoomPosition;
                    posArr[i, j].parent_Position = posArr[i, j].parent_Position - startRoomPosition;
                    posArr[i, j].mergeCenter_Position = posArr[i, j].mergeCenter_Position - startRoomPosition;
                    SetMaxPositionInfo(tmpArrayPosition - startRoomPosition);
                    roomsList.Add(posArr[i, j]);
                }
            }
        }

        roomsList.Distinct().ToList();

        GameManager.Instance.SetMapDataList(roomsList);
        
    }

    RoomInfo NomalRoom(RoomInfo pos, string name)
    {
        RoomInfo single = pos;
        single.roomID = name + "(" + pos.center_Position.x + ", " + pos.center_Position.y + ")";
        single.roomName = name;
        single.center_Position = pos.center_Position;
        single.mergeCenter_Position = pos.mergeCenter_Position;
        single.distance = pos.distance;

        return single;
    }

    public List<RoomInfo> GetAroundRooms(RoomInfo targetRoom, RoomInfo dest)
    {
        List<RoomInfo> aroundRooms = new List<RoomInfo>();

        aroundRooms.Add(GetRoomInfo(targetRoom.center_Position.x, targetRoom.center_Position.y - 1));
        aroundRooms.Add(GetRoomInfo(targetRoom.center_Position.x - 1, targetRoom.center_Position.y));
        aroundRooms.Add(GetRoomInfo(targetRoom.center_Position.x + 1, targetRoom.center_Position.y));
        aroundRooms.Add(GetRoomInfo(targetRoom.center_Position.x, targetRoom.center_Position.y + 1));

        bool containDest = aroundRooms.Contains(dest);

        aroundRooms.RemoveAll(x => x == null);

        if(!blackSheepWall)
            aroundRooms.RemoveAll(x => !x.isClear);

        if (containDest)
            aroundRooms.Add(dest);

        return aroundRooms;
    }

    RoomInfo GetRoomInfo(int x, int y)
    {
        foreach (RoomInfo room in validRoomList)
        {
            if (room.center_Position.x == x && room.center_Position.y == y)
            {
                return room;
            }
        }
        return null;
    }
}
