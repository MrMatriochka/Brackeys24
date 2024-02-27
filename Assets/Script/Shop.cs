using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shop : MonoBehaviour
{
    [SerializeField] PnjShop[] allShopType;
    PnjShop shopType;
    public RoomSetUp room;
    [SerializeField] GameObject[] ItemSlot;
    [SerializeField] Image pnjImage;
    MusicManager music;
    void Start()
    {
        shopType = room.pnj;
        music = FindAnyObjectByType<MusicManager>();
        music.ChangeClip(shopType.music);
        DisplayShop();
    }

    void DisplayShop()
    {
        foreach (GameObject item in ItemSlot)
        {
            Upgrades up = shopType.itemList[Random.Range(0, shopType.itemList.Length)];
            item.GetComponent<ItemSlot>().upgrade = up;
            item.GetComponent<ItemSlot>().SetUp();
            pnjImage.sprite = shopType.sprite;
        }
    }

    
}
