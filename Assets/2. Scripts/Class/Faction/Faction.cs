using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FactionId
{
    A,
    B,
}
public class Faction : MonoBehaviour
{
    // 유저 id 이후 DB를 통해 배치할 것
    private int userId;
    private string userName;

    // 현재 스테이지 상에서의 faction 일반적으로 pve 상에서는 왼쪽이 플레이어
    [SerializeField] private FactionId factionId;
    public FactionId FactionId => factionId;
    
    private Faction enemyFaction;
    private List<CharacterBase> enemyOnStage = new List<CharacterBase>();
    public List<CharacterBase> EnemyOnStage  => enemyOnStage ;

    [SerializeField] private List<GameObject> partyPrefabs;
    private List<CharacterBase> unitOnStage = new List<CharacterBase>();
    public List<CharacterBase> UnitOnStage => unitOnStage;

    [SerializeField] private GameObject board;
    // private bool isBattle;

    // 테스트 용으로 OnEnable, Start 등에 뒀지만 이후 StageChange 등으로 분기할 것
    void OnEnable()
    {

    }

    void Start()
    {
        // Faction 선택
        var factions = GameObject.FindGameObjectsWithTag("Faction");
        foreach (var elem in factions)
        {
            var selected = elem.GetComponent<Faction>();
            if (selected.FactionId != this.factionId)
            {
                enemyFaction = selected;
            }
        }

        // 유닛 위치 설정 및 스폰
        Transform faction = null;
        if (factionId == FactionId.A)
        {
            faction = board.transform.Find("LeftFaction");
        }
        else
        {
            faction = board.transform.Find("RightFaction");
        }

        for (int i = 0; i < partyPrefabs.Count; i++)
        {
            var unit = partyPrefabs[i];

            var unitPosition = faction.GetChild(i);

            var instanceUnit = Instantiate(unit, unitPosition);
            instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
            unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
        }

        // 유닛들에게 팩션 주입
        foreach (var elem in unitOnStage)
        {
            elem.SetFaction(factionId);
        }

        // isBattle = true;
        // StartCoroutine(Battle());
    }

    // 스테이지 시작 시 전투 시작 코루틴
    private IEnumerator Battle()
    {
        yield break;
    }

    public void RemoveDiedUnit(CharacterBase diedUnit)
    {
        unitOnStage.Remove(diedUnit);
    }
}
