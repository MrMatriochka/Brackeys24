using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemSlot : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text description;

    [HideInInspector] public Upgrades upgrade;
    UpgradesFunction functions;
    private void Start()
    {
        functions = transform.parent.parent.GetComponent<UpgradesFunction>();
    }
    public void SetUp()
    {
        nameText.text = upgrade.displayName;
        description.text = upgrade.description.Replace("<br>", "\n");
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
                functions.ParcheminListen(5);
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
