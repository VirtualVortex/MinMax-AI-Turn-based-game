using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeSearch : MonoBehaviour
{
    [SerializeField] bool usePruning;
    [SerializeField] int depthSearch;
    [SerializeField] Player player1, player2;

    Player opponint;
    Player curPlayer;
    TurnBasedSystem turnBasedSystem;
    TreeGraph treeGraph;

    // Start is called before the first frame update
    void Start()
    {
        turnBasedSystem = GetComponent<TurnBasedSystem>();
        treeGraph = GetComponent<TreeGraph>();
    }

    public void SetPlayer(Player player)
    {
        curPlayer = player;
        
        if (player1 != player) opponint = player1;
        else if (player2 != player) opponint = player2;

        //Setting up root node state
        States state = new States();
        state.name = "root";
        state.health = opponint.health;
        state.mana = curPlayer.mana;
        state.parentNode = state;

        //if the game isn't over continue to make decisions
        if (turnBasedSystem.curDecisionState != TurnBasedSystem.battleState.gameover)
        {
            States rootState = MiniMax(state, depthSearch, -Mathf.Infinity, Mathf.Infinity, true);
            string dictKey = ((depthSearch+1).ToString() + " previous node: " + state.name + " score " + rootState.score.ToString());
            treeGraph.BuildTree(dictKey, rootState);
            turnBasedSystem.ChooseAbility(rootState.bestAbility, curPlayer, opponint);
            treeGraph.BuildGraph(depthSearch);
            treeGraph.nodes.Clear();
            StartCoroutine(treeGraph.DestroyGraphNode());
        }
    }
    
    States  MiniMax(States state, int depth, float alpha, float beta, bool maximizingPlayer) 
    {
        States action = new States();
        
        //leaf node, calculate evaluation
        if (depth <= 0f)
        {
            States evalState = new States();

            //Calculate evaluation player health - opponent's health after applied damage
            float score = Evaluation((curPlayer.health + curPlayer.mana), (state.health + state.ability.health) + state.mana);

            evalState.health = state.health;
            evalState.mana = state.mana;
            evalState.ability = state.ability;
            
            evalState.score = score;
            evalState.parentNode = state;
            evalState.name = state.ability.name;

            action = evalState;
            
            return evalState;
        }
        else
        {
            float value = maximizingPlayer ? -Mathf.Infinity : Mathf.Infinity;
            float evalValue = 0;
            List<States> valueList = new List<States>();
            Abilities[] abilitiesArray = PossibleAbilities(curPlayer.abilities, state.mana);
            States curState = new States();

            //setting up new state for mini max
            curState.health = state.health;
            curState.mana = state.mana;
            curState.ability = state.ability;

            //Cycle through each possible ability to ensure agents only use what it viable to them
            foreach (Abilities ability in abilitiesArray)
            {
                States returnState = new States();
                //state parameter for next level
                curState.ability = ability;
                curState.health += ability.health;
                curState.mana += ability.mana;
                curState.name = ability.name;
                curState.parentNode = state;

                //return score from child node
                if(maximizingPlayer) returnState = MiniMax(curState, depth - 1, alpha, beta, false);
                else if (!maximizingPlayer) returnState = MiniMax(curState, depth - 1, alpha, beta, true);

                evalValue = returnState.score;
                returnState.parentNode = curState;
                returnState.name = curState.name;

                string dictKey = (depth.ToString() + " previous node: " + state.name + " " + ability.name + " score " + returnState.score);
                treeGraph.BuildTree(dictKey, returnState);

                //Picking the largest value when maximizing ignore inifinity to prevent the wrong action from being picked
                if (evalValue > value && evalValue != -Mathf.Infinity && evalValue != Mathf.Infinity && maximizingPlayer)
                {
                    value = evalValue;
                    curState.bestAbility = ability;
                    valueList.Clear();
                }
                else if (evalValue < value && evalValue != Mathf.Infinity && evalValue != -Mathf.Infinity && !maximizingPlayer) 
                {
                    //Picking the smallest value when maximizing ignore inifinity to prevent the wrong action from being picked
                    value = evalValue;
                    curState.bestAbility = ability;
                    valueList.Clear();
                }

                //Add to list of nodes with equal score for random pick
                if (evalValue == value && evalValue != -Mathf.Infinity && evalValue != Mathf.Infinity)
                {
                    States tempState = new States();
                    tempState.score = evalValue;
                    tempState.ability = ability;
                    valueList.Add(tempState);
                }
                
                if (usePruning && maximizingPlayer)
                {
                    alpha = Mathf.Max(alpha, evalValue);
                    if (beta <= alpha)
                        break;
                }
                else if (usePruning && !maximizingPlayer)
                {
                    beta = Mathf.Min(beta, evalValue);
                    if (beta <= alpha)
                        break;
                }
            }

            //Incase node doesn't reach evaluation level
            if (value == -Mathf.Infinity || value == Mathf.Infinity) value = 0;

            //If level has node with the same score pick ability at random
            if (valueList.Count == 0) curState.score = value;
            else if (valueList.Count > 0)
            {
                States randomState = valueList[Random.Range(0, valueList.Count)];
                curState.score = randomState.score;
                curState.bestAbility = randomState.ability;
            }
            
            return curState;
            
        }
        return action;
    }

    //Does the agent have enought mana to use ability
    Abilities[] PossibleAbilities(Abilities[] abilities, float mana) 
    {
        List<Abilities> possibleAbilities = new List<Abilities>();
        
        foreach (Abilities ability in abilities)
        {
            //see if ability is possible to use
            if (Mathf.Abs(ability.mana) <= mana && !possibleAbilities.Contains(ability))
            {
                possibleAbilities.Add(ability);
            }
        }

        return (possibleAbilities.ToArray());
    }

    float Evaluation(float player, float opponent)
    {
        return (player - opponent);
    }
}
