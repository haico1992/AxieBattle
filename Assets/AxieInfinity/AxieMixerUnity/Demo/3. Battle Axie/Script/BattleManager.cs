using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void OnClickSpeedUpButton(float scale)
    {
        SetTimeScale(scale);
    }


}
