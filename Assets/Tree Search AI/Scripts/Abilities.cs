using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Abilities")]
public class Abilities : ScriptableObject
{
    public string name;
    public float health, mana;

    //Apply values to opponent and player
    public void RunAction(Player player, Player opponent) 
    {
        player.SetManaValue(mana);
        opponent.SetHealthValue(health);
    }
}
