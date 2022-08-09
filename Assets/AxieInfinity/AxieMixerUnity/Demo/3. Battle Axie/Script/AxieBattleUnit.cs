using System.Collections;
using System.Collections.Generic;
using Game;
using Spine.Unity;
using UnityEngine;

public class AxieBattleUnit : MonoBehaviour
{
    [SerializeField] private AxieFigure        axieFigure;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    public                   AxieUnit          axieUnit;
    public                   bool              facingRight = true;
    public                   int               teamIndex;
    AxieUnit                                   targetAxie     = null;
    BrickUnit                                  targetBrick    = null;
    private int                                damageThisTurn = 0;
    public  bool                               isDead         = false;
    public void SetAxieAnimation(string anim)
    {
        this.skeletonAnimation.AnimationName = anim;
    }

    public void SetupAxieByGene(string id, string geneString)
    {
        this.axieFigure.SetGenes(id,geneString);
    }
    
  
    private List<AxieBattleUnit> attackingUnit = new List<AxieBattleUnit>();
    public void GetDamagedByUnit(AxieBattleUnit unit, int damage)
    {
        attackingUnit.Add(unit);
        this.damageThisTurn += damage;
    }
    public void MakeDecision()
    {
       
            
        
        //detect surrounding enemy
        var      enemiesNearBy = this.DetectNearByEnemy();
        if (enemiesNearBy.Count > 0)
        {
            targetAxie = enemiesNearBy[0];
        }
        else if (DetectMoveablePosition().Count>0)
        {
            var closestTarget = this.DetectClosestEnemy();
            targetBrick = MoveByDirection(this.axieUnit.position,closestTarget.position - this.axieUnit.position);
            //Move
            return;
        }
        else
        {
            //idle
            
        }
              
        
    }

    public void Init()
    {
        this.axieUnit.currentHealth = this.axieUnit.health;
    }
    public void DealDamage()
    {
        if (this.targetAxie != null)
        {
            int damage = BattleHelper.CalculateDamage();
            this.targetAxie.battleUnit.GetDamagedByUnit(this,damage);
        }
    }

    public void ResolveDamage()
    {
        if (this.damageThisTurn > 0)
        {
            axieUnit.currentHealth -= this.damageThisTurn;
            this.damageThisTurn    =  0;
        }

        this.isDead = this.axieUnit.currentHealth > 0;
    }
    public void ExecuteAnimation()
    {
       
    }
    
    public virtual List<AxieUnit> DetectNearByEnemy()
    {
        List<AxieUnit> result     = new List<AxieUnit>();
        var            unitNearBy = BattleHelper.GetListUnitInRange(this.axieUnit,1);
        foreach (var unit in unitNearBy)
        {
            if (unit.battleUnit.teamIndex != this.teamIndex)
            {
                result.Add(unit);
            }
        }

        return result;
    }

    public AxieUnit DetectClosestEnemy()
    {
       return  BattleHelper.GetClosestEnemy( this.axieUnit);
    }

    public BrickUnit MoveByDirection(Vector2 origin, Vector2 direction)
    {
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {

            direction = new Vector2(direction.x / Mathf.Abs(direction.x), 0);
            
        }
        else
        {
            direction = new Vector2(0,direction.y / Mathf.Abs(direction.y));

        }




        return BattleHelper.GetBrickAtPosition(origin + direction* this.axieUnit.mobility);
    }

    public virtual List<BrickUnit> DetectMoveablePosition()
    {
        List<BrickUnit> result     = new List<BrickUnit>();
        var             squareNearBy = BattleHelper.GetSurroundingSquares(this.axieUnit.position);
        foreach (var unit in squareNearBy)
        {
            if(BattleHelper.Distance(unit.position,this.axieUnit.position)<=this.axieUnit.mobility){
                result.Add(unit);
            }
        }

        return result;
    }
}

public class AxieUnit
{
    public int            health;
    public int            currentHealth;
    public AxieCombatType type;
    public AxieBattleUnit battleUnit;
    public Vector2        position { get; }
    public int            mobility = 1 ;
    
    public enum AxieCombatType
    {
        attacker,
        defender
    }

    
}
