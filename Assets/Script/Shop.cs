using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shop : MonoBehaviour
{
    public PnjShop shopType;
    public GameObject[] ItemSlot;
    UpgradesFunction functions;
    void Start()
    {
        functions = GetComponent<UpgradesFunction>();
        DisplayShop();
    }

    void DisplayShop()
    {

    }

    public void Buy()
    {
        functions.ParcheminCrit(1,1);
    }
}
