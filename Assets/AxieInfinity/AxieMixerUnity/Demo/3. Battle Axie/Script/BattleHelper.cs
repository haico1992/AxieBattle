using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using GluonGui.Dialog;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleHelper : MonoBehaviour
{
    public static List<BrickUnit> GetSurroundingSquares(BrickUnit brickUnit)
    {
       return GetSurroundingSquares(brickUnit.position);
    }
    
    public static List<BrickUnit> GetSurroundingSquares(Vector2 position)
    {
        List<BrickUnit> listResult = new List<BrickUnit>();
        for (int i = (int)position.x - 1; i < (int)position.x + 1; i++)
        {
            for (int j = (int)position.y - 1;j < (int)position.y + 1; j++)
            { 
                var brick = MapManager.GetBrickAtPosition(new Vector2(i, j));
                if(brick) listResult.Add(brick);
            }
        }

        listResult.Remove(MapManager.GetBrickAtPosition(position));
        return listResult;
    }

    public static BrickUnit GetBrickAtPosition(Vector2 position)
    {
        return MapManager.GetBrickAtPosition(position);
    }

    public static List<AxieUnit> GetSurroundingEnemies(BrickUnit brickUnit)
    {
        List<AxieUnit> listResult = new List<AxieUnit>();
        foreach (var brick in GetSurroundingSquares(brickUnit))
        {
            if (brick.battleUnit != null)
            {
                listResult.Add(brick.axieUnit);
            }
        }

        return listResult;
    }
    public static AxieUnit GetClosestEnemy(AxieUnit origin)
    {
        int      range  = Int32.MaxValue;
        AxieUnit result = null;
        foreach (var unit in MapManager.instance.listAllUnits)
        {
            if (Distance(origin, unit) <= range)
            {
                result = unit;
            }
        }

        return result;
    }

    public static int Distance(BrickUnit brickA, BrickUnit brickB)
    {
        var v = brickA.position - brickB.position;

        return (int)(Mathf.Abs((v.x)) + Mathf.Abs((v.y)));
    }
    
    public static int Distance(Vector2 positionA, Vector2 positionB)
    {
        var v = positionA - positionB;

        return (int)(Mathf.Abs((v.x)) + Mathf.Abs((v.y)));
    }
    
    public static int Distance(AxieUnit axieA, AxieUnit axieB)
    {
        var v = axieA.position - axieB.position;

        return (int)(Mathf.Abs((v.x)) + Mathf.Abs((v.y)));
    }

    public static List<AxieUnit> GetListUnitInRange(BrickUnit origin, int range)
    {
        return GetListUnitInRange(origin.axieUnit, range);
    }
    
    public static List<AxieUnit> GetListUnitInRange(AxieUnit axieUnit, int range)
    {
        List<AxieUnit> listResult = new List<AxieUnit>();
        foreach (var unit in MapManager.instance.listAllUnits)
        {
           
            if (Distance(axieUnit, unit) <= range)
            {
                listResult.Add(unit);
            }
        }

        return listResult;
    }

    public static int CalculateDamage()
    {
        int finalDamage = 0;
        int attackerNumber = Random.Range(0, 3);
        int defenderNumber = Random.Range(0, 3);
        int result         = attackerNumber - defenderNumber;
        switch (3+result%3)
        {
            case 0:
                finalDamage = 4;
                break;
            case 1:
                finalDamage = 5;
                break;
            case 2:
                finalDamage = 3;
                break;
        }

        return finalDamage;
    }
}
