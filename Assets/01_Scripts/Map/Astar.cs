using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    public static RoomInfo PathFinding(List<RoomInfo> roomInfos, RoomInfo start, RoomInfo dest)
    {
        for (int i = 0; i < roomInfos.Count; i++)
        {
            roomInfos[i].Reset();
        }

        if (roomInfos.Contains(start) && roomInfos.Contains(dest))
        {
            List<RoomInfo> waittingRooms = new List<RoomInfo>();
            List<RoomInfo> finishRoom = new List<RoomInfo>();

            RoomInfo current = start;
            while (current != null)
            {
                var aroundRooms = MapGenerateData.Instance.GetAroundRooms(current, dest);

                for (int i = 0; i < aroundRooms.Count; i++)
                {
                    var room = aroundRooms[i];
                    if (!waittingRooms.Equals(room) && !room.isCheck)
                        waittingRooms.Add(room);
                }

                current.isCheck = true;

                if (waittingRooms.Remove(current))
                    finishRoom.Add(current);

                if (aroundRooms.Count == 0)
                    return null;
                else
                {
                    aroundRooms = aroundRooms.FindAll(x => !x.isCheck);
                }

                CalcRating(aroundRooms, start, current, dest);

                current = GetNextRoomInfo(aroundRooms, current);
                
                if (current == null)
                {
                    current = GetPriorityRoom(waittingRooms);

                    if (current == null)
                    {
                        RoomInfo exceptionRoom = null;
                        for (int i = 0; i < finishRoom.Count; i++)
                        {
                            if (exceptionRoom == null || exceptionRoom.H > finishRoom[i].H)
                                exceptionRoom = finishRoom[i];
                        }
                        current = exceptionRoom;
                        break;
                    }
                }
                else if (current.Equals(dest))
                {
                    break;
                }
            }
            while (!current.Equals(start))
            {
                current.previous.next = current;
                current = current.previous;
            }

            start.previous = null;
            return start;
        }
        return null;
    }

    public static void CalcRating(List<RoomInfo> arounds, RoomInfo start, RoomInfo current, RoomInfo dest)
    {
        if (arounds != null)
        {
            for (int i = 0; i < arounds.Count; i++)
            {
                var room = arounds[i];
                int priceFromDest = (Mathf.Abs(dest.center_Position.x - room.center_Position.x) + Mathf.Abs(dest.center_Position.y - room.center_Position.y)) * 10;

                if (room.previous == null)
                    room.previous = current;

                room.SetPrice(current.G + 10, priceFromDest);
            }
        }
    }

    public static RoomInfo GetNextRoomInfo(List<RoomInfo> arounds, RoomInfo current)
    {
        RoomInfo minValueRoom = null;

        for (int i = 0; i < arounds.Count; i++)
        {
            RoomInfo next = arounds[i];
            if (!next.isCheck)
            {
                if (minValueRoom == null)
                {
                    minValueRoom = next;
                }
                else if (minValueRoom.H > next.H)
                {
                    minValueRoom = next;
                }
            }
        }

        return minValueRoom;
    }

    public static RoomInfo GetPriorityRoom(List<RoomInfo> waittingRooms)
    {
        // 블럭 위치에 따른 가격이 제일 낮은 블럭을 반환다.
        RoomInfo room = null;
        var enumerator = waittingRooms.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (room == null || room.F < current.F)
            {
                room = current;
            }
        }

        return room;
    }
}