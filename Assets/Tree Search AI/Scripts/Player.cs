using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float health, mana;
    public Abilities[] abilities;

    [SerializeField] Text healthUI, manaUI;

    // Start is called before the first frame update
    void Start()
    {
        healthUI.text = "Health: " + health.ToString();
        manaUI.text = "Mana: " + mana.ToString();
    }
    
    //Uses + to increase or decrease mana based on positive or negative number
    public void SetManaValue(float value)
    {
        mana += value;
        manaUI.text = "Mana: " + mana.ToString();
    }

    //Uses + to increase or decrease health based on positive or negative number
    public void SetHealthValue(float value)
    {
        health += value;
        healthUI.text = "Health: " + health.ToString();
    }
}
