using System;
using System.Collections;
using System.Collections.Generic;
using AxieMixer.Unity;
using Game;
using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager instance;
    public  int              attackerCount = 10;
    public  int              defenderCount = 5;

    public  List<AxieBattleUnit> listDefender = new List<AxieBattleUnit>();
    public  List<AxieBattleUnit> ListAttacker = new List<AxieBattleUnit>();
    public  string               attackerID   = "";
    public  string               defenderID   = "";
    [SerializeField] 
    private AxieBattleUnit unitPrefab;

    public List<AxieUnit> listAllAxieUnit
    {
        get
        {
            List<AxieUnit> result = new List<AxieUnit>();
            foreach (var unit  in this.listDefender)
            {
                result.Add(unit.axieUnit);
            }
            foreach (var unit  in this.ListAttacker)
            {
                result.Add(unit.axieUnit);
            }

            return result;
        }
    }
    
    public List<AxieBattleUnit> listAllAxieBattleUnit
    {
        get
        {
            List<AxieBattleUnit> result = new List<AxieBattleUnit>();
            foreach (var unit  in this.listDefender)
            {
                result.Add(unit);
            }
            foreach (var unit  in this.ListAttacker)
            {
                result.Add(unit);
            }

            return result;
        }
    }
    IEnumerator RequestGenes(UnityAction OnFinishAction = null)
    {
        var defenderRequest = StartCoroutine(GetAxiesGenes(defenderID,this.OnAxieDefenderGeneRequest));
        var attackerRequest = StartCoroutine(GetAxiesGenes(attackerID,this.OnAxieAttackerGeneRequest));
        yield return defenderRequest;
        yield return attackerRequest;
        if(OnFinishAction!=null) OnFinishAction.Invoke();
    }

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        Mixer.Init();
        InitGamePlay();
    }
    [Button]
    public void InitGamePlay()
    {
        MapManager.instance.Init();
        this.StartCoroutine(this.RequestGenes(AfterInit));
    }

    void AfterInit()
    {
        DistributeAxie();
        TurnBaseManager.instance.StartTurnLoop();
    }

    public void DistributeAxie()
    {
        var listAllPossibleSlot      = new List<BrickUnit>(MapManager.listAllBrick);
        var listAllUndistributedAxie = new List<AxieUnit>(listAllAxieUnit);

        foreach (var axieUnit in listAllUndistributedAxie)
        {
            var selectedSlot = Random.Range(0, listAllPossibleSlot.Count);
            axieUnit.battleUnit.currentBrick = listAllPossibleSlot[selectedSlot];
            axieUnit.battleUnit.SnapToCurrentBrick();
            listAllPossibleSlot.RemoveAt(selectedSlot);
        }
        
    }


    public IEnumerator GetAxiesGenes(string axieId, UnityAction<string,string> callback )
    {
        string  searchString =  "{ axie (axieId: \"" + axieId + "\") { id, genes, newGenes}}";
        JObject jPayload     = new JObject();
        jPayload.Add(new JProperty("query", searchString));

        var    wr         = new UnityWebRequest("https://graphql-gateway.axieinfinity.com/graphql", "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jPayload.ToString().ToCharArray());
        wr.uploadHandler   = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        wr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        wr.SetRequestHeader("Content-Type", "application/json");
        wr.timeout = 10;
        yield return wr.SendWebRequest();
        if (wr.error == null)
        {
            var result = wr.downloadHandler != null ? wr.downloadHandler.text : null;
            if (!string.IsNullOrEmpty(result))
            {
                JObject jResult  = JObject.Parse(result);
                string  genesStr = (string)jResult["data"]["axie"]["newGenes"];
                callback(axieId,genesStr);
            }
        }
    }

    void OnAxieAttackerGeneRequest(string id, string genesCode)
    {
        GenerateAttacker(id, genesCode);
    }
    void OnAxieDefenderGeneRequest(string id, string genesCode)
    {
        GenerateDefender(id, genesCode);
    }

    void GenerateDefender(string id, string gene)
    {
        if (unitPrefab == null) return;
        for (int i = 0; i < this.defenderCount; i++)
        {
            var axieObj  = Instantiate(this.unitPrefab);
            var axieUnit = axieObj.GetComponent<AxieBattleUnit>();
            axieUnit.axieUnit               = new AxieUnit();
            axieUnit.axieUnit.type          = AxieUnit.AxieCombatType.defender;
            axieUnit.axieUnit.battleUnit    = axieUnit;
            axieUnit.axieUnit.mobility      = 0;
            axieUnit.teamIndex              = (int)AxieUnit.AxieCombatType.defender;
            axieUnit.axieUnit.health        = Defines.DefaultValues.DefenderHealth;
            axieUnit.axieUnit.currentHealth = Defines.DefaultValues.DefenderHealth;
            axieUnit.Init();
            this.listDefender.Add((axieUnit));
            axieUnit.SetupAxieByGene(id,gene);
        }
    }
    
    void GenerateAttacker(string id, string gene)
    {
        if (unitPrefab == null) return;
        for (int i = 0; i < this.attackerCount; i++)
        {
            var axieObj  = Instantiate(this.unitPrefab);
            var axieUnit = axieObj.GetComponent<AxieBattleUnit>();
            axieUnit.axieUnit               = new AxieUnit();
            axieUnit.axieUnit.type          = AxieUnit.AxieCombatType.attacker;
            axieUnit.axieUnit.battleUnit    = axieUnit;
            axieUnit.axieUnit.mobility      = 1;
            axieUnit.teamIndex              = (int)AxieUnit.AxieCombatType.attacker;
            axieUnit.axieUnit.health        = Defines.DefaultValues.AttackerHealth;
            axieUnit.axieUnit.currentHealth = Defines.DefaultValues.AttackerHealth;
            axieUnit.Init();
            this.ListAttacker.Add((axieUnit));
            axieUnit.SetupAxieByGene(id,gene);
        }
    }

}
