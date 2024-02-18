using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesFunction : MonoBehaviour
{
    public void ParcheminCrit(float crit, float parry)
    {
        Player.critRate += crit;
        Player.paryBonus += parry;
    }
    public void ParcheminDamage(int damage)
    {
        Player.damage += damage;
    }
    public void ParcheminVie(int health)
    {
        Player.maxhealth += health;
        Player.health += health;
    }
    public void ParcheminListen(float time)
    {
        Player.doorTimeBonus += time;
    }
}
