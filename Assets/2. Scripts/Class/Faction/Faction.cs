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

    [SerializeField] private List<GameObject> partyPrefabs;
    public List<CharacterBase> unitOnStage = new List<CharacterBase>();

    [SerializeField] private GameObject board;
    private Queue<CharacterBase> attackQueue = new Queue<CharacterBase>();
    private bool isBattle;

    // 공격 속도 제어
    // 스피드 1 = 0.25초의 딜레이 경감을 가짐
    private float delayBySpeed = 0.25f;
    private float minDelay = 0.25f;
    private float maxDelay = 2.5f;

    // 테스트 용으로 OnEnable, Start 등에 뒀지만 이후 StageChange 등으로 분기할 것
    void OnEnable()
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
        Transform formation = null;
        if (factionId == FactionId.A)
        {
            formation = board.transform.Find("LeftFormation");
        }
        else
        {
            formation = board.transform.Find("RightFormation");
        }

        for (int i = 0; i < partyPrefabs.Count; i++)
            {
                var unit = partyPrefabs[i];

                var unitPosition = formation.GetChild(i);

                var instanceUnit = Instantiate(unit, unitPosition);
                instanceUnit.transform.SetPositionAndRotation(unitPosition.position, unitPosition.rotation);
                unitOnStage.Add(instanceUnit.GetComponent<CharacterBase>());
            }

        // speed에 따라 정렬해서 Queue에 넣기
        unitOnStage.Sort((a, b) => b.CurrentSpeed.CompareTo(a.CurrentSpeed));
        foreach (var elem in unitOnStage)
        {
            if (factionId == FactionId.A)
            {
                elem.formation = false;
            }
            else
            {
                elem.formation = true;
            }
            elem.SetFormation(factionId != FactionId.A);
            attackQueue.Enqueue(elem);
        }
    }

    void Start()
    {
        isBattle = true;
        StartCoroutine(Battle());
    }

    // 스테이지 시작 시 전투 시작 코루틴
    private IEnumerator Battle()
    {
        while (isBattle)
        {
            var enemies = enemyFaction.unitOnStage;
            if (enemies == null || enemies.Count == 0) { isBattle = false; break; }

            // 공격자 설정
            CharacterBase attacker = null;
            while (attackQueue.Count > 0)
            {
                var candidate = attackQueue.Dequeue();
                if (candidate != null && candidate.CanAttack)
                {
                    attacker = candidate;
                    break;
                }
            }

            if (attacker == null)
            {
                isBattle = false;
                break;
            }

            // 타겟 선정
            CharacterBase target = SetTarget(enemies, attacker);
            if (target == null)
            {
                Debug.Log("BattleEnd");
                break;
            }

            float wait = GetDelay(attacker.CurrentSpeed);
            yield return new WaitForSeconds(wait);

            // 사망한 경우 체크
            if (!attacker.IsAlive) continue;
            if (!target.IsAlive) continue;

            // 공격 및 데미지 적용
            yield return attacker.StartCoroutine(attacker.Attack(target));
            if (attacker.IsAlive && target.IsAlive)
            {
                GameManager.gameManager.ApplyDamage(attacker, target);
            }
            if (attacker.IsAlive && attacker.CanAttack)
            {
                attackQueue.Enqueue(attacker);
            }
        }
    }

    // 적 중 타겟 지정
    private CharacterBase SetTarget(List<CharacterBase> enemies, CharacterBase attacker)
    {
        var selectedTarget = new List<CharacterBase>();

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemy = enemies[i].GetComponent<CharacterBase>();
            if (enemy != attacker && enemy.CanBeTarget)
            {
                selectedTarget.Add(enemy);
            }
        }
        if (selectedTarget.Count != 0)
        {
            return selectedTarget[Random.Range(0, selectedTarget.Count)];
        }
        else
        {
            return null;
        }
    }

    // 스피드에 따른 딜레이 처리
    private float GetDelay(int speed)
    {
        speed = Mathf.Max(0, speed);
        float delay = maxDelay - (speed * delayBySpeed);
        return Mathf.Max(minDelay, delay);
    }
}
