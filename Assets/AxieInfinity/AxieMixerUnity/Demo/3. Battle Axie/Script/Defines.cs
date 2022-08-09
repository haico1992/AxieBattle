using System.Collections;
using System.Collections.Generic;
using UnityEditor.Media;
using UnityEngine;

public class Defines : MonoBehaviour
{
    // Start is called before the first frame update
    public class SearchString
    {
        public const string AXIE_ID = "{ axie (axieId: \"{0}\") { id, genes, newGenes}}";
    }

    public class AxieAnimString
    {
        public const string Attack  = "attack/melee/horn-gore";
        public const string Defense = "defense/hit-by-normal";
        public const string Move    = "action/move-forward";
        public const string Idle    = "action/idle/normal";
    }
}
