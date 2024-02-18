using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesFunction : MonoBehaviour
{
    public void UpCrit(float crit)
    {
        Player.critRate += crit;
    }
    public void UpPary(float parry)
    {
        Player.paryBonus += parry;
        if(Player.paryBonus <0) Player.paryBonus = 0;
    }
    public void UpDamage(int damage)
    {
        Player.damage += damage;
        if (Player.damage <= 0) Player.damage = 1;
    }
    public void UpMaxHealth(int health)
    {
        Player.maxhealth += health;
        Player.health += health;
    }
    public void Heal(int heal)
    {
        Player.health += heal;
        if (Player.health > Player.maxhealth) Player.health = Player.maxhealth;
    }
    public void ParcheminListen(float time)
    {
        Player.doorTimeBonus += time;
    }
}
