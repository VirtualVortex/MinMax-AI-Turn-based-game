using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class States : MonoBehaviour
{
    //Contains the abilitiy and the score calculated to it
    public Abilities ability;
    public Abilities bestAbility;
    public float health, mana, score, depth;
    public string name;
    public States parentNode;
}
