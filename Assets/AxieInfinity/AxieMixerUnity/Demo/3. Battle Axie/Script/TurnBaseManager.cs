using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBaseManager : MonoBehaviour
{
    private       List<AxieBattleUnit> listBattleUnit;
    public static TurnBaseManager      instance;
    public        float                turnTime = 2f;

    void Awake()
    {
        instance = this;
    }

    public void ExecuteTurnAI()
    {
        listBattleUnit = GameSceneManager.instance.listAllAxieBattleUnit;
        foreach (var unit in this.listBattleUnit) // attack
        {
            unit.MakeDecision();
        }
        
        foreach (var unit in this.listBattleUnit) // Calculate damage after all decision are made
        {
            unit.DealDamage();
        }
        
        foreach (var unit in this.listBattleUnit) // Calculate damage after all decision are made
        {
            unit.ResolveDamage();
        }
        
        foreach (var unit in this.listBattleUnit) // select animation based on result
        {
            unit.ExecuteAnimation();
            unit.ExecuteUIAnimation();
        }
        foreach (var unit in this.listBattleUnit) // select animation based on result
        {
            unit.EndTurn();
        }
        
    }

    public void StartTurnLoop()
    {
        InvokeRepeating("ExecuteTurnAI",0,turnTime);
    }


}
