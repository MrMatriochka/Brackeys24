using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public static WorldGeneration instance;
    public static RoomStats[] roomList;
    public static int playerProgression=0;
    public static Dictionary<RoomStats.Pnj,bool> pnjStatus = new Dictionary<RoomStats.Pnj, bool>();
    [System.Serializable]
    public struct ZoneComposition
    {
        public int roomNb;
        public RoomStats[] possibleRooms;
    }
    [SerializeField] ZoneComposition[] zoneCompo;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        if (playerProgression == 0)
        {
            GenerateWorld();
            InitPnjStatus();
        }
        
    }
    void GenerateWorld()
    {
        
        int totalRooms = 0;
        foreach (ZoneComposition zone in zoneCompo)
        {
            totalRooms += zone.roomNb;
        }
        roomList = new RoomStats[totalRooms];
        int id = 0;
        foreach (ZoneComposition zone in zoneCompo)
        {
            for (int i = 0; i < zone.roomNb; i++)
            {
                roomList[id] = zone.possibleRooms[Random.Range(0, zone.possibleRooms.Length)];
                id++;
            }
        }
    }

    void InitPnjStatus()
    {
        if(pnjStatus.Count != 0) pnjStatus.Clear();

        int pnjTypes = System.Enum.GetValues(typeof(RoomStats.Pnj)).Length;
        for (int i = 0; i < pnjTypes; i++)
        {
            pnjStatus.Add((RoomStats.Pnj)i, true );
        }
    }
}
