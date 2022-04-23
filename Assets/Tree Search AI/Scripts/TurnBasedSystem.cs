using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedSystem : MonoBehaviour
{
    [SerializeField] Player player1, player2;
    [SerializeField] TreeSearch treeSearch;
    [SerializeField] Text gameoverText, player1AbilityText, player2AbilityText;

    string winner;

    [HideInInspector]
    public enum battleState {player1turn, player2turn, gameover}
    [HideInInspector] public battleState curDecisionState;

    // Start is called before the first frame update
    void Start()
    {
        gameoverText.enabled = false;
        DecisionState(battleState.player1turn);
    }

    // Update is called once per frame
    void Update()
    {
        if (player1.health <= 0f)
        {
            DecisionState(battleState.gameover);
            winner = "player 2";
        }
        else if (player2.health <= 0f)
        {
            DecisionState(battleState.gameover);
            winner = "player 1";
        }
    }

    public void DecisionState(battleState state) 
    {
        //pick action based on state
        curDecisionState = state;
        switch (state)
        {
            case battleState.player1turn:
                StartCoroutine(SwitchPlayer(player1));
                player1AbilityText.text = "";
                break;
            case battleState.player2turn:
                StartCoroutine(SwitchPlayer(player2));
                player2AbilityText.text = "";
                break;
            case battleState.gameover:
                gameoverText.enabled = true;
                gameoverText.text = "Game Over: " + winner;
                break;
        }
    }

    public void ChooseAbility(Abilities ability, Player player, Player opponent)
    {
        //Display ability being used
        if (player == player1)
            player1AbilityText.text = ability.name;
        else if(player == player2)
            player2AbilityText.text = ability.name;

        //Aplly value to player rather than oppoent if values are beneficial
        if (ability.health > 0 || ability.mana > 0)
            ability.RunAction(player, player);
        else if(ability.health < 0)
            ability.RunAction(player, opponent);
        
        if (player1.health > 0 || player2.health > 0)
        {
            if (player == player1) DecisionState(battleState.player2turn);
            else if (player == player2) DecisionState(battleState.player1turn);
        }
    }

    //Wait three seconds so that user knows which agent is playing
    IEnumerator SwitchPlayer(Player player)
    {
        yield return new WaitForSeconds(3);
        if (player == player1) treeSearch.SetPlayer(player1);
        else if (player == player2) treeSearch.SetPlayer(player2);
    }
}
