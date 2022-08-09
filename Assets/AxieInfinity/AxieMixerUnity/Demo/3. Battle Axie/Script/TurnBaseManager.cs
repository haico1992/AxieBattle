using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnBaseManager : MonoBehaviour
{
    private List<AxieBattleUnit> listBattleUnit;


    public void ExecuteTurnAI()
    {
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
        }
        
    }


}
