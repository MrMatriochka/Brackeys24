using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetUp : MonoBehaviour
{
    RoomStats room;
    [HideInInspector] public RoomSpaces[] roomSpaceList;
    [HideInInspector]public List<EnnemiesStats> ennemyList;
    [HideInInspector] public RoomStats.Pnj pnj;
    [HideInInspector] public RoomStats.RoomType type;

    public enum RoomSpaces
    {
        Empty,
        Ennemy,
        PNJ
    }
    void Awake()
    {
        room = WorldGeneration.roomList[WorldGeneration.playerProgression];
        RoomInit();
    }

    void RoomInit()
    {
        pnj = room.pnjType[Random.Range(0, room.pnjType.Length)];
        if(WorldGeneration.pnjStatus[pnj] == false)
        {
            pnj = RoomStats.Pnj.None;
        }


        type = room.type;
        //room slots
        int ennemyToSpawn = Random.Range(room.ennemyNb.x, room.ennemyNb.y);

        roomSpaceList = new RoomSpaces[room.roomSpace];
        for (int i = 0; i < ennemyToSpawn; i++)
        {
            roomSpaceList[i] = RoomSpaces.Ennemy;

            //ennemy list
            ennemyList.Add(room.possibleEnnemies[Random.Range(0, room.possibleEnnemies.Length)]);
        }
        if(pnj != RoomStats.Pnj.None)
        {
            roomSpaceList[room.roomSpace - 1] = RoomSpaces.PNJ;
        }
        ShuffleSlot(roomSpaceList);
    }
    void ShuffleSlot(RoomSpaces[] slot)
    {
        for (int t = 0; t < roomSpaceList.Length; t++)
        {
            RoomSpaces tmp = roomSpaceList[t];
            int r = Random.Range(t, roomSpaceList.Length);
            roomSpaceList[t] = roomSpaceList[r];
            roomSpaceList[r] = tmp;
        }
    }
}
