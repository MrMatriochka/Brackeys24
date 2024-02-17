using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shop : MonoBehaviour
{
    public PnjShop[] allShopType;
    PnjShop shopType;
    public RoomSetUp room;
    public GameObject[] ItemSlot;

    void Start()
    {
        InitShopType();
        DisplayShop();
    }

    void InitShopType()
    {
        foreach (PnjShop shop in allShopType)
        {
            if(shop.type == room.pnj)
            {
                shopType = shop;
                return;
            }
        }
    }
    void DisplayShop()
    {
        foreach (GameObject item in ItemSlot)
        {
            Upgrades up = shopType.itemList[Random.Range(0, shopType.itemList.Length)];
            item.GetComponent<ItemSlot>().upgrade = up;
            item.GetComponent<ItemSlot>().SetUp();
        }
    }

    
}
