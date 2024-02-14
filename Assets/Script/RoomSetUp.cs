using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetUp : MonoBehaviour
{
    public RoomStats room;
    public RoomSpaces[] roomSpaceList;
    public EnnemiesStats[] ennemyList;
    public RoomStats.Pnj pnj;
    public RoomStats.RoomType type;
    public enum RoomSpaces
    {
        Empty,
        Ennemy,
        PNJ
    }
    void Start()
    {
        RoomInit();
    }

    void RoomInit()
    {
        pnj = room.pnjType[Random.Range(0, room.pnjType.Length-1)];
        type = room.type;
        //room slots
        int ennemyToSpawn = Random.Range(room.ennemyNb.x, room.ennemyNb.y);
        ennemyList = new EnnemiesStats[ennemyToSpawn];

        roomSpaceList = new RoomSpaces[room.roomSpace];
        for (int i = 0; i < ennemyToSpawn; i++)
        {
            roomSpaceList[i] = RoomSpaces.Ennemy;

            //ennemy list
            ennemyList[i] = room.possibleEnnemies[Random.Range(0, room.possibleEnnemies.Length - 1)];
        }
        if(pnj != RoomStats.Pnj.None)
        {
            roomSpaceList[room.roomSpace - 1] = RoomSpaces.PNJ;
        }
        ShuffleSlot(roomSpaceList);
    }
    void ShuffleSlot(RoomSpaces[] slot)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < roomSpaceList.Length; t++)
        {
            RoomSpaces tmp = roomSpaceList[t];
            int r = Random.Range(t, roomSpaceList.Length);
            roomSpaceList[t] = roomSpaceList[r];
            roomSpaceList[r] = tmp;
        }
    }
}
