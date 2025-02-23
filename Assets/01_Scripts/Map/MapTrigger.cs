using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class MapTrigger : MonoBehaviour
{
    [SerializeField] Vector3 revisePosition;
    public TMP_Text goldText;

    public RoomButton roomButtonPrefab;
    public GameObject mapPrefab;

    public TMP_Text stageText;
    public Button inventoryBtn;

    public GameObject bossRoomEffect;

    public GameObject canvas;
    GameObject mapParent;

    StageData stageData;

    #region 추가중
    List<RoomButton> roomButtons;

    Unit movePlayerUnit;

    bool canMove;
    #endregion

    void Start()
    {
        canMove = true;

        SetMap();
        SetStageText();
        SetBackground();
        inventoryBtn.onClick.AddListener(Inventory);
        bossRoomEffect.gameObject.SetActive(false);

        goldText.text = ItemManager.Instance.currentMoney.ToString();
        // 플레이 리스트 안에서 파일 클립 찾는 함수
        MasterAudio.ChangePlaylistByName(GameManager.Instance.stageData.stageConcept.ToString());
        stageText.font = LocalizationManager.Instance.GetFont();
    }

    void SetMap()
    {
        if (GameManager.Instance.stageData != null)
            SetMapPosition();
        else
        {
            MapGenerateData.Instance.SetStageMap(0);
            SetMapPosition();
        }

        stageData = GameManager.Instance.stageData;
        
    }

    void SetStageText()
    {
        stageText.text = $"{(stageData.Stage + 1)} {LocalizationManager.Instance.GetUILocalizingText("stage")} - {LocalizationManager.Instance.GetUILocalizingText(stageData.stageConcept.ToString())}";
    }

    void Inventory()
    {
        ItemManager.Instance.OpenInventory();
    }

    void SetBackground()
    {
        GameObject background = Instantiate(stageData.background);
        background.transform.position = Vector3.up * 0.5f;
    }

    public void MapPositionReset()
    {
        mapParent.transform.position = Vector3.zero;
    }
    #region 캐릭터 이동
    public void CharacterMove(RoomInfo dest)
    {
        if (dest == GameManager.Instance.currentPlayerRoom)
        {
            RoomMove(dest);
            return;
        }

        RoomInfo next = GameManager.Instance.currentPlayerRoom;
        RoomBtnSetInteractable();
        Sequence moveSeq = DOTween.Sequence();
        int count = 0;
        movePlayerUnit.PlayAnimation("isWalk", true);
        while (true)
        {
            if (next != null)
            {
                //moveSeq.Append(movePlayerUnit.transform.GetComponent<RectTransform>().DOLocalMove(MovePositionFix(next.buttonTrans, mapParent.transform), 0.4f));
                moveSeq.Append(movePlayerUnit.transform.DOLocalMove(MovePositionFix(next.buttonTrans), 0.4f));
                if (next.next != null)
                {
                    if (next.center_Position.x < next.next.center_Position.x)
                    {
                        moveSeq.Join(
                            movePlayerUnit.transform.DOScale(
                                new Vector3(Mathf.Abs(movePlayerUnit.transform.localScale.x), movePlayerUnit.transform.localScale.y, movePlayerUnit.transform.localScale.z), 0f));
                    }
                    else if (next.center_Position.x > next.next.center_Position.x)
                    {
                        moveSeq.Join(
                            movePlayerUnit.transform.DOScale(
                                new Vector3(Mathf.Abs(movePlayerUnit.transform.localScale.x) * -1, movePlayerUnit.transform.localScale.y, movePlayerUnit.transform.localScale.z), 0f));
                    }
                }

                next = next.next;
            }
            else
            {
                //moveSeq.Append(movePlayerUnit.transform.GetComponent<RectTransform>().DOLocalMove(MovePositionFix(dest.buttonTrans, mapParent.transform), 0.4f));
                moveSeq.Append(movePlayerUnit.transform.DOLocalMove(MovePositionFix(dest.buttonTrans) + (Vector3.back * 10f), 0.4f));
                break;
            }

            if (count > 20)
            {
                print("Error");
                break;
            }

            count++;
        }

        moveSeq.OnComplete(() => RoomMove(dest));

        if (dest != next)
            moveSeq.Play();
        else
            RoomMove(dest);
    }
    Vector3 MovePositionFix(Transform moveTransform)
    {
        Vector2 pos = moveTransform.GetComponent<RectTransform>().anchoredPosition;

        return new Vector3(pos.x, pos.y, 0) + revisePosition;
    }

    void RoomMove(RoomInfo _roomInfo)
    {
        GameManager.Instance.SetCurrentRoom(_roomInfo);

        if (_roomInfo.roomConcept == RoomConcept.NORMAL || _roomInfo.roomConcept == RoomConcept.BOSS)
        {
            GameManager.Instance.LoadScene("BATTLE");
        }
        else
        {
            GameManager.Instance.LoadScene(_roomInfo.roomConcept.ToString());
        }
    }

    #endregion

    #region 맵생성기에서 이전중

    public void SetMapPosition()
    {
        //canvas = GameObject.Find("Canvas");
        roomButtons = new List<RoomButton>();
        List<RoomInfo> roomInfoList = GameManager.Instance.currentStageMap;
        mapParent = Instantiate(mapPrefab, canvas.transform);

        mapParent.transform.SetAsFirstSibling();
        mapParent.transform.localPosition = new(0, -20, 0);
        SetUnitInstantiate();
        for (int i = 0; i < roomInfoList.Count; i++)
        {
            if (VisibleRoom(roomInfoList[i]))
            {
                RoomButtonInstantiate(roomInfoList[i]);
            }
        }
        SetMapParentPos();
        movePlayerUnit.transform.SetAsLastSibling();
    }

    void SetMapParentPos()
    {
        try
        {
            float left, right, top, bottom;
            left = roomButtons[0].GetComponent<RectTransform>().anchoredPosition.x;
            right = roomButtons[0].GetComponent<RectTransform>().anchoredPosition.x;
            top = roomButtons[0].GetComponent<RectTransform>().anchoredPosition.y;
            bottom = roomButtons[0].GetComponent<RectTransform>().anchoredPosition.y;
            for (int i = 0; i < roomButtons.Count; i++)
            {
                if (left > roomButtons[i].GetComponent<RectTransform>().anchoredPosition.x)
                    left = roomButtons[i].GetComponent<RectTransform>().anchoredPosition.x;
                if (right < roomButtons[i].GetComponent<RectTransform>().anchoredPosition.x)
                    right = roomButtons[i].GetComponent<RectTransform>().anchoredPosition.x;
                if (top < roomButtons[i].GetComponent<RectTransform>().anchoredPosition.y)
                    top = roomButtons[i].GetComponent<RectTransform>().anchoredPosition.y;
                if (bottom > roomButtons[i].GetComponent<RectTransform>().anchoredPosition.y)
                    bottom = roomButtons[i].GetComponent<RectTransform>().anchoredPosition.y;
            }
            float xPos = -((right + left) * 0.5f);
            float yPos = -((top + bottom) * 0.5f);
            mapParent.GetComponent<RectTransform>().anchoredPosition = new(xPos, yPos);
        }
        catch (System.NullReferenceException ex)
        {
            print($"{ex}");
        }
    }

    void SetUnitInstantiate()
    {
        if (GameManager.Instance.isGameStart)
        {
            int randomIndex = Random.Range(0, GameManager.Instance.playerUnitDatas.Length);

            if (GameManager.Instance.playerUnitDatas[randomIndex].currentHP <= 0)
            {
                randomIndex = (randomIndex + 1) % GameManager.Instance.playerUnitDatas.Length;
            }

            Unit heroUnit = Instantiate(PrefabManager.Instance.GetUnitPrefab(GameManager.Instance.playerUnitDatas[randomIndex]));

            heroUnit.Init(GameManager.Instance.playerUnitDatas[randomIndex]);
            heroUnit.transform.localScale = Vector3.one * 1.2f;

            if (heroUnit.unitData.sanctuarySkill != null)
            {
                GameObject aura = EffectManager.Instance.InstantiateAura(heroUnit.transform, heroUnit.unitData.sanctuarySkill);
                //aura.GetComponent<ParticleSystemRenderer>().sortingOrder = 1;
                aura.transform.localPosition = aura.transform.localPosition + (Vector3.forward * 0.1f);
            }

            heroUnit.gameObject.SetActive(true);
            heroUnit.GetComponent<BoxCollider2D>().enabled = false;
            movePlayerUnit = heroUnit;
        }
    }

    bool VisibleRoom(RoomInfo roomInfo)
    {
        return RoomDistance(roomInfo.center_Position) <= 1 || roomInfo.isClear || roomInfo.isVisible || MapGenerateData.Instance.blackSheepWall;
    }

    float RoomDistance(Vector2 room)
    {
        Vector2 playerRoom = GameManager.Instance.currentPlayerRoom.center_Position;

        float distance;

        distance = Mathf.Sqrt(Mathf.Pow(playerRoom.x - room.x, 2) + Mathf.Pow(playerRoom.y - room.y, 2));
        return distance;
    }
    void RoomButtonInstantiate(RoomInfo roomInfo)
    {
        RoomButton roomButton = Instantiate(roomButtonPrefab, mapParent.transform);

        roomButton.SetMapTrigger(this);

        Vector3 roomButtonPosition = new((roomInfo.center_Position.x) * roomButton.size, (roomInfo.center_Position.y) * roomButton.size, 0);
        roomButton.SetRoomInfo(roomInfo);
        roomButton.transform.localPosition = roomButtonPosition;
        roomInfo.buttonTrans = roomButton.GetComponent<RectTransform>();
        roomInfo.isVisible = true;
        if (GameManager.Instance.currentPlayerRoom == roomInfo)
        {
            //movePlayerUnit.transform.GetComponent<RectTransform>().anchoredPosition = MovePositionFix(roomButton.transform, mapParent.transform);
            //movePlayerUnit.transform.position = MovePositionFix(roomButton.transform, mapParent.transform) + (Vector3.up * 0.3f);
            movePlayerUnit.transform.SetParent(mapParent.transform);
            movePlayerUnit.transform.localPosition = MovePositionFix(roomButton.transform);
        }

        roomButtons.Add(roomButton);
    }
    void RoomBtnSetInteractable()
    {
        foreach (var btn in roomButtons)
        {
            btn.SetInteractable(false);
        }
    }
    #endregion

    public void MapPositionMove(Vector3 dir)
    {
        if (canMove)
        {
            canMove = false;
            mapParent.transform.DOMove(mapParent.transform.position + (dir), 0.5f).OnComplete(() => canMove = true);
            movePlayerUnit.transform.DOMove(movePlayerUnit.transform.position + dir + (Vector3.back * 10f), 0.5f);
        }
    }
    #region Move Map Function
    public void MoveUp()
    {
        MapPositionMove(Vector3.up);
    }
    public void MoveDown()
    {
        MapPositionMove(Vector3.down);
    }
    public void MoveLeft()
    {
        MapPositionMove(Vector3.left);
    }
    public void MoveRight()
    {
        MapPositionMove(Vector3.right);
    }
    #endregion
}
