using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using Spine.Unity;
using UnityEngine;

public class AxieBattleUnit : MonoBehaviour
{
    [SerializeField] private AxieFigure        axieFigure;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private HealthBarManager  healthBar;
    public                   AxieUnit          axieUnit;
    public                   bool              facingRight = true;
    public                   int               teamIndex;

    public BrickUnit currentBrick
    {
        get{
            return this._currentBrick;
        }
        set
        {
            if(this._currentBrick!=null) this._currentBrick.UpdateCurrentAxie(null);
            this._currentBrick = value;
            if(this._currentBrick!=null) this._currentBrick.UpdateCurrentAxie(this.axieUnit);
        }
    }
    AxieUnit          targetAxie  = null;
    private BrickUnit targetBrick = null;
    private BrickUnit _currentBrick   = null;
    private int       damageThisTurn = 0;
    public  bool      isDead         = false;
    public  void      SetAxieAnimation(string anim, bool loop = false)
    {
        this.skeletonAnimation.state.ClearTrack(1);
        this.skeletonAnimation.state.SetAnimation(1, anim, loop).Delay=0.3f;
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
        if (this.axieUnit.currentHealth <= 0)
        {
            isDead = true;
            return;
            //idle

        }
        //detect surrounding enemy
        var      enemiesNearBy = this.DetectNearestEnemy();
        if (enemiesNearBy.Count > 0)
        {
            targetAxie = enemiesNearBy[0];
        }
        else if (DetectMoveablePosition().Count > 0)
        {
            var closestTarget = this.DetectClosestEnemy();
            if (closestTarget == null) return;
            var direction = closestTarget.position - this.axieUnit.position;
            targetBrick      = MoveByDirection(this.axieUnit.position,direction);
            this.facingRight = direction.x > 0;
            //Move
            return;
        }
              
        
    }

    public void Init()
    {
        this.axieUnit.currentHealth     = this.axieUnit.health;
        this.healthBar.maxHealth        = this.axieUnit.health;
        this.healthBar.currentHealth    = this.axieUnit.currentHealth;
        
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
        }

        this.axieUnit.currentHealth = Mathf.Max(this.axieUnit.currentHealth, 0);
        this.isDead                 = this.axieUnit.currentHealth <= 0;
    }


    public void EndTurn()
    {
        
        this.damageThisTurn = 0;
        targetAxie          = null;
        this.targetBrick    = null;
    }
    
    public void ExecuteAnimation()
    {
        StartCoroutine((this.ExecuteAnimationFlowCorroutine()));
    }

    public void ExecuteUIAnimation()
    {
        StartCoroutine((this.ExecuteUIAnimationCoroutine()));
    }
    IEnumerator ExecuteUIAnimationCoroutine()
    {
    
        int newHealth = this.axieUnit.currentHealth;
        newHealth = Mathf.Max(0, newHealth);
        while (this.healthBar.currentHealth!= newHealth)
        {
          
            this.healthBar.currentHealth = (int)Mathf.Lerp(   this.healthBar.currentHealth, newHealth, 0.01f);
            yield return new WaitForEndOfFrame();
        }
      
    }

    IEnumerator ExecuteAnimationFlowCorroutine()
    {
        if (isDead) //dead
        {
            this.SetAxieAnimation(Defines.AxieAnimString.Dead);
        }
        else
        {

            this.axieFigure.flipX = this.facingRight;
            //idle
            if (this.targetAxie == null && this.targetBrick == null)
            {
                this.SetAxieAnimation(Defines.AxieAnimString.Idle, true);
            }

            //Move
            if (this.targetBrick != null)
            {

                this.SetAxieAnimation(Defines.AxieAnimString.Move);
                this.currentBrick = this.targetBrick;
                MoveToBrick(this.currentBrick);

            }

            //Attack
            if (this.targetAxie != null)
            {
                this.SetAxieAnimation(Defines.AxieAnimString.Attack);
            }
        }

        //dead{}
        yield return new WaitForEndOfFrame();
    }

    public virtual List<AxieUnit> DetectNearestEnemy()
    {
        List<AxieUnit> result     = new List<AxieUnit>();
        var            unitNearBy = BattleHelper.GetListUnitInRange(this.axieUnit,1);
        foreach (var unit in unitNearBy)
        {
            if (unit.battleUnit.teamIndex != this.teamIndex && !unit.battleUnit.isDead)
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
            if(BattleHelper.Distance(unit.position,this.axieUnit.position) <= this.axieUnit.mobility &&  unit.axieUnit == null){
                result.Add(unit);
            }
        }

        return result;
    }

    public void SnapToCurrentBrick()
    {
        this.transform.position = currentBrick.transform.position;
    }
    public void MoveToBrick(BrickUnit brick)
    {

        this.transform.DOMove(brick.transform.position, 1f);
    }
}

public class AxieUnit
{
    public int            health;
    public int            currentHealth;
    public AxieCombatType type;
    public AxieBattleUnit battleUnit;

    public Vector2 position
    {
        get
        {
            return this.battleUnit.currentBrick.position;
        }
    }

    public int            mobility = 1 ;
    
    public enum AxieCombatType
    {
        attacker,
        defender
    }

    
}
