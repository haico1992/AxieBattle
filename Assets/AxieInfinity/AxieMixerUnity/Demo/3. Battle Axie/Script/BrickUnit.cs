 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickUnit : MonoBehaviour
{
    public  BrickType      type = BrickType.A;
    public  Vector2        position { get { return _position; } }
    private Vector2        _position;
    public  AxieUnit       axieUnit = null;
    public  AxieBattleUnit battleUnit;
    public void SetType(BrickType type)
    {
        switch (type)
        {
            case BrickType.B :
                this.GetComponent<SpriteRenderer>().color = Color.gray;
                break;
            case BrickType.A :
                this.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            default:
                this.GetComponent<SpriteRenderer>().color = Color.white;
                break;
        }

        this.type = type;
    }


}
 public enum BrickType
 {
       A,
       B
 }
