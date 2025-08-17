using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CommandingPlayer : MonoBehaviour
{
    private int userId;
    private string userName;

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


    // 싱글톤 선언
    public static CommandingPlayer commandingPlayer = null;
    void Awake()
    {
        if (commandingPlayer == null)
        {
            commandingPlayer = this;
        }
        else if (commandingPlayer != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    // 테스트 용으로 OnEnable, Start 등에 뒀지만 이후 StageChange 등으로 분기할 것
    void OnEnable()
    {
        var formation = board.transform.Find("LeftFormation");

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
            elem.formation = false;
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
            List<CharacterBase> enemies = DungeonManager.dungeonManager.unitOnStage;
            if (enemies == null || enemies.Count == 0)
            {
                isBattle = false;
                break;
            }

            // 공격자 지정
            CharacterBase attacker = null;
            while (attackQueue.Count > 0)
            {
                CharacterBase selectedAttacker = attackQueue.Dequeue();
                if (selectedAttacker != null && selectedAttacker.CanAttack)
                {
                    attacker = selectedAttacker;
                    break;
                }
                // 죽은 후보는 그냥 버림(재등록 X)
            }

            // 타겟 지정
            CharacterBase target = SetTarget(enemies, attacker);
            // null이 리턴되면 배틀 종료
            if (target == null)
            {
                break;
            }

            // 스피드에 따른 딜레이 지정
            float wait = GetDelay(attacker.CurrentSpeed);
            yield return new WaitForSeconds(wait);

            // 공격 실제 적용 후 다시 큐로
            yield return StartCoroutine(attacker.Attack(target));
            GameManager.gameManager.ApplyDamage(attacker, target);
            attackQueue.Enqueue(attacker);
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
            Debug.Log("BattleEnd");
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
