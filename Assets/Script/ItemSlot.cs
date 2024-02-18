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
                functions.UpCrit(0.1f);
                functions.UpPary(0.2f);
                break;
            case Upgrades.Upgrade.PierreCrit:
                functions.UpCrit(0.1f);
                break;
            case Upgrades.Upgrade.ParcheminDamage:
            case Upgrades.Upgrade.PierreDamage:
                functions.UpDamage(1);
                break;
            case Upgrades.Upgrade.ParcheminVie:
                functions.UpMaxHealth(1);
                break;
            case Upgrades.Upgrade.ParcheminListen:
                functions.ParcheminListen(1);
                break;
            case Upgrades.Upgrade.SakeSmall:
                functions.Heal(1);
                break;
            case Upgrades.Upgrade.SakeBig:
                functions.Heal(1);
                break;
            case Upgrades.Upgrade.PerleCrit:
                functions.UpCrit(0.1f);
                functions.UpPary(0.2f);
                functions.UpDamage(-1);
                break;
            case Upgrades.Upgrade.PerleDamage:
                Player.maxhealth = 1;
                Player.health = 1;
                functions.UpDamage(3);
                break;
            case Upgrades.Upgrade.PerleHealth:
                functions.UpMaxHealth(3);
                functions.UpPary(-0.2f);
                break;
            default:
                break;
        }
        transform.parent.gameObject.SetActive(false);
    }
}
