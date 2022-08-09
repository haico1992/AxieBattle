using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public static MapManager instance;
    // Start is called before the first frame update
    public         GameObject      brickUnit;
    public         int             width  = 10;
    public         int             height = 10;
    private        GameObject      mapObject;
    private static List<BrickUnit> listAllBrick;

    public List<AxieUnit> listAllUnits
    {
        get
        {
            return GameSceneManager.instance.listAllUnit;
        }
    }

    private static List<AxieUnit>  _listAllUnits;
    void Awake()
    {
        instance = this;
    }
    public void GenerateMap (int width, int height, Transform parent)
    {
        listAllBrick = new List<BrickUnit>();
        for(int i =0 ; i< width;i++){
            for (int j = 0; j < height; j++)
            {
                var brick=LayBrickByPosition(new Vector2(i, j), parent);
                if ((i + j) % 2 == 0)
                {
                    brick.GetComponent<BrickUnit>().SetType(BrickType.B);
                }
                listAllBrick.Add(brick.GetComponent<BrickUnit>());
            }
        }
    }

    GameObject LayBrickByPosition(Vector2 position, Transform parent)
    {
        var brickObj = Instantiate(this.brickUnit,position, Quaternion.identity,parent);
        brickObj.name = position.ToString();
        return brickObj;
    }
    /// <summary>
    /// return Brick unit at position or null if not found.
    /// OPTIMIZABLE
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    static public BrickUnit GetBrickAtPosition(Vector2 position)
    {
        BrickUnit result = null;
        foreach (var brick in listAllBrick)
        {
            if (brick.position.Equals(position))
            {
                result = brick;
            }
        }

        return result;
    }

    void Start()
    {
        mapObject = new GameObject("MapBricks");
        GenerateMap(width,height,mapObject.transform);
    }
}

