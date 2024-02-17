using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image image;
    [HideInInspector] public Upgrades upgrade;
    UpgradesFunction functions;
    private void Start()
    {
        functions = transform.parent.GetComponent<UpgradesFunction>();
    }
    public void SetUp()
    {
        image.sprite = upgrade.sprite;
    }
    public void Buy()
    {
        switch (upgrade.upgardeFunction)
        {
            case Upgrades.Upgrade.ParcheminCrit:
                functions.ParcheminCrit(0.1f, 1);
                break;
            case Upgrades.Upgrade.ParcheminDamage:
                functions.ParcheminDamage(1);
                break;
            case Upgrades.Upgrade.ParcheminVie:
                functions.ParcheminVie(1);
                break;
            case Upgrades.Upgrade.ParcheminListen:
                functions.ParcheminListen(1);
                break;
            default:
                break;
        }
        transform.parent.gameObject.SetActive(false);
    }
}
